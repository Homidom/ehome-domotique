Imports System.Xml
Imports System.Xml.XPath
Imports System.Runtime.Serialization.Formatters.Soap
Imports System.Threading
Imports System.IO
Imports System.Net.NetworkInformation
Imports eHomeApi.eHomeApi

Namespace eHomeApi

    Public Class Server
        Inherits MarshalByRefObject
        Implements IeHomeServer

#Region "Variable"
        'VARIABLES
        Dim _NameConfig As String 'nom de la config XML
        Dim _IP As String 'IP Network
        Dim _PortTCP As String = 8888 'Port Network TCP
        Dim _PortUDP As String = 8888 'Port Network UDP
        Dim _Longitude As Double 'Longitude
        Dim _Latitude As Double 'Latitude
        Dim _FileConfig As String 'Chemin complet du fichier de config
        Public Shared _HistorisationDayMax As Integer = 365 'nombre de jour de rétention de données dans un fichier si > on crée nouveau fichier
        Dim WithEvents Timer_Seconde As New Timers.Timer
        Dim _FlagMnS As Boolean = False 'Deviens true à la première minute après démarrage
        'Dim WithEvents Timer_200ms As New Timers.Timer

        Public Shared WithEvents m_ListAdapters As New ArrayList 'Lste des adapers
        Public Shared WithEvents m_ListDevices As New ArrayList 'Liste des devices
        Public Shared m_ListZones As New ArrayList 'Liste des zones
        Public Shared WithEvents m_ListSchedules As New ArrayList  'Liste des schedulers
        Public Shared WithEvents m_ListScripts As New ArrayList 'Liste des scripts
        Public Shared WithEvents m_ListTriggers As New ArrayList 'Liste des triggers
        Public Shared WithEvents m_ListMenus As New ArrayList 'Liste des menus
        Public Shared m_ListHistorisations As New ArrayList 'liste des historisations
        Public Shared m_listVariables As New ArrayList 'liste des variables
        Public Shared _Log As New Log

        'Variables specifiques
        Private soleil = New Soleil
        Dim var_soleil_lever As Date = DateAndTime.Now.ToString("yyyy-MM-dd") & " 07:00:00" 'heure du lever du soleil par defaut      
        Dim var_soleil_coucher As Date = DateAndTime.Now.ToString("yyyy-MM-dd") & " 21:00:00" 'heure de coucher du soleil par defaut      
        Public Shared var_soleil_lever2 As DateTime = DateAndTime.Now.ToString("yyyy-MM-dd") & " 07:00:00" 'heure du lever du soleil par defaut      
        Public Shared var_soleil_coucher2 As DateTime = DateAndTime.Now.ToString("yyyy-MM-dd") & " 21:00:00" 'heure de coucher du soleil par defaut  

        Dim heure_coucher_correction As Integer
        Dim heure_lever_correction As Integer
        Shared _IsJour As Boolean
        Shared _IsNuit As Boolean
#End Region

#Region "Propriétés"
        'PROPRIETES
        Public Property NameConfig() As String
            Get
                Return _NameConfig
            End Get
            Set(ByVal value As String)
                _NameConfig = value
            End Set
        End Property

        Public Property IPServer() As String
            Get
                Return _IP
            End Get
            Set(ByVal value As String)
                _IP = value
            End Set
        End Property

        Public Property PortTCP() As String
            Get
                Return _PortTCP
            End Get
            Set(ByVal value As String)
                _PortTCP = value
            End Set
        End Property

        Public Property PortUDP() As String
            Get
                Return _PortUDP
            End Get
            Set(ByVal value As String)
                _PortUDP = value
            End Set
        End Property

        Public Shared ReadOnly Property IsJour() As Boolean
            Get
                Return _IsJour
            End Get
        End Property

        Public Shared ReadOnly Property IsNuit() As Boolean
            Get
                Return _IsNuit
            End Get
        End Property
#End Region

#Region "Hors interface"
        Public Sub New()
            _Log.Fichier = "c:\ehome\log\Servicehome.log"
            Timer_Seconde.Interval = 1000
            AddHandler Timer_Seconde.Elapsed, AddressOf Timer_Seconde_Tick

            'Dim myX10 As Object = Nothing
            'myX10 = CreateObject("cm11a.controlcm")
            'myX10.init()
            'myX10 = Nothing
        End Sub

        'Evenement quand un device a été modifié
        Public Sub DeviceChange(ByVal Id As String, ByVal Reason As String, ByVal Parametres As Object)
            'On va vérifier si un trigger est impacté
            Dim _trigger As New ThreadTrigger(Id, Reason, Parametres, m_ListTriggers)
            Dim x As New Thread(AddressOf _trigger.Traite)
            x.Name = "Traitement des triggers"
            x.Start()
            x = Nothing
        End Sub

#Region "Traitement des scripts"
        Shared Sub RunScript(ByVal ScriptId As String)
            Dim x As Script = Nothing
            For i As Integer = 0 To m_ListScripts.Count - 1
                If m_ListScripts.Item(i).id = ScriptId Then
                    x = m_ListScripts.Item(i)
                    Exit For
                End If
            Next
            If x IsNot Nothing Then
                If x.Enable = False Then
                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script non lancé car non Enable: " & x.Name)
                    Exit Sub
                End If
                'On va lancer le script dans un thread
                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Demarrage du script: " & x.Name)
                Dim _script As New ThreadScript(x)
                Dim y As New Thread(AddressOf _script.Lance)
                y.Name = "Traitement du script"
                y.Start()
                y.Priority = ThreadPriority.Normal
                x = Nothing
                y = Nothing
            End If
        End Sub
#End Region

        Private Sub Timer_Seconde_Tick()

            'Actions à effectuer toutes les secondes

            'on traite les schedules
            If Schedules.Count > 0 Then 'il y a au moins 1 schedule
                For j As Integer = 0 To Schedules.Count - 1
                    If Schedules.Item(j).enable = True Then
                        Dim _schedule As New ThreadSchedule(Schedules, Schedules.Item(j).Id)
                        Dim x As New Thread(AddressOf _schedule.Traite)
                        x.Name = "Traitement du schedule " & j
                        x.Priority = ThreadPriority.Normal
                        x.Start()
                        _schedule = Nothing
                        x = Nothing
                    End If
                Next
            End If
            '------------------------

            'Actions à effectuer toutes les minutes
            If Now.Second = 1 Then
                If _FlagMnS = False Then 'deviens true à la prochaine minute après le 1er démarrage du service
                    '---------- Enregistrement historisation au 1er démarrage ----
                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Lancement de l'historisation au premier démarrage")
                    Dim _histo1 As New ThreadHistorisation
                    Dim f As New Thread(AddressOf _histo1.Traite)
                    f.Priority = ThreadPriority.Normal
                    f.Start()
                    _histo1 = Nothing
                    f = Nothing
                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                    _FlagMnS = True
                End If
            End If
            '------------------------

            'Actions à effectuer toutes les heures
            If Now.Minute = 59 And Now.Second = 59 Then
                Dim _histo As New ThreadHistorisation
                Dim x As New Thread(AddressOf _histo.Traite)
                x.Priority = ThreadPriority.Normal
                x.Start()
                _histo = Nothing
                x = Nothing
            End If
            '------------------------

            'Actions à effectuer à minuit
            If Now.Hour = 0 And Now.Minute = 0 And Now.Second = 0 Then
                Dim thrMnt As New Thread(AddressOf MAJ_HeuresSoleil)
                thrMnt.Start()
                thrMnt = Nothing
            End If
            '------------------------

            'Actions à effectuer à midi
            If Now.Hour = 12 And Now.Minute = 0 And Now.Second = 0 Then
                Dim thrMdi As New Thread(AddressOf MAJ_HeuresSoleil)
                thrMdi.Start()
                thrMdi = Nothing
            End If
        End Sub

        Public Sub MAJ_HeuresSoleil()
            '---------- Initialisation des heures du soleil -------              etape_startup = 21             
            Dim dtSunrise As Date
            Dim dtSolarNoon As Date
            Dim dtSunset As Date

            soleil.CalculateSolarTimes(_Latitude, _Longitude, Date.Now, dtSunrise, dtSolarNoon, dtSunset)
            _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Initialisation des heures du soleil")
            var_soleil_lever = dtSunrise
            var_soleil_coucher = dtSunset
            var_soleil_coucher2 = DateAdd(DateInterval.Minute, heure_coucher_correction, var_soleil_coucher)
            var_soleil_lever2 = DateAdd(DateInterval.Minute, heure_lever_correction, var_soleil_lever)

            If Now >= var_soleil_lever2 And Now <= var_soleil_coucher2 Then
                _IsJour = True
            Else
                _IsJour = False
            End If

            _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "     -> Heure du lever : " & var_soleil_lever2)
            _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "     -> Heure du coucher : " & var_soleil_coucher2)

            'génération de l'event
            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
            msg.EventTime = Now
            msg.Reason = IeHomeServer.Reason.UNKNOWN_REASON
            msg.Message = "MAJ HEURES SOLEIL"
            RaiseEvent MessageArrivedHandler(msg)

            If Now.ToString = var_soleil_lever2 Then
                'génération de l'event
                msg.EventTime = Now
                msg.Reason = IeHomeServer.Reason.SUNRISE
                msg.Message = "LEVER_SOLEIL"
                RaiseEvent MessageArrivedHandler(msg)
            End If

            If Now.ToString = var_soleil_coucher2 Then
                'génération de l'event
                msg.EventTime = Now
                msg.Reason = IeHomeServer.Reason.SUNSET
                msg.Message = "COUCHER_SOLEIL"
                RaiseEvent MessageArrivedHandler(msg)
            End If

            msg = Nothing
        End Sub

        Public Sub AdaptersEvent(ByVal PluginName As String, ByVal TypeMessage As String, ByVal Data As String)
            Select Case PluginName
                Case "rfid"
                    Dim a() As String = Data.Split("|")
                    Select Case a(0)
                        Case "0"
                            Dim _adress As String = a(1)
                            _Log.AddToLog(Log.TypeLog.MESSAGE, "rfid", Data)
                            For i As Integer = 0 To m_ListDevices.Count - 1
                                If m_ListDevices.Item(i).adresse = _adress Then
                                    m_ListDevices.Item(i).status = False
                                    Exit For
                                    Exit Select
                                End If
                            Next
                        Case "1"
                            Dim _adress As String = a(1)
                            _Log.AddToLog(Log.TypeLog.MESSAGE, "rfid", Data)
                            For i As Integer = 0 To m_ListDevices.Count - 1
                                If m_ListDevices.Item(i).adresse = _adress Then
                                    m_ListDevices.Item(i).status = True
                                    Exit For
                                    Exit Select
                                End If
                            Next
                        Case "2"
                            _Log.AddToLog(Log.TypeLog.MESSAGE, "rfid", Data)
                        Case "3"
                            _Log.AddToLog(Log.TypeLog.MESSAGE, "rfid", Data)
                    End Select
            End Select

            'génération de l'event
            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
            msg.EventTime = Now
            msg.Reason = TypeMessage
            msg.Message = Data
            RaiseEvent MessageArrivedHandler(msg)

            msg = Nothing
        End Sub

        Public Sub LoadAction(ByVal Source As Object, ByVal list As XmlNode, ByVal TypeAction As String)
            Select Case TypeAction
                Case "DEVICE"
                    Dim obj As New Script.ClassDevice
                    obj.IDDevice = list.ChildNodes.Item(1).InnerText
                    obj.DeviceFunction = list.ChildNodes.Item(2).InnerText
                    obj.Value = list.ChildNodes.Item(3).InnerText
                    Source.Add(obj)
                Case "SCRIPT"
                    Dim obj As New Script.ClassScript
                    obj.IDScript = list.ChildNodes.Item(1).InnerText
                    Source.Add(obj)
                Case "MAIL"
                    Dim obj As New Script.ClassEmail
                    obj.MailServerSMTP = list.ChildNodes.Item(1).InnerText
                    obj.MailServerIdentification = list.ChildNodes.Item(2).InnerText
                    obj.MailServerLogin = list.ChildNodes.Item(3).InnerText
                    obj.MailServerPassword = list.ChildNodes.Item(4).InnerText
                    obj.From = list.ChildNodes.Item(5).InnerText
                    obj.A = list.ChildNodes.Item(6).InnerText
                    obj.Sujet = list.ChildNodes.Item(7).InnerText
                    obj.Message = list.ChildNodes.Item(8).InnerText
                    Source.Add(obj)
                Case "PAUSE"
                    Dim obj As New Script.ClassPause
                    obj.Heure = list.ChildNodes.Item(1).InnerText
                    obj.Minute = list.ChildNodes.Item(2).InnerText
                    obj.Seconde = list.ChildNodes.Item(3).InnerText
                    obj.MilliSeconde = list.ChildNodes.Item(4).InnerText
                    Source.Add(obj)
                Case "EXIT"
                    Dim obj As New Script.ClassExit
                    Source.Add(obj)
                Case "SCRIPT_VB"
                    Dim obj As New Script.ClassVBScript
                    obj.Script = list.ChildNodes.Item(1).InnerText
                    Source.Add(obj)
                Case "PARLER"
                    Dim obj As New Script.ClassVoice
                    obj.Message = list.ChildNodes.Item(1).InnerText
                    Source.Add(obj)
                    'ATTENTION UN IF NE PAS INCLURE UN AUTRE IF
                Case "IF"
                    Dim obj As New Script.ClassIf
                    For k2 As Integer = 0 To list.ChildNodes.Count - 1
                        'chargement des conditions
                        If list.ChildNodes.Item(k2).Name = "condition" Then
                            Dim obj2 As New Script.ClassIf.Condition
                            With obj2
                                .TypeCondition = list.ChildNodes.Item(k2).ChildNodes.Item(0).InnerText
                                .ItemId = list.ChildNodes.Item(k2).ChildNodes.Item(1).InnerText
                                .Parametre = list.ChildNodes.Item(k2).ChildNodes.Item(2).InnerText
                                .Value = list.ChildNodes.Item(k2).ChildNodes.Item(3).InnerText
                                .Operateur = list.ChildNodes.Item(k2).ChildNodes.Item(4).InnerText
                            End With
                            obj.ListCondition.Add(obj2)
                        End If
                        'chargement des actions Then
                        If list.ChildNodes.Item(k2).Name = "then" Then
                            For k3 As Integer = 0 To list.ChildNodes.Item(k2).ChildNodes.Count - 1
                                If list.ChildNodes.Item(k2).ChildNodes.Item(k3).Name = "action" Then
                                    If list.ChildNodes.Item(k2).ChildNodes.Item(k3).HasChildNodes Then
                                        Dim _TypeAction2 As String = list.ChildNodes.Item(k2).ChildNodes.Item(k3).ChildNodes.Item(0).InnerText
                                        LoadAction(obj.ThenListAction, list.ChildNodes.Item(k2).ChildNodes.Item(k3), _TypeAction2)
                                    End If
                                End If
                            Next
                        End If
                        'chargement des actions else
                        If list.ChildNodes.Item(k2).Name = "else" Then
                            For k3 As Integer = 0 To list.ChildNodes.Item(k2).ChildNodes.Count - 1
                                If list.ChildNodes.Item(k2).ChildNodes.Item(k3).Name = "action" Then
                                    If list.ChildNodes.Item(k2).ChildNodes.Item(k3).HasChildNodes Then
                                        Dim _TypeAction2 As String = list.ChildNodes.Item(k2).ChildNodes.Item(k3).ChildNodes.Item(0).InnerText
                                        LoadAction(obj.ElseListAction, list.ChildNodes.Item(k2).ChildNodes.Item(k3), _TypeAction2)
                                    End If
                                End If
                            Next
                        End If

                    Next
                    Source.Add(obj)
            End Select
        End Sub

       
#End Region

#Region "Gestion Fonction/Sub ServiceWeb"

#Region "Propriétés du serveur"
        Public Property LogFile() As String Implements IeHomeServer.LogFile
            Get
                Return _Log.Fichier
            End Get
            Set(ByVal value As String)
                _Log.Fichier = value
            End Set
        End Property

        Public Property LogMaxSize() As Double Implements IeHomeServer.LogMaxSize
            Get
                Return _Log.MaxFileSize
            End Get
            Set(ByVal value As Double)
                _Log.MaxFileSize = value
            End Set
        End Property

        Public Property Longitude() As Double Implements IeHomeServer.Longitude
            Get
                Return _Longitude
            End Get
            Set(ByVal value As Double)
                _Longitude = value
            End Set
        End Property

        Public Property Latitude() As Double Implements IeHomeServer.Latitude
            Get
                Return _Latitude
            End Get
            Set(ByVal value As Double)
                _Latitude = value
            End Set
        End Property

        Public Property HeureCorrectionCoucher() As Integer Implements IeHomeServer.HeureCorrectionCoucher
            Get
                Return heure_coucher_correction
            End Get
            Set(ByVal value As Integer)
                heure_coucher_correction = value
            End Set
        End Property

        Public Property HeureCorrectionLever() As Integer Implements IeHomeServer.HeureCorrectionLever
            Get
                Return heure_lever_correction
            End Get
            Set(ByVal value As Integer)
                heure_lever_correction = value
            End Set
        End Property

        Public Property HistorisationDayMax() As Integer Implements IeHomeServer.HistorisationDayMax
            Get
                Return _HistorisationDayMax
            End Get
            Set(ByVal value As Integer)
                _HistorisationDayMax = value
            End Set
        End Property
#End Region

#Region "Liste des propriétées Liste partagées"
        Public Property Adapters() As ArrayList Implements IeHomeServer.Adapters
            Get
                Return m_ListAdapters
            End Get
            Set(ByVal value As ArrayList)
                m_ListAdapters = value
            End Set
        End Property

        Public Property Devices() As ArrayList Implements IeHomeServer.Devices
            Get
                Return m_ListDevices
            End Get
            Set(ByVal value As ArrayList)
                m_ListDevices = value
            End Set
        End Property

        Public Property Zones() As ArrayList Implements IeHomeServer.Zones
            Get
                Return m_ListZones
            End Get
            Set(ByVal value As ArrayList)
                m_ListZones = value
            End Set
        End Property

        Public Property Schedules() As ArrayList Implements IeHomeServer.Schedules
            Get
                Return m_ListSchedules
            End Get
            Set(ByVal value As ArrayList)
                m_ListSchedules = value
            End Set
        End Property

        Public Property Scripts() As ArrayList Implements IeHomeServer.Scripts
            Get
                Return m_ListScripts
            End Get
            Set(ByVal value As ArrayList)
                m_ListScripts = value
            End Set
        End Property

        Public Property Triggers() As ArrayList Implements IeHomeServer.Triggers
            Get
                Return m_ListTriggers
            End Get
            Set(ByVal value As ArrayList)
                m_ListTriggers = value
            End Set
        End Property

        Public Property Menus() As ArrayList Implements IeHomeServer.Menus
            Get
                Return m_ListMenus
            End Get
            Set(ByVal value As ArrayList)
                m_ListMenus = value
            End Set
        End Property

        Public Property Historisations() As ArrayList Implements IeHomeServer.Historisations
            Get
                Return m_ListHistorisations
            End Get
            Set(ByVal value As ArrayList)
                m_ListHistorisations = value
            End Set
        End Property

        Public Property Variables() As ArrayList Implements IeHomeServer.Variables
            Get
                Return m_listVariables
            End Get
            Set(ByVal value As ArrayList)
                m_listVariables = value
            End Set
        End Property
#End Region

#Region "Configuration"
        '******************************
        'Charger la configuration
        '******************************

        Public Function LoadConfig(ByVal FileXML As String) As String
            'Si le fichier de config n'existe pas on le crée

            'Copy du fichier de config avant chargement
            Try
                Dim _file As String = FileXML & "wehome_server"
                If File.Exists(_file & ".bak") = True Then File.Delete(_file & ".bak")
                File.Copy(_file & ".xml", Mid(_file & ".xml", 1, Len(_file & ".xml") - 4) & ".bak")
            Catch ex As Exception
                _Log.AddToLog(Log.TypeLog.ERREUR, "Serveur", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
            End Try

            Try
                Dim dirInfo As New System.IO.DirectoryInfo(FileXML)
                Dim file As System.IO.FileInfo
                Dim files() As System.IO.FileInfo = dirInfo.GetFiles("wehome_server.xml")
                Dim myxml As XML

                If (files IsNot Nothing) Then
                    For Each file In files
                        Dim myfile As String = file.FullName
                        Dim list As XmlNodeList

                        myxml = New XML(myfile)

                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement du fichier config: " & myfile)

                        '******************************************
                        'on va chercher les paramètres du serveur
                        '******************************************
                        list = myxml.SelectNodes("/wehome/server")
                        If list.Count > 0 Then 'présence des paramètres server
                            For j As Integer = 0 To list.Item(0).ChildNodes.Count - 1
                                If list.Item(0).ChildNodes.Item(j).Name = "ipnetwork" Then
                                    IPServer = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "IP: " & IPServer)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "portnetworktcp" Then
                                    PortTCP = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Port TCP: " & PortTCP)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "portnetworkudp" Then
                                    PortUDP = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Port UDP: " & PortUDP)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "longitude" Then
                                    Longitude = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Longitude: " & Longitude)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "latitude" Then
                                    Latitude = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Latitude: " & Latitude)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "heurecorrectionlever" Then
                                    HeureCorrectionLever = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Correction heure lever: " & HeureCorrectionLever)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "heurecorrectioncoucher" Then
                                    HeureCorrectionCoucher = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Correction heure coucher: " & HeureCorrectionCoucher)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "logfile" Then
                                    _Log.Fichier = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Fichier Log: " & _Log.Fichier)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "logmaxsize" Then
                                    _Log.MaxFileSize = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Taille Max fichier log: " & _Log.MaxFileSize)
                                End If
                                If list.Item(0).ChildNodes.Item(j).Name = "histmaxday" Then
                                    HistorisationDayMax = list.Item(0).ChildNodes.Item(j).InnerText
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Nombre de jour du fichier historisation: " & HistorisationDayMax)
                                End If
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Paramètres du serveur chargés")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")

                        '********************************
                        'on va chercher les services
                        '*********************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des drivers")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/service")

                        If list.Count > 0 Then 'présence d'un driver
                            For j As Integer = 0 To list.Count - 1
                                Dim _namedrv As String = list.Item(j).ChildNodes.Item(1).InnerText
                                Dim _drv As Object = Nothing

                                Select Case _namedrv
                                    Case "usbuirt"
                                        Dim x As New Drivers.UsbUIRT
                                        AddHandler x.SendMessage, AddressOf AdaptersEvent
                                        _drv = x
                                    Case "virtuel"
                                        Dim x As New Drivers.DriverVirtuel
                                        AddHandler x.SendMessage, AddressOf AdaptersEvent
                                        _drv = x
                                    Case "rfid"
                                        Dim x As New Drivers.Rfid
                                        AddHandler x.SendMessage, AddressOf AdaptersEvent
                                        _drv = x
                                    Case "k8055"
                                        Dim x As New Drivers.k8055
                                        AddHandler x.SendMessage, AddressOf AdaptersEvent
                                        _drv = x
                                    Case "k8056"
                                        Dim x As New Drivers.k8056
                                        AddHandler x.SendMessage, AddressOf AdaptersEvent
                                        _drv = x
                                    Case "1wire"
                                        Dim x As New Drivers.OneWire
                                        AddHandler x.SendMessage, AddressOf AdaptersEvent
                                        _drv = x
                                    Case "x10"
                                        Dim x As New Drivers.X10
                                        AddHandler x.SendMessage, AddressOf AdaptersEvent
                                        _drv = x
                                End Select
                                If _drv IsNot Nothing Then
                                    _drv.Id = list.Item(j).ChildNodes.Item(0).InnerText 'Api.GenerateGUID
                                    _drv.Protocol = list.Item(j).ChildNodes.Item(2).InnerText
                                    _drv.Port = list.Item(j).ChildNodes.Item(3).InnerText
                                    _drv.AutoStart = list.Item(j).ChildNodes.Item(4).InnerText
                                    _drv.Parametres = list.Item(j).ChildNodes.Item(5).InnerText
                                    m_ListAdapters.Add(_drv)
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "")
                                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Plugin " & _drv.PluginName & " chargé")
                                    If _drv.AutoStart = True Then _Log.AddToLog(Log.TypeLog.INFO, _drv.pluginName, "Start service: " & _drv.Start)
                                    _drv = Nothing
                                End If
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_ListAdapters.Count & " driver(s) trouvé(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")

                        '******************************************
                        'on va chercher les devices
                        '********************************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des devices")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/devices/device")

                        If list.Count > 0 Then 'présence d'un device
                            For j As Integer = 0 To list.Count - 1
                                'Commun à tous les devices
                                Dim ID As String = list.Item(j).ChildNodes.Item(1).InnerText
                                Dim name As String = list.Item(j).ChildNodes.Item(2).InnerText
                                Dim image As String = list.Item(j).ChildNodes.Item(6).InnerText
                                Dim adapter As String = list.Item(j).ChildNodes.Item(5).InnerText
                                Dim address As String = list.Item(j).ChildNodes.Item(3).InnerText
                                Dim enable As Boolean = list.Item(j).ChildNodes.Item(4).InnerText
                                Dim _Dev As Object

                                'Suivant chaque type de device
                                Select Case list.Item(j).ChildNodes.Item(0).InnerText
                                    Case "lightingdimmer"
                                        Dim o As New Device.Lighting.Dimmer
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "temperature"
                                        Dim o As New Device.Temperature
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "web"
                                        Dim o As New Device.PageWeb
                                        _Dev = o
                                        o = Nothing
                                    Case "contact"
                                        Dim o As New Device.Contact
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "audiozone"
                                        Dim o As New Device.Audio.AudioZone
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "meteo"
                                        Dim o As New Device.Meteo
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "freebox"
                                        Dim o As New Device.Media.Freebox
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        _Dev = o
                                        o = Nothing
                                    Case "tv"
                                        Dim o As New Device.Media.TV
                                        AddHandler o.DeviceChanged, AddressOf DeviceChange
                                        For k As Integer = 0 To list.Item(j).ChildNodes.Count - 1
                                            If list.Item(j).ChildNodes.Item(k).Name = "commands" Then
                                                o.ListCommandName.Clear()
                                                o.ListCommandData.Clear()
                                                o.ListCommandRepeat.Clear()
                                                For k1 As Integer = 0 To list.Item(j).ChildNodes.Item(k).ChildNodes.Count - 1
                                                    o.ListCommandName.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(0).Value)
                                                    o.ListCommandData.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(1).Value)
                                                    o.ListCommandRepeat.Add(list.Item(j).ChildNodes.Item(k).ChildNodes.Item(k1).Attributes(2).Value)
                                                Next
                                            End If
                                        Next
                                        _Dev = o
                                        o = Nothing
                                End Select
                                With _Dev
                                    .ID = ID
                                    .Name = name
                                    .Picture = image
                                    .DriverID = adapter
                                    .Adresse = address
                                    .Enable = enable
                                    .Driver = ReturnDriverById(adapter)
                                End With
                                m_ListDevices.Add(_Dev)
                                _Dev = Nothing

                                ID = Nothing
                                name = Nothing
                                image = Nothing
                                adapter = Nothing
                                address = Nothing
                                enable = Nothing
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_ListDevices.Count & " devices(s) trouvé(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")

                        '******************************************
                        'on va chercher les zones
                        '********************************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des zones")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/zones/zone")

                        If list.Count > 0 Then 'présence d'une zone
                            For j As Integer = 0 To list.Count - 1
                                Dim _zone As New Zone

                                With _zone
                                    .ID = list.Item(j).ChildNodes.Item(0).InnerText
                                    .Name = list.Item(j).ChildNodes.Item(1).InnerText
                                    .Image = list.Item(j).ChildNodes.Item(2).InnerText
                                    For k3 As Integer = 0 To list.Item(j).ChildNodes.Count - 1
                                        If list.Item(j).ChildNodes.Item(k3).Name = "deviceid" Then
                                            _zone.ListDeviceId.Add(list.Item(j).ChildNodes.Item(k3).InnerText)
                                        End If
                                    Next
                                End With
                                m_ListZones.Add(_zone)
                                _zone = Nothing
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_ListZones.Count & " zone(s) trouvée(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                        list = Nothing

                        '******************************************
                        'on va chercher les historisations
                        '********************************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des historisations")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/histos/histo")

                        If list.Count > 0 Then 'présence d'une historisation
                            For j As Integer = 0 To list.Count - 1
                                Dim _histo As New Historisation

                                With _histo
                                    .ID = list.Item(j).ChildNodes.Item(0).InnerText
                                    .Enable = list.Item(j).ChildNodes.Item(1).InnerText
                                    .DeviceId = list.Item(j).ChildNodes.Item(2).InnerText
                                    .PropertyDevice = list.Item(j).ChildNodes.Item(3).InnerText
                                End With
                                m_ListHistorisations.Add(_histo)
                                _histo = Nothing
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_ListHistorisations.Count & " historisation(s) trouvée(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                        list = Nothing


                        '******************************************
                        'on va chercher les variables
                        '********************************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des variables")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/variables/variable")

                        If list.Count > 0 Then 'présence d'une variable
                            For j As Integer = 0 To list.Count - 1
                                Dim _var As New Variable

                                With _var
                                    .ID = list.Item(j).ChildNodes.Item(0).InnerText
                                    .Name = list.Item(j).ChildNodes.Item(1).InnerText
                                    .Type = list.Item(j).ChildNodes.Item(2).InnerText
                                    .Value = list.Item(j).ChildNodes.Item(3).InnerText
                                    .DefaultValue = list.Item(j).ChildNodes.Item(4).InnerText
                                End With
                                m_listVariables.Add(_var)
                                _var = Nothing
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_listVariables.Count & " variable(s) trouvée(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                        list = Nothing


                        '******************************************
                        'on va chercher les schedules
                        '********************************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des schedules")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/schedules/schedule")

                        If list.Count > 0 Then 'présence d'un schedule
                            For j As Integer = 0 To list.Count - 1
                                Dim _Sch As New Schedule

                                With _Sch
                                    .ID = list.Item(j).ChildNodes.Item(0).InnerText
                                    .Name = list.Item(j).ChildNodes.Item(1).InnerText
                                    .Enable = list.Item(j).ChildNodes.Item(2).InnerText
                                    .TriggerType = list.Item(j).ChildNodes.Item(3).InnerText
                                    .RecurrencePatternList = list.Item(j).ChildNodes.Item(4).InnerText
                                    .StartDateTime = list.Item(j).ChildNodes.Item(5).InnerText
                                    Dim _tmpJ As String = Format(list.Item(j).ChildNodes.Item(6).InnerText, "0000000")
                                    .Jour(0) = Mid(_tmpJ, 1, 1)
                                    .Jour(1) = Mid(_tmpJ, 2, 1)
                                    .Jour(2) = Mid(_tmpJ, 3, 1)
                                    .Jour(3) = Mid(_tmpJ, 4, 1)
                                    .Jour(4) = Mid(_tmpJ, 5, 1)
                                    .Jour(5) = Mid(_tmpJ, 6, 1)
                                    .Jour(6) = Mid(_tmpJ, 7, 1)
                                    .AsEnd = list.Item(j).ChildNodes.Item(7).InnerText
                                    .EndDateTime = list.Item(j).ChildNodes.Item(8).InnerText
                                    .SunRiseBefore = list.Item(j).ChildNodes.Item(9).InnerText
                                    .SunRiseBeforeTime = list.Item(j).ChildNodes.Item(10).InnerText
                                    .SunSetBefore = list.Item(j).ChildNodes.Item(11).InnerText
                                    .SunsetBeforeTime = list.Item(j).ChildNodes.Item(12).InnerText
                                    .LastSchedule = list.Item(j).ChildNodes.Item(13).InnerText

                                    If list.Item(j).ChildNodes.Item(14).Name = "scripts" Then
                                        For c As Integer = 0 To list.Item(j).ChildNodes.Item(14).ChildNodes.Count - 1
                                            .ListScript.Add(list.Item(j).ChildNodes.Item(14).ChildNodes.Item(c).InnerText)
                                        Next
                                    End If

                                End With
                                m_ListSchedules.Add(_Sch)
                                _Sch = Nothing
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_ListSchedules.Count & " schedules(s) trouvé(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                        list = Nothing

                        '******************************************
                        'on va chercher les triggers
                        '********************************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des triggers")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/triggers/trigger")

                        If list.Count > 0 Then 'présence d'un trigger
                            For j As Integer = 0 To list.Count - 1

                                Dim _Trg As New Trigger
                                With _Trg
                                    .Id = list.Item(j).ChildNodes.Item(0).InnerText
                                    .Name = list.Item(j).ChildNodes.Item(1).InnerText
                                    .Enable = list.Item(j).ChildNodes.Item(2).InnerText
                                    .DeviceId = list.Item(j).ChildNodes.Item(3).InnerText
                                    .Status = list.Item(j).ChildNodes.Item(4).InnerText
                                    .Condition = list.Item(j).ChildNodes.Item(5).InnerText
                                    .Value = list.Item(j).ChildNodes.Item(6).InnerText

                                    If list.Item(j).ChildNodes.Item(7).Name = "scripts" Then
                                        For c As Integer = 0 To list.Item(j).ChildNodes.Item(7).ChildNodes.Count - 1
                                            .ListScript.Add(list.Item(j).ChildNodes.Item(7).ChildNodes.Item(c).InnerText)
                                        Next
                                    End If

                                End With
                                m_ListTriggers.Add(_Trg)
                                _Trg = Nothing
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_ListTriggers.Count & " trigger(s) trouvé(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                        list = Nothing


                        '******************************************
                        'on va chercher les scripts
                        '********************************************
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Chargement des scripts")
                        list = Nothing
                        list = myxml.SelectNodes("/wehome/scripts/script")

                        If list.Count > 0 Then 'présence d'un script
                            For j As Integer = 0 To list.Count - 1

                                Dim _Scrp As New Script
                                With _Scrp
                                    .ID = list.Item(j).ChildNodes.Item(0).InnerText
                                    .Name = list.Item(j).ChildNodes.Item(1).InnerText
                                    .Enable = list.Item(j).ChildNodes.Item(2).InnerText

                                    For k As Integer = 0 To list.Item(j).ChildNodes.Count - 1
                                        If list.Item(j).ChildNodes.Item(k).Name = "action" Then
                                            Dim _TypeAction As String = list.Item(j).ChildNodes.Item(k).ChildNodes.Item(0).InnerText
                                            LoadAction(_Scrp.ListAction, list.Item(j).ChildNodes.Item(k), _TypeAction)
                                        End If
                                    Next

                                End With
                                m_ListScripts.Add(_Scrp)
                                _Scrp = Nothing
                            Next
                        End If
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", m_ListScripts.Count & " script(s) trouvé(s)")
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                        list = Nothing
                    Next
                End If

                '---------- Initialisation des heures du soleil -------              etape_startup = 21             
                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Initialisation des heures du soleil")
                Dim thr As New Thread(AddressOf MAJ_HeuresSoleil)
                thr.Start()
                thr = Nothing
                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " ")
                thr = Nothing


                '*************
                'TEST
                '******************

                'Vide les variables
                dirInfo = Nothing
                file = Nothing
                files = Nothing
                myxml = Nothing

                Timer_Seconde.Enabled = True

                Return " Chargement de la configuration terminée"

            Catch ex As Exception
                Return " Erreur de chargement de la config: " & ex.Message
            End Try
        End Function

        'Creation d'un nouveau fichier de config
        'retourne si rien si aucune erreur
        Public Function NewConfig(ByVal Name As String) As String
            Dim retour As String = ""

            If File.Exists("C:\ehome\config\" & Name & ".xml") = True Then
                retour = "Impossible de créer ce fichier le nom de configuration existe déjà!"
            End If

            Try
                Dim writer As New XmlTextWriter("C:\ehome\config\" & Name & ".xml", System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 2

                writer.WriteStartElement("wehome")

                '------------ server
                writer.WriteStartElement("server")
                writer.WriteStartElement("nameconfig")
                writer.WriteString(Name)
                writer.WriteEndElement()
                writer.WriteStartElement("ipnetwork")
                writer.WriteString(IPServer)
                writer.WriteEndElement()
                writer.WriteStartElement("portnetworktcp")
                writer.WriteString(PortTCP)
                writer.WriteEndElement()
                writer.WriteStartElement("portnetworkudp")
                writer.WriteString(PortUDP)
                writer.WriteEndElement()
                writer.WriteStartElement("longitude")
                writer.WriteString(Longitude)
                writer.WriteEndElement()
                writer.WriteStartElement("latitude")
                writer.WriteString(Latitude)
                writer.WriteEndElement()
                writer.WriteStartElement("heurecorrectionlever")
                writer.WriteString(HeureCorrectionLever)
                writer.WriteEndElement()
                writer.WriteStartElement("heurecorrectioncoucher")
                writer.WriteString(HeureCorrectionCoucher)
                writer.WriteEndElement()
                writer.WriteEndElement()

                '-------------------
                '------------service
                '------------------
                For i As Integer = 0 To m_ListAdapters.Count - 1
                    If m_ListAdapters.Item(i).PluginName <> "server" Then
                        writer.WriteStartElement("service")
                        writer.WriteStartElement("id")
                        writer.WriteString(m_ListAdapters.Item(i).ID)
                        writer.WriteEndElement()
                        writer.WriteStartElement("name")
                        writer.WriteString(m_ListAdapters.Item(i).PluginName)
                        writer.WriteEndElement()
                        writer.WriteStartElement("protocol")
                        writer.WriteString(m_ListAdapters.Item(i).Protocol)
                        writer.WriteEndElement()
                        writer.WriteStartElement("port")
                        writer.WriteString(m_ListAdapters.Item(i).Protocol)
                        writer.WriteEndElement()
                        writer.WriteStartElement("startauto")
                        writer.WriteString(m_ListAdapters.Item(i).AutoStart)
                        writer.WriteEndElement()
                        writer.WriteStartElement("parametres")
                        writer.WriteString(m_ListAdapters.Item(i).Parametres)
                        writer.WriteEndElement()
                        writer.WriteEndElement()
                    End If
                Next

                '------------
                'devices
                '------------
                writer.WriteStartElement("devices")
                writer.WriteEndElement()
                '------------

                '------------
                'zones
                '------------
                writer.WriteStartElement("zones")
                writer.WriteEndElement()
                '------------

                '------------
                'schedules
                '------------
                writer.WriteStartElement("schedules")
                writer.WriteEndElement()
                '------------

                '------------
                'triggers
                '------------
                writer.WriteStartElement("triggers")
                writer.WriteEndElement()
                '------------

                '------------
                'scripts
                '------------
                writer.WriteStartElement("scripts")
                writer.WriteEndElement()
                '------------

                writer.WriteEndDocument()
                writer.Close()
                retour = ""
            Catch ex As Exception
                retour = " Erreur de sauvegarde de la configuration: " & ex.Message
            End Try
            Return retour
        End Function
#End Region


        Public Function DeleteDevice(ByVal deviceId As String) As Integer Implements IeHomeServer.DeleteDevice
            For i As Integer = 0 To m_ListDevices.Count - 1
                If m_ListDevices.Item(i).Id = deviceId Then
                    m_ListDevices.Item(i).ListCommand.removeat(i)

                    'génération de l'event
                    Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
                    msg.EventTime = Now
                    msg.Reason = IeHomeServer.Reason.DELETE_DEVICE
                    msg.Message = 0
                    RaiseEvent MessageArrivedHandler(msg)

                    Exit Function
                End If
            Next
        End Function

        Public Function DeleteDeviceCommand(ByVal deviceId As String, ByVal CmdName As String) As Integer Implements IeHomeServer.DeleteDeviceCommand
            For i As Integer = 0 To m_ListDevices.Count - 1
                If m_ListDevices.Item(i).Id = deviceId Then
                    For j As Integer = 0 To m_ListDevices.Item(i).ListCommandname.count - 1
                        If m_ListDevices.Item(i).ListCommandname(j) = CmdName Then
                            m_ListDevices.Item(i).ListCommandname.removeat(j)
                            m_ListDevices.Item(i).ListCommanddata.removeat(j)
                            m_ListDevices.Item(i).ListCommandrepeat.removeat(j)

                            'génération de l'event
                            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
                            msg.EventTime = Now
                            msg.Reason = IeHomeServer.Reason.DELETE_DEVICE
                            msg.Message = 0
                            RaiseEvent MessageArrivedHandler(msg)

                            Exit Function
                        End If
                    Next
                End If
            Next
        End Function

        Public Function DeleteMacro(ByVal macroID As String) As Integer Implements IeHomeServer.DeleteMacro

        End Function

        Public Function DeleteSchedule(ByVal scheduleId As String) As Integer Implements IeHomeServer.DeleteSchedule
            For i As Integer = 0 To m_ListSchedules.Count - 1
                If m_ListSchedules.Item(i).id = scheduleId Then
                    m_ListSchedules.RemoveAt(i)

                    'génration de l'event
                    Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
                    msg.EventTime = Now
                    msg.Reason = IeHomeServer.Reason.DELETE_SCHEDULE
                    msg.Message = scheduleId
                    RaiseEvent MessageArrivedHandler(msg)

                    Return i
                    Exit Function
                End If
            Next
        End Function

        Public Function DeleteTrigger(ByVal trigId As String) As Integer Implements IeHomeServer.DeleteTrigger
            For i As Integer = 0 To m_ListTriggers.Count - 1
                If m_ListTriggers.Item(i).id = trigId Then
                    m_ListTriggers.RemoveAt(i)

                    'génration de l'event
                    Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
                    msg.EventTime = Now
                    msg.Reason = IeHomeServer.Reason.DELETE_TRIGGER
                    msg.Message = trigId
                    RaiseEvent MessageArrivedHandler(msg)

                    Return i
                    Exit Function
                End If
            Next
        End Function

        Public Function DeleteZone(ByVal zoneId As String) As Integer Implements IeHomeServer.DeleteZone
            For i As Integer = 0 To m_ListZones.Count - 1
                If m_ListZones.Item(i).id = zoneId Then
                    m_ListZones.RemoveAt(i)

                    'génration de l'event
                    Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
                    msg.EventTime = Now
                    msg.Reason = IeHomeServer.Reason.DELETE_ZONE
                    msg.Message = zoneId
                    RaiseEvent MessageArrivedHandler(msg)

                    Return i
                    Exit Function
                End If
            Next
        End Function

        'Couché du soleil
        Public Function GetSunSet() As String Implements IeHomeServer.GetSunSet
            Return var_soleil_coucher2.ToShortTimeString
        End Function

        'Levé du soleil
        Public Function GetSunRise() As String Implements IeHomeServer.GetSunRise
            Return var_soleil_lever2.ToShortTimeString
        End Function

        'Retourne la date et heure
        Public Function GetTime() As DateTime Implements IeHomeServer.GetTime
            Return Now
        End Function

        Public Function GetVersion() As String Implements IeHomeServer.GetVersion
            GetVersion = "1.0"
        End Function

        Public Sub Initialize(ByVal Name As String) Implements IeHomeServer.Initialize

        End Sub

        Public Function IsDeleteZoneAllowed(ByVal zoneId As String) As Boolean Implements IeHomeServer.IsDeleteZoneAllowed
            For i As Integer = 0 To m_ListZones.Count - 1
                If m_ListZones.Item(i).id = zoneId And m_ListZones.Item(i).listdeviceid.count > 0 Then
                    Return True
                End If
            Next
        End Function

        'Evenement publié
        Public Event MessageArrivedHandler(ByVal msg As IeHomeServer.AuServerEventMsg) Implements IeHomeServer.MessageArrivedHandler

        Public Sub MessageToServer(ByVal Message As String) Implements IeHomeServer.MessageToServer

        End Sub

        Public Function MoveDeviceToNewZone(ByVal deviceId As String, ByVal newZoneId As String) As Boolean Implements IeHomeServer.MoveDeviceToNewZone
            For i As Integer = 0 To m_ListZones.Count - 1
                For k As Integer = 0 To m_ListZones.Item(i).Listdeviceid.count - 1
                    If m_ListZones.Item(i).Listdeviceid(k) = deviceId And m_ListZones.Item(i).id <> newZoneId Then
                        m_ListZones.Item(i).Listdeviceid.removeat(k)
                    End If
                    If m_ListZones.Item(i).id = newZoneId Then
                        m_ListZones.Item(i).Listdeviceid.add(deviceId)
                    End If
                Next
            Next
        End Function

        Public Function Ping(ByVal machineName As String) As Boolean Implements IeHomeServer.Ping
            Return My.Computer.Network.Ping(machineName)
        End Function

        'retourne le device par son ID
        Public Function ReturnDeviceById(ByVal DeviceId As String) As Object Implements IeHomeServer.ReturnDeviceByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To m_ListDevices.Count - 1
                If m_ListDevices.Item(i).ID = DeviceId Then
                    retour = m_ListDevices.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne le driver par son ID
        Public Function ReturnDriverById(ByVal DriverId As String) As Object Implements IeHomeServer.ReturnDriverByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To m_ListAdapters.Count - 1
                If m_ListAdapters.Item(i).ID = DriverId Then
                    retour = m_ListAdapters.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne le schedule par son ID
        Public Function ReturnScheduleById(ByVal scheduleId As String) As Object Implements IeHomeServer.ReturnScheduleByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To m_ListSchedules.Count - 1
                If m_ListSchedules.Item(i).ID = scheduleId Then
                    retour = m_ListSchedules.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne le trigger par son ID
        Public Function ReturnTriggerById(ByVal triggerId As String) As Object Implements IeHomeServer.ReturnTriggerByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To m_ListTriggers.Count - 1
                If m_ListTriggers.Item(i).ID = triggerId Then
                    retour = m_ListTriggers.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne le zone par son ID
        Public Function ReturnZoneById(ByVal zoneId As String) As Object Implements IeHomeServer.ReturnZoneByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To m_ListZones.Count - 1
                If m_ListZones.Item(i).ID = zoneId Then
                    retour = m_ListZones.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        'retourne le script par son ID
        Public Function ReturnScriptById(ByVal scriptId As String) As Object Implements IeHomeServer.ReturnScriptByID
            Dim retour As Object = Nothing
            For i As Integer = 0 To m_ListScripts.Count - 1
                If m_ListScripts.Item(i).ID = scriptId Then
                    retour = m_ListScripts.Item(i)
                    Exit For
                End If
            Next
            Return retour
        End Function

        Public Function RunMacro(ByVal macroId As String) As Boolean Implements IeHomeServer.RunMacro
            RunScript(macroId)
        End Function

        Public Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address As String, ByVal image As String, ByVal enable As Boolean, ByVal adapter As String, ByVal typeclass As String) As String Implements IeHomeServer.SaveDevice
            Dim myID As String

            If deviceId = "" Then 'C'est un nouveau device
                myID = eHomeApi.GenerateGUID
                'Suivant chaque type de device
                Select Case typeclass
                    Case "lightingdimmer"
                        Dim o As New Device.Lighting.Dimmer
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        m_ListDevices.Add(o)
                    Case "audiozone"
                        Dim o As New Device.Audio.AudioZone
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        m_ListDevices.Add(o)
                    Case "web"
                        Dim o As New Device.PageWeb
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                        End With
                        m_ListDevices.Add(o)
                    Case "meteo"
                        Dim o As New Device.Meteo
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        m_ListDevices.Add(o)
                    Case "tv"
                        Dim o As New Device.Media.TV
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        m_ListDevices.Add(o)
                    Case "freebox"
                        Dim o As New Device.Media.Freebox
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        m_ListDevices.Add(o)
                    Case "temperature"
                        Dim o As New Device.Temperature
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            'AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        m_ListDevices.Add(o)
                    Case "contact"
                        Dim o As New Device.Contact
                        With o
                            .ID = myID
                            .Name = name
                            .Picture = image
                            .DriverID = adapter
                            .Adresse = address
                            .Enable = enable
                            .Driver = ReturnDriverById(adapter)
                            AddHandler o.DeviceChanged, AddressOf DeviceChange
                        End With
                        m_ListDevices.Add(o)
                End Select

            Else 'Device Existant
                myID = deviceId
                For i As Integer = 0 To m_ListDevices.Count - 1
                    If m_ListDevices.Item(i).ID = deviceId Then
                        m_ListDevices.Item(i).name = name
                        m_ListDevices.Item(i).adresse = address
                        m_ListDevices.Item(i).picture = image
                        m_ListDevices.Item(i).enable = enable
                        m_ListDevices.Item(i).driverid = adapter
                        m_ListDevices.Item(i).Driver = ReturnDriverById(adapter)
                    End If
                Next
            End If

            'génration de l'event
            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
            msg.EventTime = Now
            msg.Reason = IeHomeServer.Reason.INSERT_DEVICE
            msg.Message = myID
            RaiseEvent MessageArrivedHandler(msg)

            Return myID
        End Function

        Public Function SaveDeviceCommand(ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String Implements IeHomeServer.SaveDeviceCommand
            Dim flag As Boolean

            'On vérifie avant que si la commande existe on la modifie
            For i As Integer = 0 To m_ListDevices.Count - 1
                If m_ListDevices.Item(i).id = deviceId Then
                    For j As Integer = 0 To m_ListDevices.Item(i).listcommandName.count - 1
                        If m_ListDevices.Item(i).listcommandname(j) = CmdName Then
                            m_ListDevices.Item(i).listcommanddata(j) = CmdData
                            m_ListDevices.Item(i).listcommandrepeat(j) = CmdRepeat
                            flag = True
                        End If
                    Next
                    'sinon on la crée
                    If flag = False Then
                        m_ListDevices.Item(i).listcommandname.add(CmdName)
                        m_ListDevices.Item(i).listcommanddata.add(CmdData)
                        m_ListDevices.Item(i).listcommandrepeat.add(CmdRepeat)
                    End If
                End If
            Next

            Return 0
        End Function

        Public Function SaveScript(ByVal scriptId As String, ByVal name As String, ByVal enable As Boolean, ByVal listaction As ArrayList) As String Implements IeHomeServer.SaveScript
            Dim myID As String

            If scriptId = "" Then 'C'est un nouveau script
                myID = eHomeApi.GenerateGUID
                Dim o As New Script
                With o
                    .ID = myID
                    .Name = name
                    .Enable = enable
                    .ListAction = listaction
                End With
                m_ListScripts.Add(o)
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Creation du nouveau script " & o.Name)
            Else 'Device Existant
                myID = scriptId
                For i As Integer = 0 To m_ListScripts.Count - 1
                    If m_ListScripts.Item(i).ID = scriptId Then
                        m_ListScripts.Item(i).name = name
                        m_ListScripts.Item(i).enable = enable
                        m_ListScripts.Item(i).ListAction = listaction
                        _Log.AddToLog(Log.TypeLog.INFO, "Server", "Modification du script " & name)
                    End If
                Next
            End If

            'génration de l'event
            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
            msg.EventTime = Now
            msg.Reason = IeHomeServer.Reason.INSERT_MACRO
            msg.Message = myID
            RaiseEvent MessageArrivedHandler(msg)

            Return myID
        End Function

        Public Function SaveSchedule(ByVal scheduleId As String, ByVal name As String, ByVal enable As Boolean, ByVal listscript As ArrayList, ByVal triggertype As Integer, ByVal recurrence As Integer, ByVal startdatetime As String, ByVal jour As String, ByVal asend As Boolean, ByVal enddatetime As String, ByVal sunrisebefore As Boolean, ByVal sunrisebeforetime As String, ByVal sunsetbefore As Boolean, ByVal sunsetbeforetime As String) As String Implements IeHomeServer.SaveSchedule
            Dim myID As String

            If scheduleId = "" Then 'C'est un nouveau schedule
                myID = eHomeApi.GenerateGUID
                Dim o As New Schedule
                With o
                    .ID = myID
                    .Name = name
                    .Enable = enable
                    .ListScript = listscript
                    .TriggerType = triggertype
                    .RecurrencePatternList = recurrence
                    .StartDateTime = startdatetime
                    .Jour(0) = Mid(jour, 1, 1)
                    .Jour(1) = Mid(jour, 2, 1)
                    .Jour(2) = Mid(jour, 3, 1)
                    .Jour(3) = Mid(jour, 4, 1)
                    .Jour(4) = Mid(jour, 5, 1)
                    .Jour(5) = Mid(jour, 6, 1)
                    .Jour(6) = Mid(jour, 7, 1)
                    .AsEnd = asend
                    .EndDateTime = enddatetime
                    .SunRiseBefore = sunrisebefore
                    .SunRiseBeforeTime = sunrisebeforetime
                    .SunSetBefore = sunsetbefore
                    .SunsetBeforeTime = sunsetbeforetime
                End With
                m_ListSchedules.Add(o)
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Creation du nouveau schedule: " & o.Name)
            Else 'Device Existant
                myID = scheduleId
                For i As Integer = 0 To m_ListSchedules.Count - 1
                    If m_ListSchedules.Item(i).ID = scheduleId Then
                        m_ListSchedules.Item(i).name = name
                        m_ListSchedules.Item(i).enable = enable
                        m_ListSchedules.Item(i).listscript = listscript
                        m_ListSchedules.Item(i).TriggerType = triggertype
                        For idx As Integer = 0 To 6
                            m_ListSchedules.Item(i).Jour(idx) = Mid(jour, idx + 1, 1)
                        Next
                        m_ListSchedules.Item(i).RecurrencePatternList = recurrence
                        m_ListSchedules.Item(i).StartDateTime = startdatetime
                        m_ListSchedules.Item(i).AsEnd = asend
                        m_ListSchedules.Item(i).EndDateTime = enddatetime
                        m_ListSchedules.Item(i).SunRiseBefore = sunrisebefore
                        m_ListSchedules.Item(i).SunRiseBeforeTime = sunrisebeforetime
                        m_ListSchedules.Item(i).SunSetBefore = sunsetbefore
                        m_ListSchedules.Item(i).SunSetBeforeTime = sunsetbeforetime
                        _Log.AddToLog(Log.TypeLog.INFO, "Server", "Modification du schedule: " & name)
                    End If
                Next
            End If

            'génration de l'event
            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
            msg.EventTime = Now
            msg.Reason = IeHomeServer.Reason.INSERT_SCHEDULE
            msg.Message = myID
            RaiseEvent MessageArrivedHandler(msg)

            Return myID
        End Function

        Public Function SaveTrigger(ByVal triggerID As String, ByVal name As String, ByVal enable As Boolean, ByVal deviceid As String, ByVal status As String, ByVal condition As String, ByVal value As Object, ByVal listscript As ArrayList) As String Implements IeHomeServer.SaveTrigger
            Dim myID As String

            If triggerID = "" Then 'c'est un nouveau trigger
                myID = eHomeApi.GenerateGUID
                Dim o As New Trigger
                o.Id = myID
                o.Name = name
                o.Enable = enable
                o.DeviceId = deviceid
                o.Status = status
                o.Condition = condition
                o.Value = value
                o.ListScript = listscript
                m_ListTriggers.Add(o)
            Else 'Trigger existant
                myID = triggerID
                For i As Integer = 0 To m_ListTriggers.Count - 1
                    If m_ListTriggers.Item(i).id = triggerID Then
                        m_ListTriggers.Item(i).name = name
                        m_ListTriggers.Item(i).enable = enable
                        m_ListTriggers.Item(i).deviceid = deviceid
                        m_ListTriggers.Item(i).status = status
                        m_ListTriggers.Item(i).condition = condition
                        m_ListTriggers.Item(i).value = value
                        m_ListTriggers.Item(i).listscript = listscript
                    End If
                Next
            End If

            'génration de l'event
            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
            msg.EventTime = Now
            msg.Reason = IeHomeServer.Reason.INSERT_TRIGGER
            msg.Message = myID
            RaiseEvent MessageArrivedHandler(msg)

            Return myID
        End Function

        Public Function SaveZone(ByVal zoneID As String, ByVal name As String, ByVal image As String, ByVal ListDevice As ArrayList) As String Implements IeHomeServer.SaveZone
            Dim myID As String

            If zoneID = "" Then 'C'est un nouvel zone
                myID = eHomeApi.GenerateGUID
                Dim o As New Zone
                With o
                    .ID = myID
                    .Name = name
                    .Image = image
                End With
                m_ListZones.Add(o)
            Else 'Device Existant
                myID = zoneID
                For i As Integer = 0 To m_ListZones.Count - 1
                    If m_ListZones.Item(i).ID = zoneID Then
                        m_ListZones.Item(i).name = name
                        m_ListZones.Item(i).image = image
                        m_ListZones.Item(i).ListDeviceId = ListDevice
                    End If
                Next
            End If

            'génration de l'event
            Dim msg As New eHomeApi.IeHomeServer.AuServerEventMsg
            msg.EventTime = Now
            msg.Reason = IeHomeServer.Reason.INSERT_ZONE
            msg.Message = myID
            RaiseEvent MessageArrivedHandler(msg)

            Return myID
        End Function

        Public Sub SendCommand(ByVal [function] As String, ByVal deviceId As String) Implements IeHomeServer.SendCommand
            Try
                Dim _device As Object = ReturnDeviceById(deviceId)
                Dim _driver As Object = ReturnDriverById(_device.DriverID)

                'On ne traite pas la commande si le driver n'est pas connecté
                If _driver.IsConnect = False Then
                    _Log.AddToLog(Log.TypeLog.MESSAGE, "serveur", "Impossible de traiter la commande le driver n'est pas connecté!")
                    Exit Sub
                End If

                'Arguments de la fonction VB CallByNAme
                'oObject -> Objet contenant la méthode à appeler
                'sName -> Nom (type String) de la méthode à appeler
                'cType -> Type de la méthode (VbGet, VbLet, VbSet, VbMethod)
                'Args() -> Tableau de Variant pour les arguments
                'Valeur de retour : ce que retourne votre fonction
                'CallByName(oObject, sName, cType, Args())
                Dim retour = CallByName(_device, "Command", CallType.Method, [function])
            Catch ex As Exception
                _Log.AddToLog(Log.TypeLog.ERREUR, "Serveur", "Erreur SendCommand: " & ex.Message)
            End Try
        End Sub

        Public Function StartIrLearning() As String Implements IeHomeServer.StartIrLearning
            Dim retour As String = ""
            For i As Integer = 0 To m_ListAdapters.Count - 1
                If m_ListAdapters.Item(i).protocol = "IR" Then
                    Dim x As Drivers.UsbUIRT = m_ListAdapters.Item(i)
                    retour = x.LearnCode()
                    _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Apprentissage IR: " & x.LearnCode())
                End If
            Next
            Return retour
        End Function

        Public Function SaveConfig(ByVal Fichier As String) As String Implements IeHomeServer.SaveConfig
            Try
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde de la config sous le fichier " & Fichier)

                'Copy du fichier de config avant sauvegarde
                Try
                    Dim _file As String = Fichier.Replace(".xml", "")
                    If File.Exists(_file & ".sav") = True Then File.Delete(_file & ".sav")
                    File.Copy(_file & ".xml", _file & ".sav")
                Catch ex As Exception
                    _Log.AddToLog(Log.TypeLog.ERREUR, "Serveur", "Erreur impossible de créer une copie de backup du fichier de config: " & ex.Message)
                End Try

                'Creation du fichier XML
                Dim writer As New XmlTextWriter(Fichier, System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 2

                writer.WriteStartElement("wehome")

                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des paramètres serveur")
                '------------ server
                writer.WriteStartElement("server")
                writer.WriteStartElement("nameconfig")
                writer.WriteString(_NameConfig)
                writer.WriteEndElement()
                writer.WriteStartElement("ipnetwork")
                writer.WriteString(_IP)
                writer.WriteEndElement()
                writer.WriteStartElement("portnetworktcp")
                writer.WriteString(_PortTCP)
                writer.WriteEndElement()
                writer.WriteStartElement("portnetworkudp")
                writer.WriteString(_PortUDP)
                writer.WriteEndElement()
                writer.WriteStartElement("longitude")
                writer.WriteString(_Longitude)
                writer.WriteEndElement()
                writer.WriteStartElement("latitude")
                writer.WriteString(_Latitude)
                writer.WriteEndElement()
                writer.WriteStartElement("heurecorrectionlever")
                writer.WriteString(HeureCorrectionLever)
                writer.WriteEndElement()
                writer.WriteStartElement("heurecorrectioncoucher")
                writer.WriteString(HeureCorrectionCoucher)
                writer.WriteEndElement()
                writer.WriteStartElement("logfile")
                writer.WriteString(_Log.Fichier)
                writer.WriteEndElement()
                writer.WriteStartElement("logmaxsize")
                writer.WriteString(_Log.MaxFileSize)
                writer.WriteEndElement()
                writer.WriteStartElement("histmaxday")
                writer.WriteString(HistorisationDayMax)
                writer.WriteEndElement()
                writer.WriteEndElement()

                '-------------------
                '------------service
                '------------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des services")
                For i As Integer = 0 To m_ListAdapters.Count - 1
                    If m_ListAdapters.Item(i).PluginName <> "server" Then
                        writer.WriteStartElement("service")
                        writer.WriteStartElement("id")
                        writer.WriteString(m_ListAdapters.Item(i).ID)
                        writer.WriteEndElement()
                        writer.WriteStartElement("name")
                        writer.WriteString(m_ListAdapters.Item(i).PluginName)
                        writer.WriteEndElement()
                        writer.WriteStartElement("protocol")
                        writer.WriteString(m_ListAdapters.Item(i).Protocol)
                        writer.WriteEndElement()
                        writer.WriteStartElement("port")
                        writer.WriteString(m_ListAdapters.Item(i).Protocol)
                        writer.WriteEndElement()
                        writer.WriteStartElement("startauto")
                        writer.WriteString(m_ListAdapters.Item(i).AutoStart)
                        writer.WriteEndElement()
                        writer.WriteStartElement("parametres")
                        writer.WriteString(m_ListAdapters.Item(i).Parametres)
                        writer.WriteEndElement()
                        writer.WriteEndElement()
                    End If
                Next

                '------------
                'Sauvegarde des devices
                '------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des devices")
                writer.WriteStartElement("devices")
                For i As Integer = 0 To m_ListDevices.Count - 1
                    writer.WriteStartElement("device")
                    writer.WriteStartElement("typeclass")
                    writer.WriteString(m_ListDevices.Item(i).TypeClass)
                    writer.WriteEndElement()
                    writer.WriteStartElement("id")
                    writer.WriteString(m_ListDevices.Item(i).ID)
                    writer.WriteEndElement()
                    writer.WriteStartElement("name")
                    writer.WriteString(m_ListDevices.Item(i).Name)
                    writer.WriteEndElement()
                    writer.WriteStartElement("adresse")
                    writer.WriteString(m_ListDevices.Item(i).Adresse)
                    writer.WriteEndElement()
                    writer.WriteStartElement("enable")
                    writer.WriteString(m_ListDevices.Item(i).Enable)
                    writer.WriteEndElement()
                    writer.WriteStartElement("driver")
                    writer.WriteString(m_ListDevices.Item(i).DriverID)
                    writer.WriteEndElement()
                    writer.WriteStartElement("image")
                    writer.WriteString(m_ListDevices.Item(i).Picture)
                    writer.WriteEndElement()
                    Select Case m_ListDevices.Item(i).TypeClass
                        Case "tv"
                            writer.WriteStartElement("commands")
                            For k As Integer = 0 To m_ListDevices.Item(i).ListCommandName.Count - 1
                                writer.WriteStartElement("command")
                                writer.WriteStartAttribute("key")
                                writer.WriteValue(m_ListDevices.Item(i).ListCommandName(k))
                                writer.WriteEndAttribute()
                                writer.WriteStartAttribute("data")
                                writer.WriteValue(m_ListDevices.Item(i).ListCommandData(k))
                                writer.WriteEndAttribute()
                                writer.WriteStartAttribute("repeat")
                                writer.WriteValue(m_ListDevices.Item(i).ListCommandRepeat(k))
                                writer.WriteEndAttribute()
                                writer.WriteEndElement()
                            Next
                            writer.WriteEndElement()
                    End Select
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                '------------

                '------------
                'Sauvegarde des zones
                '------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des zones")
                writer.WriteStartElement("zones")
                For i As Integer = 0 To m_ListZones.Count - 1
                    writer.WriteStartElement("zone")
                    writer.WriteStartElement("id")
                    writer.WriteString(m_ListZones.Item(i).ID)
                    writer.WriteEndElement()
                    writer.WriteStartElement("name")
                    writer.WriteString(m_ListZones.Item(i).Name)
                    writer.WriteEndElement()
                    writer.WriteStartElement("image")
                    writer.WriteString(m_ListZones.Item(i).image)
                    writer.WriteEndElement()
                    For j As Integer = 0 To m_ListZones.Item(i).listdeviceid.count - 1
                        writer.WriteStartElement("deviceid")
                        writer.WriteString(m_ListZones.Item(i).listdeviceid.item(j))
                        writer.WriteEndElement()
                    Next
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                '------------

                '------------
                'Sauvegarde des historisations
                '------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des historisations")
                writer.WriteStartElement("histos")
                For i As Integer = 0 To m_ListHistorisations.Count - 1
                    writer.WriteStartElement("histo")
                    writer.WriteStartElement("id")
                    writer.WriteString(m_ListHistorisations.Item(i).ID)
                    writer.WriteEndElement()
                    writer.WriteStartElement("enable")
                    writer.WriteString(m_ListHistorisations.Item(i).Enable)
                    writer.WriteEndElement()
                    writer.WriteStartElement("deviceid")
                    writer.WriteString(m_ListHistorisations.Item(i).deviceid)
                    writer.WriteEndElement()
                    writer.WriteStartElement("propertydevice")
                    writer.WriteString(m_ListHistorisations.Item(i).PropertyDevice)
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                '------------

                '------------
                'Sauvegarde des variables
                '------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des variables")
                writer.WriteStartElement("variables")
                For i As Integer = 0 To m_listVariables.Count - 1
                    writer.WriteStartElement("variable")
                    writer.WriteStartElement("id")
                    writer.WriteString(m_listVariables.Item(i).ID)
                    writer.WriteEndElement()
                    writer.WriteStartElement("name")
                    writer.WriteString(m_listVariables.Item(i).name)
                    writer.WriteEndElement()
                    writer.WriteStartElement("type")
                    writer.WriteString(m_listVariables.Item(i).type)
                    writer.WriteEndElement()
                    writer.WriteStartElement("value")
                    writer.WriteString(m_listVariables.Item(i).value)
                    writer.WriteEndElement()
                    writer.WriteStartElement("defaultvalue")
                    writer.WriteString(m_listVariables.Item(i).defaultvalue)
                    writer.WriteEndElement()
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                '------------

                '------------
                'Sauvegarde des schedules
                '------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des schedules")
                writer.WriteStartElement("schedules")
                If m_ListSchedules.Count > 0 Then
                    For i As Integer = 0 To m_ListSchedules.Count - 1
                        writer.WriteStartElement("schedule")
                        writer.WriteStartElement("id")
                        writer.WriteString(m_ListSchedules.Item(i).id)
                        writer.WriteEndElement()
                        writer.WriteStartElement("name")
                        writer.WriteString(m_ListSchedules.Item(i).name)
                        writer.WriteEndElement()
                        writer.WriteStartElement("enable")
                        writer.WriteString(m_ListSchedules.Item(i).enable)
                        writer.WriteEndElement()
                        writer.WriteStartElement("triggertype")
                        writer.WriteString(m_ListSchedules.Item(i).triggertype)
                        writer.WriteEndElement()
                        writer.WriteStartElement("recurrence")
                        writer.WriteString(m_ListSchedules.Item(i).RecurrencePatternList)
                        writer.WriteEndElement()
                        writer.WriteStartElement("startdatetime")
                        writer.WriteString(m_ListSchedules.Item(i).startdatetime)
                        writer.WriteEndElement()
                        writer.WriteStartElement("jour")
                        Dim a As String = ""
                        For idx As Integer = 0 To 6
                            a &= CByte(m_ListSchedules.Item(i).jour(idx))
                        Next
                        writer.WriteString(a)
                        writer.WriteEndElement()
                        writer.WriteStartElement("asend")
                        writer.WriteString(m_ListSchedules.Item(i).asend)
                        writer.WriteEndElement()
                        writer.WriteStartElement("enddatetime")
                        writer.WriteString(m_ListSchedules.Item(i).enddatetime)
                        writer.WriteEndElement()
                        writer.WriteStartElement("sunrisebefore")
                        writer.WriteString(m_ListSchedules.Item(i).sunrisebefore)
                        writer.WriteEndElement()
                        writer.WriteStartElement("sunrisebeforetime")
                        writer.WriteString(m_ListSchedules.Item(i).sunrisebeforetime)
                        writer.WriteEndElement()
                        writer.WriteStartElement("sunsetbefore")
                        writer.WriteString(m_ListSchedules.Item(i).sunsetbefore)
                        writer.WriteEndElement()
                        writer.WriteStartElement("sunsetbeforetime")
                        writer.WriteString(m_ListSchedules.Item(i).sunsetbeforetime)
                        writer.WriteEndElement()
                        writer.WriteStartElement("lastschedule")
                        writer.WriteString(m_ListSchedules.Item(i).lastschedule)
                        writer.WriteEndElement()
                        writer.WriteStartElement("scripts")
                        For j As Integer = 0 To m_ListSchedules.Item(i).listscript.count - 1
                            writer.WriteStartElement("scriptid")
                            writer.WriteString(m_ListSchedules.Item(i).listscript.Item(j))
                            writer.WriteEndElement()
                        Next
                        writer.WriteEndElement()
                        writer.WriteEndElement()
                    Next
                End If
                writer.WriteEndElement()
                '------------

                '------------
                'Sauvegarde des triggers
                '------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des triggers")
                writer.WriteStartElement("triggers")
                If m_ListSchedules.Count > 0 Then
                    For i As Integer = 0 To m_ListTriggers.Count - 1
                        writer.WriteStartElement("trigger")
                        writer.WriteStartElement("id")
                        writer.WriteString(m_ListTriggers.Item(i).id)
                        writer.WriteEndElement()
                        writer.WriteStartElement("name")
                        writer.WriteString(m_ListTriggers.Item(i).name)
                        writer.WriteEndElement()
                        writer.WriteStartElement("enable")
                        writer.WriteString(m_ListTriggers.Item(i).enable)
                        writer.WriteEndElement()
                        writer.WriteStartElement("deviceid")
                        writer.WriteString(m_ListTriggers.Item(i).deviceid)
                        writer.WriteEndElement()
                        writer.WriteStartElement("status")
                        writer.WriteString(m_ListTriggers.Item(i).status)
                        writer.WriteEndElement()
                        writer.WriteStartElement("condition")
                        writer.WriteString(m_ListTriggers.Item(i).condition)
                        writer.WriteEndElement()
                        writer.WriteStartElement("value")
                        writer.WriteString(m_ListTriggers.Item(i).value)
                        writer.WriteEndElement()
                        writer.WriteStartElement("scripts")
                        For j As Integer = 0 To m_ListTriggers.Item(i).listscript.count - 1
                            writer.WriteStartElement("scriptid")
                            writer.WriteString(m_ListTriggers.Item(i).listscript.Item(j))
                            writer.WriteEndElement()
                        Next
                        writer.WriteEndElement()
                        writer.WriteEndElement()
                    Next
                End If
                writer.WriteEndElement()
                '------------

                '------------
                'Sauvegarde des scripts
                '------------
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde des scripts")
                writer.WriteStartElement("scripts")
                If m_ListSchedules.Count > 0 Then
                    For i As Integer = 0 To m_ListScripts.Count - 1
                        writer.WriteStartElement("script")
                        writer.WriteStartElement("id")
                        writer.WriteString(m_ListScripts.Item(i).id)
                        writer.WriteEndElement()
                        writer.WriteStartElement("name")
                        writer.WriteString(m_ListScripts.Item(i).name)
                        writer.WriteEndElement()
                        writer.WriteStartElement("enable")
                        writer.WriteString(m_ListScripts.Item(i).enable)
                        writer.WriteEndElement()
                        For j As Integer = 0 To m_ListScripts.Item(i).listaction.count - 1
                            writer.WriteStartElement("action")

                            writer.WriteStartElement("typeaction")
                            writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).TypeClass)
                            writer.WriteEndElement()
                            Select Case m_ListScripts.Item(i).listaction.Item(j).TypeClass
                                Case "DEVICE"
                                    writer.WriteStartElement("deviceid")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).iddevice)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("function")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).devicefunction)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("value")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).value)
                                    writer.WriteEndElement()
                                Case "SCRIPT"
                                    writer.WriteStartElement("scriptid")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).iddevice)
                                    writer.WriteEndElement()
                                Case "MAIL"
                                    writer.WriteStartElement("smtp")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).MailServerSMTP)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("identification")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).MailServerIdentification)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("login")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).MailServerLogin)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("password")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).MailServerPassword)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("from")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).From)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("to")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).A)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("sujet")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).Sujet)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("message")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).Message)
                                    writer.WriteEndElement()
                                Case "EXIT"

                                Case "PAUSE"
                                    writer.WriteStartElement("heure")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).Heure)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("minute")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).Minute)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("seconde")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).Seconde)
                                    writer.WriteEndElement()
                                    writer.WriteStartElement("mseconde")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).MilliSeconde)
                                    writer.WriteEndElement()
                                Case "SCRIPTVB"
                                    writer.WriteStartElement("script")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).Script)
                                    writer.WriteEndElement()
                                Case "PARLER"
                                    writer.WriteStartElement("parler")
                                    writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).message)
                                    writer.WriteEndElement()
                                Case "IF" 'pas développé pour l'instant
                                    'CONDITION
                                    For k As Integer = 0 To m_ListScripts.Item(i).listaction.Item(j).listcondition.count - 1
                                        writer.WriteStartElement("condition")
                                        writer.WriteStartElement("typecondition")
                                        writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).listcondition.item(k).TypeCondition)
                                        writer.WriteEndElement()
                                        writer.WriteStartElement("itemid")
                                        writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).listcondition.item(k).ItemId)
                                        writer.WriteEndElement()
                                        writer.WriteStartElement("parametre")
                                        writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).listcondition.item(k).Parametre)
                                        writer.WriteEndElement()
                                        writer.WriteStartElement("value")
                                        writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).listcondition.item(k).Value)
                                        writer.WriteEndElement()
                                        writer.WriteStartElement("operateur")
                                        writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).listcondition.item(k).Operateur)
                                        writer.WriteEndElement()
                                        writer.WriteEndElement()
                                    Next
                                    'THEN
                                    writer.WriteStartElement("then")
                                    For k As Integer = 0 To m_ListScripts.Item(i).listaction.Item(j).ThenListAction.count - 1
                                        writer.WriteStartElement("action")

                                        writer.WriteStartElement("typeaction")
                                        writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).TypeClass)
                                        writer.WriteEndElement()
                                        Select Case m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).TypeClass
                                            Case "DEVICE"
                                                writer.WriteStartElement("deviceid")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).iddevice)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("function")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).devicefunction)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("value")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).value)
                                                writer.WriteEndElement()
                                            Case "MAIL"
                                                writer.WriteStartElement("smtp")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).MailServerSMTP)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("identification")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).MailServerIdentification)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("login")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).MailServerLogin)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("password")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).MailServerPassword)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("from")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).From)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("to")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).A)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("sujet")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).Sujet)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("message")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).Message)
                                                writer.WriteEndElement()
                                            Case "EXIT"

                                            Case "PAUSE"
                                                writer.WriteStartElement("heure")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).Heure)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("minute")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).Minute)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("seconde")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).Seconde)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("mseconde")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).MilliSeconde)
                                                writer.WriteEndElement()
                                            Case "SCRIPTVB"
                                                writer.WriteStartElement("script")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).Script)
                                                writer.WriteEndElement()
                                            Case "PARLER"
                                                writer.WriteStartElement("parler")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ThenListAction.Item(k).message)
                                                writer.WriteEndElement()
                                        End Select

                                        writer.WriteEndElement()
                                    Next
                                    writer.WriteEndElement()
                                    'ELSE
                                    writer.WriteStartElement("else")
                                    For k As Integer = 0 To m_ListScripts.Item(i).listaction.Item(j).ElseListAction.count - 1
                                        writer.WriteStartElement("action")

                                        writer.WriteStartElement("typeaction")
                                        writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).TypeClass)
                                        writer.WriteEndElement()
                                        Select Case m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).TypeClass
                                            Case "DEVICE"
                                                writer.WriteStartElement("deviceid")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).iddevice)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("function")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).devicefunction)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("value")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).value)
                                                writer.WriteEndElement()
                                            Case "MAIL"
                                                writer.WriteStartElement("smtp")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).MailServerSMTP)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("identification")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).MailServerIdentification)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("login")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).MailServerLogin)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("password")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).MailServerPassword)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("from")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).From)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("to")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).A)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("sujet")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).Sujet)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("message")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).Message)
                                                writer.WriteEndElement()
                                            Case "EXIT"

                                            Case "PAUSE"
                                                writer.WriteStartElement("heure")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).Heure)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("minute")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).Minute)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("seconde")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).Seconde)
                                                writer.WriteEndElement()
                                                writer.WriteStartElement("mseconde")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).MilliSeconde)
                                                writer.WriteEndElement()
                                            Case "SCRIPTVB"
                                                writer.WriteStartElement("script")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).Script)
                                                writer.WriteEndElement()
                                            Case "PARLER"
                                                writer.WriteStartElement("parler")
                                                writer.WriteString(m_ListScripts.Item(i).listaction.Item(j).ElseListAction.Item(k).message)
                                                writer.WriteEndElement()
                                        End Select

                                        writer.WriteEndElement()
                                    Next
                                    writer.WriteEndElement()
                            End Select

                            writer.WriteEndElement()
                        Next
                        writer.WriteEndElement()
                    Next
                End If
                writer.WriteEndElement()
                '------------

                writer.WriteEndDocument()
                writer.Close()
                _Log.AddToLog(Log.TypeLog.INFO, "Server", "Sauvegarde terminée")
                Return ""
            Catch ex As Exception
                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", " Erreur de sauvegarde de la configuration: " & ex.Message)
                Return " Erreur de sauvegarde de la configuration: " & ex.Message
            End Try
        End Function
#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub
    End Class

End Namespace

