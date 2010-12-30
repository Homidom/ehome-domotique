﻿Imports System.IO
Imports System.Xml
Imports System.Threading
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Net
Imports eHomeApi.eHomeApi

Namespace eHomeApi

    Public Class Device
        Inherits Server
        '**************************************************************************************************************************
        'DEVICES
        '**************************************************************************************************************************

#Region "Alarm"
        Class Alarm



        End Class
#End Region

#Region "Lighting"
        Public Class Lighting

#Region "Switch"
            <Serializable()> Public Class Switch
                Dim _ID As String
                Dim _Name As String
                Dim _Picture As String
                Dim _DriverID As String
                Dim _Adresse As String
                Dim _Enable As Boolean
                Dim mClass As Integer = "lightingswitch"
                Dim _Status As Boolean

                Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As Object)

                Public Property ID() As String
                    Get
                        Return _ID
                    End Get
                    Set(ByVal Value As String)
                        _ID = Value
                    End Set
                End Property

                Public Property Name() As String
                    Get
                        Return _Name
                    End Get
                    Set(ByVal Value As String)
                        _Name = Value
                    End Set
                End Property

                Public Property Picture() As String
                    Get
                        Return _Picture
                    End Get
                    Set(ByVal Value As String)
                        _Picture = Value
                    End Set
                End Property

                Public Property DriverID() As String
                    Get
                        Return _DriverID
                    End Get
                    Set(ByVal Value As String)
                        _DriverID = Value
                    End Set
                End Property

                Public Property Adresse() As String
                    Get
                        Return _Adresse
                    End Get
                    Set(ByVal Value As String)
                        _Adresse = Value
                    End Set
                End Property

                Public Property Enable() As Boolean
                    Get
                        Return _Enable
                    End Get
                    Set(ByVal Value As Boolean)
                        _Enable = Value
                    End Set
                End Property

                Public ReadOnly Property TypeClass() As Integer
                    Get
                        Return mClass
                    End Get
                End Property

                Public Property Status() As Boolean
                    Get
                        Return _Status
                    End Get
                    Set(ByVal value As Boolean)
                        _Status = value
                        RaiseEvent DeviceChanged(_ID, "Status", value)
                    End Set
                End Property

                Public Function GetStatus() As Boolean
                    Return _Status
                End Function

                Public Sub [On]()
                    Status = True
                End Sub

                Public Sub Off()
                    Status = False
                End Sub

                Public Sub New()
                End Sub
            End Class
#End Region

#Region "Dimmer"
            <Serializable()> Public Class Dimmer
                Dim _ID As String
                Dim _Name As String
                Dim _Picture As String
                Dim _DriverID As String
                Dim _Adresse As String
                Dim _Enable As Boolean

                Dim _Status As Boolean
                Dim _Level As Byte

                Dim mClass As String = "lightingdimmer"

                Public Driver As New Object
                Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As Object)

                Public Property ID() As String
                    Get
                        Return _ID
                    End Get
                    Set(ByVal Value As String)
                        _ID = Value
                    End Set
                End Property

                Public Property Name() As String
                    Get
                        Return _Name
                    End Get
                    Set(ByVal Value As String)
                        _Name = Value
                    End Set
                End Property

                Public Property Picture() As String
                    Get
                        Return _Picture
                    End Get
                    Set(ByVal Value As String)
                        _Picture = Value
                    End Set
                End Property

                Public Property DriverID() As String
                    Get
                        Return _DriverID
                    End Get
                    Set(ByVal Value As String)
                        _DriverID = Value
                    End Set
                End Property

                Public Property Adresse() As String
                    Get
                        Return _Adresse
                    End Get
                    Set(ByVal Value As String)
                        _Adresse = Value
                    End Set
                End Property

                Public Property Enable() As Boolean
                    Get
                        Return _Enable
                    End Get
                    Set(ByVal Value As Boolean)
                        _Enable = Value
                    End Set
                End Property

                Public ReadOnly Property TypeClass() As String
                    Get
                        Return mClass
                    End Get
                End Property

                Public Property Status() As Boolean
                    Get
                        Return _Status
                    End Get
                    Set(ByVal value As Boolean)
                        _Status = value
                        RaiseEvent DeviceChanged(_ID, "Status", value)
                    End Set
                End Property

                Public Function GetLevel() As Byte 'return entre 0 et 100 %
                    Return _Level
                End Function

                Public Function GetStatus() As Boolean
                    Return _Status
                End Function

                Public Sub [On]()

                    Status = True
                    _Level = 100

                End Sub

                Public Sub [Off]()
                    Status = False
                    _Level = 0

                End Sub

                Public Sub [Dim](ByVal Level As Byte)
                    If Level < 0 Then Level = 0
                    If Level > 100 Then Level = 100


                End Sub

                Public Sub [Bright](ByVal Level As Byte)
                    If Level < 0 Then Level = 0
                    If Level > 100 Then Level = 100
                End Sub

            End Class
#End Region
        End Class
#End Region

#Region "Media"
        Public Class Media

#Region "TV"
            <Serializable()> Public Class TV
                Dim _ID As String
                Dim _Name As String
                Dim _Picture As String
                Dim _DriverID As String
                Dim _Adresse As String
                Dim _Enable As Boolean
                Dim mClass As String = "tv"
                Public Driver As New Object

                Public ListCommandName As New ArrayList
                Public ListCommandData As New ArrayList
                Public ListCommandRepeat As New ArrayList

                Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As Object)

                Public Property ID() As String
                    Get
                        Return _ID
                    End Get
                    Set(ByVal Value As String)
                        _ID = Value
                    End Set
                End Property

                Public Property Name() As String
                    Get
                        Return _Name
                    End Get
                    Set(ByVal Value As String)
                        _Name = Value
                    End Set
                End Property

                Public Property Picture() As String
                    Get
                        Return _Picture
                    End Get
                    Set(ByVal Value As String)
                        _Picture = Value
                    End Set
                End Property

                Public Property DriverID() As String
                    Get
                        Return _DriverID
                    End Get
                    Set(ByVal Value As String)
                        _DriverID = Value
                    End Set
                End Property

                Public Property Adresse() As String
                    Get
                        Return _Adresse
                    End Get
                    Set(ByVal Value As String)
                        _Adresse = Value
                    End Set
                End Property

                Public Property Enable() As Boolean
                    Get
                        Return _Enable
                    End Get
                    Set(ByVal Value As Boolean)
                        _Enable = Value
                    End Set
                End Property

                Public ReadOnly Property TypeClass() As String
                    Get
                        Return mClass
                    End Get
                End Property

                Public Sub ChannelDown()
                End Sub

                Public Sub ChannelUp()
                End Sub

                Public Function GetChannel() As Integer

                End Function

                Public Sub SetChannel(ByVal channel As Integer)
                End Sub

                Public Sub Power()
                    For i As Integer = 0 To ListCommandName.Count - 1
                        If ListCommandName(i) = "Power" Then
                            Driver.SendCode(ListCommandData(i), ListCommandRepeat(i))
                        End If
                    Next
                End Sub

                Public Sub Command(ByVal NameCommand As String)
                    For i As Integer = 0 To ListCommandName.Count - 1
                        If ListCommandName(i) = NameCommand Then
                            Driver.SendCode(ListCommandData(i), ListCommandRepeat(i))
                        End If
                    Next
                End Sub

                Public Sub VolumeUp()
                End Sub

                Public Sub VolumeDown()
                End Sub

                Public Sub Mute()
                End Sub

                Public Sub Source()
                End Sub

                Public Sub SendKey(ByVal key As Byte)

                End Sub

                Public Sub New()
                    ListCommandName.Add("Power")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("ChannelUp")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("ChannelDown")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("VolumeUp")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("VolumeDown")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("Mute")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("Source")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("0")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("1")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("2")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("3")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("4")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("5")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("6")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("7")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("8")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")
                    ListCommandName.Add("9")
                    ListCommandData.Add("0")
                    ListCommandRepeat.Add("0")

                End Sub
            End Class

#End Region
#Region "Freebox"
            <Serializable()> Public Class Freebox
                Dim _ID As String
                Dim _Name As String
                Dim _Picture As String
                Dim _DriverID As String
                Dim _Adresse As String
                Dim _Enable As Boolean
                Dim mClass As String = "freebox"
                Dim _Status As String = "?"
                Dim _File As String

                Public Driver As New Object

                Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As Object)

                Public Property ID() As String
                    Get
                        Return _ID
                    End Get
                    Set(ByVal Value As String)
                        _ID = Value
                    End Set
                End Property

                Public Property Name() As String
                    Get
                        Return _Name
                    End Get
                    Set(ByVal Value As String)
                        _Name = Value
                    End Set
                End Property

                Public Property Picture() As String
                    Get
                        Return _Picture
                    End Get
                    Set(ByVal Value As String)
                        _Picture = Value
                    End Set
                End Property

                Public Property DriverID() As String
                    Get
                        Return _DriverID
                    End Get
                    Set(ByVal Value As String)
                        _DriverID = Value
                    End Set
                End Property

                Public Property Adresse() As String
                    Get
                        Return _Adresse
                    End Get
                    Set(ByVal Value As String)
                        _Adresse = Value
                        If File.Exists(Value) = False Then
                            _Log.AddToLog(Log.TypeLog.ERREUR, "DEVICE AUDIO", "Le fichier foobar n'existe pas: " & Value)
                        End If
                    End Set
                End Property

                Public Property Enable() As Boolean
                    Get
                        Return _Enable
                    End Get
                    Set(ByVal Value As Boolean)
                        _Enable = Value
                    End Set
                End Property

                Public ReadOnly Property TypeClass() As String
                    Get
                        Return mClass
                    End Get
                End Property

                Public Property Status() As String
                    Get
                        Return _Status
                    End Get
                    Set(ByVal value As String)
                        _Status = value
                        RaiseEvent DeviceChanged(_ID, "Status", value)
                    End Set
                End Property

                Private Function Sendhttp(ByVal cmd As String) As String
                    Dim URL As String = " http://hd1.freebox.fr/pub/remote_control ?key=" & cmd
                    Dim request As WebRequest = WebRequest.Create(URL)
                    Dim response As WebResponse = request.GetResponse()
                    Dim reader As StreamReader = New StreamReader(response.GetResponseStream())
                    Dim str As String = reader.ReadToEnd
                    'Do While str.Length > 0
                    '    Console.WriteLine(str)
                    '    str = reader.ReadLine()
                    'Loop
                    reader.Close()
                    Return str
                End Function

                Public Sub Touche0()
                    Try
                        Dim retour As String
                        retour = Sendhttp("0")
                        Status = "0"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche0: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche1()
                    Try
                        Dim retour As String
                        retour = Sendhttp("1")
                        Status = "1"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche1: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche2()
                    Try
                        Dim retour As String
                        retour = Sendhttp("2")
                        Status = "2"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche2: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche3()
                    Try
                        Dim retour As String
                        retour = Sendhttp("3")
                        Status = "3"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche3: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche4()
                    Try
                        Dim retour As String
                        retour = Sendhttp("4")
                        Status = "4"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche4: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche5()
                    Try
                        Dim retour As String
                        retour = Sendhttp("5")
                        Status = "5"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche5: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche6()
                    Try
                        Dim retour As String
                        retour = Sendhttp("6")
                        Status = "6"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche6: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche7()
                    Try
                        Dim retour As String
                        retour = Sendhttp("7")
                        Status = "7"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche7: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche8()
                    Try
                        Dim retour As String
                        retour = Sendhttp("8")
                        Status = "8"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche8: " & ex.Message)
                    End Try
                End Sub

                Public Sub Touche9()
                    Try
                        Dim retour As String
                        retour = Sendhttp("9")
                        Status = "9"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " Touche9: " & ex.Message)
                    End Try
                End Sub

                Public Sub VolumeUp()
                    Try
                        Dim retour As String
                        retour = Sendhttp("vol_inc")
                        Status = "vol_inc"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VolumeUp: " & ex.Message)
                    End Try
                End Sub

                Public Sub VolumeDown()
                    Try
                        Dim retour As String
                        retour = Sendhttp("vol_dec")
                        Status = "vol_dec"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VolumeDown: " & ex.Message)
                    End Try
                End Sub

                Public Sub OK()
                    Try
                        Dim retour As String
                        retour = Sendhttp("ok")
                        Status = "ok"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " OK: " & ex.Message)
                    End Try
                End Sub

                Public Sub HAUT()
                    Try
                        Dim retour As String
                        retour = Sendhttp("up")
                        Status = "up"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " HAUT: " & ex.Message)
                    End Try
                End Sub

                Public Sub BAS()
                    Try
                        Dim retour As String
                        retour = Sendhttp("down")
                        Status = "down"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " BAS: " & ex.Message)
                    End Try
                End Sub

                Public Sub GAUCHE()
                    Try
                        Dim retour As String
                        retour = Sendhttp("left")
                        Status = "left"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " GAUCHE: " & ex.Message)
                    End Try
                End Sub

                Public Sub DROITE()
                    Try
                        Dim retour As String
                        retour = Sendhttp("right")
                        Status = "right"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " DROITE: " & ex.Message)
                    End Try
                End Sub

                Public Sub MUTE()
                    Try
                        Dim retour As String
                        retour = Sendhttp("mute")
                        Status = "mute"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " MUTE: " & ex.Message)
                    End Try
                End Sub

                Public Sub HOME()
                    Try
                        Dim retour As String
                        retour = Sendhttp("home")
                        Status = "home"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " HOME: " & ex.Message)
                    End Try
                End Sub

                Public Sub ENREGISTRER()
                    Try
                        Dim retour As String
                        retour = Sendhttp("rec")
                        Status = "rec"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " ENREGISTRER: " & ex.Message)
                    End Try
                End Sub

                Public Sub RETOUR()
                    Try
                        Dim retour As String
                        retour = Sendhttp("bwd")
                        Status = "bwd"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " RETOUR: " & ex.Message)
                    End Try
                End Sub

                Public Sub PRECEDENT()
                    Try
                        Dim retour As String
                        retour = Sendhttp("prev")
                        Status = "prev"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PRECEDENT: " & ex.Message)
                    End Try
                End Sub

                Public Sub PLAY()
                    Try
                        Dim retour As String
                        retour = Sendhttp("play")
                        Status = "play"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PLAY: " & ex.Message)
                    End Try
                End Sub

                Public Sub AVANCE()
                    Try
                        Dim retour As String
                        retour = Sendhttp("fwd")
                        Status = "fwd"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " AVANCE: " & ex.Message)
                    End Try
                End Sub

                Public Sub SUIVANT()
                    Try
                        Dim retour As String
                        retour = Sendhttp("next")
                        Status = "next"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " SUIVANT: " & ex.Message)
                    End Try
                End Sub

                Public Sub BoutonROUGE()
                    Try
                        Dim retour As String
                        retour = Sendhttp("red")
                        Status = "red"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " BoutonROUGE: " & ex.Message)
                    End Try
                End Sub

                Public Sub BoutonVERT()
                    Try
                        Dim retour As String
                        retour = Sendhttp("green")
                        Status = "green"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " BoutonVERT: " & ex.Message)
                    End Try
                End Sub

                Public Sub BoutonJAUNE()
                    Try
                        Dim retour As String
                        retour = Sendhttp("yellow")
                        Status = "yellow"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " BoutonJAUNE: " & ex.Message)
                    End Try
                End Sub

                Public Sub BoutonBLEU()
                    Try
                        Dim retour As String
                        retour = Sendhttp("blue")
                        Status = "blue"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " BoutonBLEU: " & ex.Message)
                    End Try
                End Sub
            End Class
#End Region
        End Class

#End Region

#Region "Meteo"
        <Serializable()> Public Class Meteo
            Dim _ID As String
            Dim _Name As String
            Dim _Picture As String
            Dim _DriverID As String
            Dim _Adresse As String
            Dim _Enable As Boolean
            Dim mClass As String = "meteo"

            Dim x As New Timers.Timer

            Dim _ConditionActuel As String = ""
            Public Property ConditionActuel() As String
                Get
                    Return _ConditionActuel
                End Get
                Set(ByVal value As String)
                    _ConditionActuel = value
                    RaiseEvent DeviceChanged(_ID, "ConditionActuel", value)
                End Set
            End Property

            Dim _TempActuel As String = ""
            Public Property TemperatureActuel() As String
                Get
                    Return _TempActuel
                End Get
                Set(ByVal value As String)
                    _TempActuel = value
                    RaiseEvent DeviceChanged(_ID, "TemperatureActuel", value)
                End Set
            End Property

            Dim _HumActuel As String = ""
            Public Property HumiditeActuel() As String
                Get
                    Return _HumActuel
                End Get
                Set(ByVal value As String)
                    _HumActuel = value
                    RaiseEvent DeviceChanged(_ID, "HumiditeActuel", value)
                End Set
            End Property

            Dim _IconActuel As String = ""
            Public Property IconActuel() As String
                Get
                    Return _IconActuel
                End Get
                Set(ByVal value As String)
                    _IconActuel = value
                End Set
            End Property

            Dim _VentActuel As String = ""
            Public Property VentActuel() As String
                Get
                    Return _VentActuel
                End Get
                Set(ByVal value As String)
                    _VentActuel = value
                    RaiseEvent DeviceChanged(_ID, "VentActuel", value)
                End Set
            End Property

            Dim _JourToday As String = ""
            Public Property JourToday() As String
                Get
                    Return _JourToday
                End Get
                Set(ByVal value As String)
                    _JourToday = value
                    RaiseEvent DeviceChanged(_ID, "JourToday", value)
                End Set
            End Property

            Dim _MinToday As String = ""
            Public Property MinToday() As String
                Get
                    Return _MinToday
                End Get
                Set(ByVal value As String)
                    _MinToday = value
                    RaiseEvent DeviceChanged(_ID, "MinToday", value)
                End Set
            End Property

            Dim _MaxToday As String = ""
            Public Property MaxToday() As String
                Get
                    Return _MaxToday
                End Get
                Set(ByVal value As String)
                    _MaxToday = value
                    RaiseEvent DeviceChanged(_ID, "MaxToday", value)
                End Set
            End Property

            Dim _IconToday As String = ""
            Public Property IconToday() As String
                Get
                    Return _IconToday
                End Get
                Set(ByVal value As String)
                    _IconToday = value
                End Set
            End Property

            Dim _ConditionToday As String = ""
            Public Property ConditionToday() As String
                Get
                    Return _ConditionToday
                End Get
                Set(ByVal value As String)
                    _ConditionToday = value
                    RaiseEvent DeviceChanged(_ID, "ConditionToday", value)
                End Set
            End Property

            Dim _JourJ1 As String = ""
            Public Property JourJ1() As String
                Get
                    Return _JourJ1
                End Get
                Set(ByVal value As String)
                    _JourJ1 = value
                    RaiseEvent DeviceChanged(_ID, "JourJ1", value)
                End Set
            End Property

            Dim _MinJ1 As String = ""
            Public Property MinJ1() As String
                Get
                    Return _MinJ1
                End Get
                Set(ByVal value As String)
                    _MinJ1 = value
                    RaiseEvent DeviceChanged(_ID, "MinJ1", value)
                End Set
            End Property

            Dim _MaxJ1 As String = ""
            Public Property MaxJ1() As String
                Get
                    Return _MaxJ1
                End Get
                Set(ByVal value As String)
                    _MaxJ1 = value
                    RaiseEvent DeviceChanged(_ID, "MaxJ1", value)
                End Set
            End Property

            Dim _IconJ1 As String = ""
            Public Property IconJ1() As String
                Get
                    Return _IconJ1
                End Get
                Set(ByVal value As String)
                    _IconJ1 = value
                End Set
            End Property

            Dim _ConditionJ1 As String = ""
            Public Property ConditionJ1() As String
                Get
                    Return _ConditionJ1
                End Get
                Set(ByVal value As String)
                    _ConditionJ1 = value
                    RaiseEvent DeviceChanged(_ID, "ConditionJ1", value)
                End Set
            End Property

            Dim _JourJ2 As String = ""
            Public Property JourJ2() As String
                Get
                    Return _JourJ2
                End Get
                Set(ByVal value As String)
                    _JourJ2 = value
                    RaiseEvent DeviceChanged(_ID, "JourJ2", value)
                End Set
            End Property

            Dim _MinJ2 As String = ""
            Public Property MinJ2() As String
                Get
                    Return _MinJ2
                End Get
                Set(ByVal value As String)
                    _MinJ2 = value
                    RaiseEvent DeviceChanged(_ID, "MinJ2", value)
                End Set
            End Property

            Dim _MaxJ2 As String = ""
            Public Property MaxJ2() As String
                Get
                    Return _MaxJ2
                End Get
                Set(ByVal value As String)
                    _MaxJ2 = value
                    RaiseEvent DeviceChanged(_ID, "MaxJ2", value)
                End Set
            End Property

            Dim _IconJ2 As String = ""
            Public Property IconJ2() As String
                Get
                    Return _IconJ2
                End Get
                Set(ByVal value As String)
                    _IconJ2 = value
                End Set
            End Property

            Dim _ConditionJ2 As String = ""
            Public Property ConditionJ2() As String
                Get
                    Return _ConditionJ2
                End Get
                Set(ByVal value As String)
                    _ConditionJ2 = value
                    RaiseEvent DeviceChanged(_ID, "ConditionJ2", value)
                End Set
            End Property

            Dim _JourJ3 As String = ""
            Public Property JourJ3() As String
                Get
                    Return _JourJ3
                End Get
                Set(ByVal value As String)
                    _JourJ3 = value
                    RaiseEvent DeviceChanged(_ID, "JourJ3", value)
                End Set
            End Property

            Dim _MinJ3 As String = ""
            Public Property MinJ3() As String
                Get
                    Return _MinJ3
                End Get
                Set(ByVal value As String)
                    _MinJ3 = value
                    RaiseEvent DeviceChanged(_ID, "MinJ3", value)
                End Set
            End Property

            Dim _MaxJ3 As String = ""
            Public Property MaxJ3() As String
                Get
                    Return _MaxJ3
                End Get
                Set(ByVal value As String)
                    _MaxJ3 = value
                    RaiseEvent DeviceChanged(_ID, "MaxJ3", value)
                End Set
            End Property

            Dim _IconJ3 As String = ""
            Public Property IconJ3() As String
                Get
                    Return _IconJ3
                End Get
                Set(ByVal value As String)
                    _IconJ3 = value
                End Set
            End Property

            Dim _ConditionJ3 As String = ""
            Public Property ConditionJ3() As String
                Get
                    Return _ConditionJ3
                End Get
                Set(ByVal value As String)
                    _ConditionJ3 = value
                    RaiseEvent DeviceChanged(_ID, "ConditionJ3", value)
                End Set
            End Property


            Dim _TimeRefresh As DateTime
            Public Driver As New Object
            Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As String)

            Public Property ID() As String
                Get
                    Return _ID
                End Get
                Set(ByVal Value As String)
                    _ID = Value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    _Name = Value
                End Set
            End Property

            Public Property Picture() As String
                Get
                    Return _Picture
                End Get
                Set(ByVal Value As String)
                    _Picture = Value
                End Set
            End Property

            Public Property DriverID() As String
                Get
                    Return _DriverID
                End Get
                Set(ByVal Value As String)
                    _DriverID = Value
                End Set
            End Property

            Public Property Adresse() As String
                Get
                    Return _Adresse
                End Get
                Set(ByVal Value As String)
                    _Adresse = Value
                End Set
            End Property

            Public Property Enable() As Boolean
                Get
                    Return _Enable
                End Get
                Set(ByVal Value As Boolean)
                    _Enable = Value
                    If Value = True Then
                        Dim y As New Thread(AddressOf MAJ)
                        y.Start()
                        x.Interval = 60000
                        AddHandler x.Elapsed, AddressOf TimerTick
                        x.Enabled = True
                    Else
                        x.Enabled = False
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                If (Now.Minute = 0 Or Now.Minute = 10 Or Now.Minute = 20 Or Now.Minute = 30 Or Now.Minute = 40 Or Now.Minute = 50) Then
                    Dim x As New Thread(AddressOf MAJ)
                    x.Start()
                    x = Nothing
                End If
            End Sub

            Public ReadOnly Property TypeClass() As String
                Get
                    Return mClass
                End Get
            End Property

            Public ReadOnly Property TimeRefresh() As DateTime
                Get
                    Return _TimeRefresh
                End Get
            End Property

            Public Sub MAJ()
                Try
                    'Si internet n'est pas disponible on ne mets pas à jour les informations
                    If My.Computer.Network.IsAvailable = False Then
                        Exit Sub
                    End If


                    'Dim GoogleRequest As HttpWebRequest
                    'Dim GoogleResponse As HttpWebResponse = Nothing
                    Dim doc As New XmlDocument
                    Dim nodes As XmlNodeList

                    ' Create a new XmlDocument   
                    doc = New XmlDocument()

                    Dim url As New Uri("http://www.google.com/ig/api?weather=" & _Adresse & "&hl=fr")
                    Dim Request As HttpWebRequest = CType(HttpWebRequest.Create(url), System.Net.HttpWebRequest)
                    Request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; fr; rv:1.8.0.7) Gecko/20060909 Firefox/1.5.0.7"
                    Dim response As Net.HttpWebResponse = CType(Request.GetResponse(), Net.HttpWebResponse)


                    doc.Load(response.GetResponseStream)
                    nodes = doc.SelectNodes("/xml_api_reply/weather/current_conditions")
                    For Each node As XmlNode In nodes
                        For j = 0 To node.ChildNodes.Count - 1
                            Dim a As String = node.ChildNodes.Item(j).Name
                            If node.ChildNodes.Item(j).Attributes.Count > 0 Then
                                Dim b As String = node.ChildNodes.Item(j).Attributes(0).Value
                                Select Case a
                                    Case "condition"
                                        ConditionActuel = b
                                    Case "temp_c"
                                        TemperatureActuel = b
                                    Case "humidity"
                                        HumiditeActuel = b
                                    Case "icon"
                                        IconActuel = b
                                    Case "wind_condition"
                                        VentActuel = b
                                End Select
                            End If
                        Next
                    Next
                    nodes = doc.SelectNodes("/xml_api_reply/weather/forecast_conditions")
                    Dim idx As Byte = 0
                    For Each node As XmlNode In nodes
                        For j = 0 To node.ChildNodes.Count - 1
                            Dim a As String = node.ChildNodes.Item(j).Name
                            If node.ChildNodes.Item(j).Attributes.Count > 0 Then
                                Dim b As String = node.ChildNodes.Item(j).Attributes(0).Value
                                Select Case a
                                    Case "day_of_week"
                                        Select Case idx
                                            Case 0
                                                JourToday = b
                                            Case 1
                                                JourJ1 = b
                                            Case 2
                                                JourJ2 = b
                                            Case 3
                                                JourJ3 = b
                                        End Select
                                    Case "low"
                                        Select Case idx
                                            Case 0
                                                MinToday = b
                                            Case 1
                                                MinJ1 = b
                                            Case 2
                                                MinJ2 = b
                                            Case 3
                                                MinJ3 = b
                                        End Select
                                    Case "high"
                                        Select Case idx
                                            Case 0
                                                MaxToday = b
                                            Case 1
                                                MaxJ1 = b
                                            Case 2
                                                MaxJ2 = b
                                            Case 3
                                                MaxJ3 = b
                                        End Select
                                    Case "icon"
                                        Select Case idx
                                            Case 0
                                                IconToday = b
                                            Case 1
                                                IconJ1 = b
                                            Case 2
                                                IconJ2 = b
                                            Case 3
                                                IconJ3 = b
                                        End Select
                                    Case "condition"
                                        Select Case idx
                                            Case 0
                                                ConditionToday = b
                                            Case 1
                                                ConditionJ1 = b
                                            Case 2
                                                ConditionJ2 = b
                                            Case 3
                                                ConditionJ3 = b
                                        End Select
                                End Select
                            End If
                        Next
                        idx += 1
                    Next

                    doc = Nothing
                    nodes = Nothing

                    _TimeRefresh = Now

                    _Log.AddToLog(Log.TypeLog.ERREUR, "DEVICE METEO", "MAJ Meteo effectuée pour " & _Name)
                Catch ex As Exception
                    _Log.AddToLog(Log.TypeLog.ERREUR, "DEVICE METEO", "Erreur Getmeteo " & _Name & " " & ex.Message)
                End Try
            End Sub

            Protected Overrides Sub Finalize()
                x.Enabled = False
                MyBase.Finalize()
            End Sub

            Private Function TraduireJour(ByVal Jour As String) As String
                TraduireJour = "?"
                Select Case Jour
                    Case "Thu"
                        TraduireJour = "Jeu"
                    Case "Fri"
                        TraduireJour = "Ven"
                    Case "Sat"
                        TraduireJour = "Sam"
                    Case "Sun"
                        TraduireJour = "Dim"
                    Case "Mon"
                        TraduireJour = "Lun"
                    Case "Tue"
                        TraduireJour = "Mar"
                    Case "Wed"
                        TraduireJour = "Mer"
                End Select
            End Function

            Private Function Traduire(ByVal txt)

                Traduire = txt

                If txt = "A Few Clouds" Then
                    Traduire = "Quelques nuages"
                End If
                If txt = "A Few Clouds and Breezy" Then
                    Traduire = "Quelques nuages et Frais"
                End If
                If txt = "A Few Clouds and Windy" Then
                    Traduire = "Quelques nuages et  et grand vent"
                End If
                If txt = "A Few Clouds with Haze" Then
                    Traduire = "Quelques nuages et brume"
                End If
                If txt = "AM Clouds / PM Sun" Then
                    Traduire = "Matin nuages / Après-Midi Soleil"
                End If
                If txt = "AM Fog / PM Sun" Then
                    Traduire = "Matin Brouillard / Après-Midi Soleil"
                End If
                If txt = "AM Rain / Snow Showers" Then
                    Traduire = "Matin pluie / averses neigeuses"
                End If
                If txt = "AM Showers" Then
                    Traduire = "Averses matinales"
                End If
                If txt = "AM Snow Showers" Then
                    Traduire = "Averses de neige le matin"
                End If
                If txt = "AM T-Storms" Then
                    Traduire = "Orages le matin"
                End If
                If txt = "Blowing Dust" Then
                    Traduire = "Vent de poussière"
                End If
                If txt = "Blowing Sand" Then
                    Traduire = "Vent de sable"
                End If
                If txt = "Blowing Snow" Then
                    Traduire = "Vent de neige"
                End If
                If txt = "Blowing Snow in Vicinity" Then
                    Traduire = "Vent de neige dans les environs"
                End If
                If txt = "Clear" Then
                    Traduire = "Clair"
                End If
                If txt = "Clear / Wind" Then
                    Traduire = "Clair / vent"
                End If
                If txt = "Clear and Breezy" Then
                    Traduire = "Clair et frais"
                End If
                If txt = "Clear with Haze" Then
                    Traduire = "Clair avec brume légère"
                End If
                If txt = "Cloudy Clair" Then
                    Traduire = "Nuageux clair"
                End If
                If txt = "Clouds Early / Clearing Late" Then
                    Traduire = "Nuages matinaux suivis d'éclaircies"
                End If
                If txt = "Cloudy" Then
                    Traduire = "Nuageux"
                End If
                If txt = "Cloudy / Wind" Then
                    Traduire = "Nuageux / Vent"
                End If
                If txt = "Cloudy / Windy" Then
                    Traduire = ""
                End If
                If txt = "Drifting Snow" Then
                    Traduire = "Amoncellement de neige"
                End If
                If txt = "Drizzle" Then
                    Traduire = "Crachin"
                End If
                If txt = "Drizzle Fog/Mist" Then
                    Traduire = "Crachin brouillard/brume"
                End If
                If txt = "Drizzle Fog" Then
                    Traduire = "Crachin brouillard"
                End If
                If txt = "Drizzle Ice Pellets" Then
                    Traduire = "Crachin grêlons"
                End If
                If txt = "Drizzle Snow" Then
                    Traduire = "Crachin neige"
                End If
                If txt = "Dust" Then
                    Traduire = "Poussière"
                End If
                If txt = "Dust Storm in Vicinity" Then
                    Traduire = "Tempête de poussière dans les environs"
                End If
                If txt = "Dust Storm" Then
                    Traduire = "Tempête de poussière"
                End If
                If txt = "Dust/Sand Whirls" Then
                    Traduire = "Tourbillons de poussière/sable"
                End If
                If txt = "Dust/Sand Whirls in Vicinity" Then
                    Traduire = "Tourbillons de poussière/sable dans les environs"
                End If
                If txt = "Fair" Then
                    Traduire = "Ciel dégagé"
                End If
                If txt = "Fair/ Windy" Then
                    Traduire = "Ciel dégagé / grand vent"
                End If
                If txt = "Fair and Breezy" Then
                    Traduire = "Ciel dégagé et frais"
                End If
                If txt = "Fair and Windy" Then
                    Traduire = "Ciel dégagé et grand vent"
                End If
                If txt = "Fair with Haze" Then
                    Traduire = "Ciel dégagé avec brume légère"
                End If
                If txt = "Few Showers" Then
                    Traduire = "Quelques averses"
                End If
                If txt = "Few Snow Showers" Then
                    Traduire = "Quelques averses de neige"
                End If
                If txt = "Few Snow Showers / Wind" Then
                    Traduire = "Quelques averses de neige / Vent"
                End If
                If txt = "Fog" Then
                    Traduire = "Brouillard"
                End If
                If txt = "Fog in Vicinity" Then
                    Traduire = "Brouillard dans les environs"
                End If
                If txt = "Fog/Mist" Then
                    Traduire = "Brouillard/brume"
                End If
                If txt = "Freezing Drizzle" Then
                    Traduire = "Crachin givrant"
                End If
                If txt = "Freezing Drizzle in Vicinity" Then
                    Traduire = "Crachin givrant dans les environs"
                End If
                If txt = "Freezing Drizzle Rain" Then
                    Traduire = "Crachin pluie givrant "
                End If
                If txt = "Freezing Drizzle Snow" Then
                    Traduire = "Crachin neige givrant"
                End If
                If txt = "Freezing Fog" Then
                    Traduire = "Brouillard givrant"
                End If
                If txt = "Freezing Fog in Vicinity" Then
                    Traduire = "Brouillard givrant dans les environs"
                End If
                If txt = "Freezing Rain" Then
                    Traduire = "Pluie givrante"
                End If
                If txt = "Freezing Rain in Vicinity" Then
                    Traduire = "Pluie givrante dans les environs"
                End If
                If txt = "Freezing Rain Snow" Then
                    Traduire = "Pluie neige givrante dans les environs"
                End If
                If txt = "Frigid" Then
                    Traduire = "Grand froid"
                End If
                If txt = "Funnel Cloud in Vicinity" Then
                    Traduire = "Nuage en entonnoir dans les environs"
                End If
                If txt = "Funnel Cloud" Then
                    Traduire = "Nuage en entonnoir"
                End If
                If txt = "Hail" Then
                    Traduire = "Grêle"
                End If
                If txt = "Hail Showers" Then
                    Traduire = "Averses de grêle"
                End If
                If txt = "Haze" Then
                    Traduire = "Brume légère"
                End If
                If txt = "Heavy Drizzle" Then
                    Traduire = "Gros Crachin "
                End If
                If txt = "Heavy Drizzle Fog/Mist" Then
                    Traduire = "Gros Crachin Brouillard/brume"
                End If
                If txt = "Heavy Drizzle Fog" Then
                    Traduire = "Gros Crachin Brouillard"
                End If
                If txt = "Heavy Drizzle Ice Pellets" Then
                    Traduire = "Gros Crachin Grêlons"
                End If
                If txt = "Heavy Drizzle Snow" Then
                    Traduire = "Gros Crachin Neige"
                End If
                If txt = "Heavy Dust Storm" Then
                    Traduire = "Grosse tempête de poussière"
                End If
                If txt = "Heavy Freezing Drizzle Rain" Then
                    Traduire = "Gros Crachin verglassant Pluie"
                End If
                If txt = "Heavy Freezing Drizzle Snow" Then
                    Traduire = "Gros Crachin verglassant Neige"
                End If
                If txt = "Heavy Freezing Drizzle" Then
                    Traduire = "Gros Crachin verglassant"
                End If
                If txt = "Heavy Freezing Fog" Then
                    Traduire = "Gros brouillard verglassant"
                End If
                If txt = "Heavy Freezing Rain Snow" Then
                    Traduire = "Grosse pluie verglassante Neige"
                End If
                If txt = "Heavy Freezing Rain" Then
                    Traduire = "Grosse pluie verglassante"
                End If
                If txt = "Heavy Ice Pellets" Then
                    Traduire = "Gros Grêlons"
                End If
                If txt = "Heavy Ice Pellets Drizzle" Then
                    Traduire = "Gros Grêlons Crachin"
                End If
                If txt = "Heavy Ice Pellets Rain" Then
                    Traduire = "Gros Grêlons Pluie"
                End If
                If txt = "Heavy Rain" Then
                    Traduire = "Grosse pluie"
                End If
                If txt = "Heavy Rain Fog/Mist" Then
                    Traduire = "Grosse pluie Brouillard/brume"
                End If
                If txt = "Heavy Rain Fog" Then
                    Traduire = "Grosse pluie Brouillard"
                End If
                If txt = "Heavy Rain Freezing Drizzle" Then
                    Traduire = "Grosse pluie Crachin verglassant"
                End If
                If txt = "Heavy Rain Freezing Rain" Then
                    Traduire = "Grosse pluie Pluie verglassante"
                End If
                If txt = "Heavy Rain Ice Pellets" Then
                    Traduire = "Grosse pluie Grêlons"
                End If
                If txt = "Heavy Rain Icy" Then
                    Traduire = "Grosse pluie verglassante"
                End If
                If txt = "Heavy Rain Showers Fog/Mist" Then
                    Traduire = "Grosse pluie Averses Brouillard/brume"
                End If
                If txt = "Heavy Rain Showers" Then
                    Traduire = "Grosse pluie Averses"
                End If
                If txt = "Heavy Rain Snow" Then
                    Traduire = "Grosse pluie Neige"
                End If
                If txt = "Heavy Sand Storm" Then
                    Traduire = "Grosse tempête de sable "
                End If
                If txt = "Heavy Showers Rain Fog/Mist" Then
                    Traduire = "Grosses averses Pluie Brouillard/brume"
                End If
                If txt = "Heavy Showers Rain" Then
                    Traduire = "Grosses averses Pluie"
                End If
                If txt = "Heavy Showers Snow Fog/Mist" Then
                    Traduire = "Grosses Neige Pluie Brouillard/brume"
                End If
                If txt = "Heavy Showers Snow Fog" Then
                    Traduire = "Grosses averses Neige Brouillard"
                End If
                If txt = "Heavy Showers Snow" Then
                    Traduire = "Grosses averses Neige"
                End If
                If txt = "Heavy Snow" Then
                    Traduire = "Beaucoup de neige"
                End If
                If txt = "Heavy Snow Drizzle" Then
                    Traduire = "Beaucoup de neige Crachin"
                End If
                If txt = "Heavy Snow Fog/Mist" Then
                    Traduire = "Beaucoup de neige Brouillard/brume"
                End If
                If txt = "Heavy Snow Fog" Then
                    Traduire = "Beaucoup de neige Brouillard"
                End If
                If txt = "Heavy Snow Freezing Drizzle" Then
                    Traduire = "Beaucoup de neige Crachin verglassant"
                End If
                If txt = "Heavy Snow Freezing Rain" Then
                    Traduire = "Beaucoup de neige Pluie verglassante"
                End If
                If txt = "Heavy Snow Rain" Then
                    Traduire = "Beaucoup de neige Pluie"
                End If
                If txt = "Heavy Snow Showers Fog/Mist" Then
                    Traduire = "Beaucoup de neige Averses Brouillard/brume"
                End If
                If txt = "Heavy Snow Showers Fog" Then
                    Traduire = "Beaucoup de neige Averses Brouillard"
                End If
                If txt = "Heavy Snow Showers" Then
                    Traduire = "Beaucoup de neige Averses"
                End If
                If txt = "Heavy T-Storm" Then
                    Traduire = "Gros orage"
                End If
                If txt = "Heavy T-Storms Rain Fog/Mist" Then
                    Traduire = "Gros orage Brouillard/brume"
                End If
                If txt = "Heavy T-Storms Rain Fog" Then
                    Traduire = "Gros orage Brouillard"
                End If
                If txt = "Heavy T-Storms Rain Hail Fog/Mist" Then
                    Traduire = "Gros orage Grêle Brouillard/brume"
                End If
                If txt = "Heavy T-Storms Rain Hail Fog" Then
                    Traduire = "Gros orage Grêle  Brouillard"
                End If
                If txt = "Heavy T-Storms Rain Hail Haze" Then
                    Traduire = "Gros orage Grêle Brume légère"
                End If
                If txt = "Heavy T-Storms Rain Hail" Then
                    Traduire = "Gros orage Grêle"
                End If
                If txt = "Heavy T-Storms Rain Haze" Then
                    Traduire = "Gros orage Brume légère"
                End If
                If txt = "Heavy T-Storms Rain" Then
                    Traduire = "Gros orage Pluie"
                End If
                If txt = "Heavy T-Storms Snow" Then
                    Traduire = "Gros orage Neige"
                End If
                If txt = "Ice Crystals" Then
                    Traduire = "Cristaux de glace"
                End If
                If txt = "Ice Pellets Drizzle" Then
                    Traduire = "Grêlons Crachin"
                End If
                If txt = "Ice Pellets in Vicinity" Then
                    Traduire = "Grêlons dans les environs "
                End If
                If txt = "Ice Pellets Rain" Then
                    Traduire = "Grêlons Pluie"
                End If
                If txt = "Ice Pellets" Then
                    Traduire = "Grêlons"
                End If
                If txt = "Isolated T-Storms" Then
                    Traduire = "Orages isolés"
                End If
                If txt = "Light Drizzle" Then
                    Traduire = "Léger crachin"
                End If
                If txt = "Light Drizzle Fog/Mist" Then
                    Traduire = "Léger crachin Brouillard/brume"
                End If
                If txt = "Light Drizzle Fog" Then
                    Traduire = "Léger crachin Brouillard"
                End If
                If txt = "Light Drizzle Ice Pellets" Then
                    Traduire = "Léger crachin Grêlons"
                End If
                If txt = "Light Drizzle Snow" Then
                    Traduire = "Léger crachin Neige"
                End If
                If txt = "Light Freezing Drizzle" Then
                    Traduire = "Léger crachin verglassant"
                End If
                If txt = "Light Freezing Drizzle Rain" Then
                    Traduire = "Léger crachin verglassant Pluie"
                End If
                If txt = "Light Freezing Drizzle Snow" Then
                    Traduire = "Léger crachin verglassant Neige"
                End If
                If txt = "Light Freezing Fog" Then
                    Traduire = "Léger brouillard verglassant"
                End If
                If txt = "Light Freezing Rain" Then
                    Traduire = "Légère pluie verglassante"
                End If
                If txt = "Light Freezing Rain Snow" Then
                    Traduire = "Légère pluie verglassante Neige"
                End If
                If txt = "Light Ice Pellets Drizzle" Then
                    Traduire = "Petit Grêlons Crachin"
                End If
                If txt = "Light Ice Pellets Rain" Then
                    Traduire = "Petit Grêlons Pluie"
                End If
                If txt = "Light Ice Pellets" Then
                    Traduire = "Petit Grêlons"
                End If
                If txt = "Light Rain" Then
                    Traduire = "Légère Pluie"
                End If
                If txt = "Light Rain Early" Then
                    Traduire = "Légère Pluie matinal"
                End If
                If txt = "Light Rain Fog/Mist" Then
                    Traduire = "Légère Pluie Brouillard/brume"
                End If
                If txt = "Light Rain Fog" Then
                    Traduire = "Légère Pluie Brouillard"
                End If
                If txt = "Light Rain Freezing Drizzle" Then
                    Traduire = "Légère Pluie Crachin verglassant"
                End If
                If txt = "Light Rain Freezing Rain" Then
                    Traduire = "Légère Pluie Pluie verglassante"
                End If
                If txt = "Light Rain Ice Pellets" Then
                    Traduire = "Légère Pluie Grêlons"
                End If
                If txt = "Light Rain Icy" Then
                    Traduire = "Légère pluie verglassante"
                End If
                If txt = "Light Rain Late" Then
                    Traduire = "Légère Pluie tardive"
                End If
                If txt = "Light Rain Shower" Then
                    Traduire = "Légère pluie Averse"
                End If
                If txt = "Light Rain Shower and Windy" Then
                    Traduire = "Légère pluie Averses et Vent"
                End If
                If txt = "Light Rain Showers" Then
                    Traduire = "Légères pluie Averses"
                End If
                If txt = "Light Rain Snow" Then
                    Traduire = "Légères pluie Neige"
                End If
                If txt = "Light Rain with Thunder" Then
                    Traduire = "Légère Pluie avec tonnerre"
                End If
                If txt = "Light Showers Rain" Then
                    Traduire = "Légère averses Pluie"
                End If
                If txt = "Light Showers Rain Fog/Mist" Then
                    Traduire = "Légère averses Pluie Brouillard/brume"
                End If
                If txt = "Light Showers Snow" Then
                    Traduire = "Légère averses Neige"
                End If
                If txt = "Light Showers Snow Fog" Then
                    Traduire = "Légère averses Neige Brouillard"
                End If
                If txt = "Light Showers Snow Fog/Mist" Then
                    Traduire = "Légère averses Neige Brouillard/brume"
                End If
                If txt = "Light Snow" Then
                    Traduire = "Peu de neige"
                End If
                If txt = "Light Snow Drizzle" Then
                    Traduire = "Peu de neige Crachin"
                End If
                If txt = "Light Snow Fall" Then
                    Traduire = "Légère chutes de neige"
                End If
                If txt = "Light Snow Fog/Mist" Then
                    Traduire = "Peu de neige Brouillard/brume"
                End If
                If txt = "Light Snow Fog" Then
                    Traduire = "Peu de neige Brouillard"
                End If
                If txt = "Light Snow Freezing Drizzle" Then
                    Traduire = "Peu de neige Crachin verglassant"
                End If
                If txt = "Light Snow Freezing Rain" Then
                    Traduire = "Peu de neige Pluie Verglassante"
                End If
                If txt = "Light Snow Grains" Then
                    Traduire = "Quelques flocons de neige"
                End If
                If txt = "Light Snow Rain" Then
                    Traduire = "Peu de neige Pluie"
                End If
                If txt = "Light Snow Shower" Then
                    Traduire = "Légère averse de neige"
                End If
                If txt = "Light Snow Showers Fog/Mist" Then
                    Traduire = "Légère averses de neige Brouillard/brume"
                End If
                If txt = "Light Snow Showers Fog" Then
                    Traduire = "Légère averses de neige Brouillard"
                End If
                If txt = "Light T-Storms Rain Fog/Mist" Then
                    Traduire = "Léger orages Pluie Brouillard/brume"
                End If
                If txt = "Light T-Storms Rain Fog" Then
                    Traduire = "Léger orages Pluie Brouillard"
                End If
                If txt = "Light T-Storms Rain Hail Fog/Mist" Then
                    Traduire = "Léger orages Pluie Grêle Brouillard/brume"
                End If
                If txt = "Light T-Storms Rain Hail Fog" Then
                    Traduire = "Léger orages Pluie Grêle Brouillard"
                End If
                If txt = "Light T-Storms Rain Hail Haze" Then
                    Traduire = "Léger orages Pluie Grêle Brume légère"
                End If
                If txt = "Light T-Storms Rain Hail" Then
                    Traduire = "Léger orages Pluie Grêle"
                End If
                If txt = "Light T-Storms Rain Haze" Then
                    Traduire = "Léger orages Pluie Brume légère"
                End If
                If txt = "Light T-Storms Rain" Then
                    Traduire = "Léger orages Pluie"
                End If
                If txt = "Light T-Storms Snow" Then
                    Traduire = "Léger orages Neige"
                End If
                If txt = "Lightening" Then
                    Traduire = "Eclairs"
                End If
                If txt = "Lightenings" Then
                    Traduire = "Eclairs"
                End If
                If txt = "Mostly Clear" Then
                    Traduire = "Ciel plutôt dégagé"
                End If
                If txt = "Mostly Cloudy" Then
                    Traduire = "Plutôt nuageux"
                End If
                If txt = "Mostly Cloudy and Breezy" Then
                    Traduire = "Plutôt nuageux et Venteux"
                End If
                If txt = "Mostly Cloudy and Windy" Or txt = "Mostly Cloudy/Wind" Then
                    Traduire = "Plutôt nuageux et Grand vent"
                End If
                If txt = "Mostly Cloudy with Haze" Then
                    Traduire = "Plutôt nuageux avec Légère Brume"
                End If
                If txt = "Mostly Sunny" Then
                    Traduire = "Plutôt ensoleillé"
                End If
                If txt = "Mostly Sunny / Wind" Then
                    Traduire = "Plutôt ensoleillé / vent"
                End If
                If txt = "Overcast" Then
                    Traduire = "Couvert"
                End If
                If txt = "Overcast and Breezy" Then
                    Traduire = "Couvert et Venteux"
                End If
                If txt = "Overcast and Windy" Then
                    Traduire = "Couvert et Grand vent"
                End If
                If txt = "Overcast with Haze" Then
                    Traduire = "Couvert avec légère brume"
                End If
                If txt = "Partial Fog" Then
                    Traduire = "Banc de Brouillard"
                End If
                If txt = "Partial Fog in Vicinity" Then
                    Traduire = "Banc de Brouillard dans les environs"
                End If
                If txt = "P Cloudy" Then
                    Traduire = "Partiellement nuageux"
                End If
                If txt = "Partly Cloudy" Then
                    Traduire = "Partiellement nuageux"
                End If
                If txt = "Partly Cloudy and Breezy" Then
                    Traduire = "Partiellement nuageux et Venteux"
                End If
                If txt = "Partly Cloudy and Windy" Then
                    Traduire = "Partiellement nuageux et Grand vent"
                End If
                If txt = "Partly Cloudy / Wind" Then
                    Traduire = "Partiellement nuageux / Vent"
                End If
                If txt = "Partly Cloudy/ Windy" Or txt = "Partly Cloudy/Wind" Then
                    Traduire = "Partiellement nuageux / Venteux"
                End If
                If txt = "Party Cloudy with Haze" Then
                    Traduire = "Partiellement nuageux avec légère brume"
                End If
                If txt = "Partly Sunny" Then
                    Traduire = "Partiellement ensoleillé"
                End If
                If txt = "Patches of Fog" Then
                    Traduire = "Nappes de Brouillard"
                End If
                If txt = "Patches of Fog in Vicinity" Then
                    Traduire = "Nappes de Brouillard dans les environs"
                End If
                If txt = "PM light rain" Then
                    Traduire = "PM Légère Pluie"
                End If
                If txt = "PM Rain / Snow" Then
                    Traduire = "PM Pluie / Neige"
                End If
                If txt = "PM Rain / Wind" Then
                    Traduire = "PM Pluie / Vent"
                End If
                If txt = "PM Showers" Then
                    Traduire = "PM Averses"
                End If
                If txt = "PM Snow Showers" Then
                    Traduire = "PM averses neigeuses"
                End If
                If txt = "PM T-Storms" Then
                    Traduire = "Orages l après-midi"
                End If
                If txt = "Rain" Then
                    Traduire = "Pluie"
                End If
                If txt = "Rain / Snow" Then
                    Traduire = "Pluie / neige"
                End If
                If txt = "Rain / Snow Showers" Then
                    Traduire = "Pluie / averses neigeuses"
                End If
                If txt = "Rain / Snow Showers Early" Then
                    Traduire = "Pluie / averses neigeuses matinales"
                End If
                If txt = "Rain / Thunder" Then
                    Traduire = "Pluie / Tonnerre"
                End If
                If txt = "Rain / Wind" Then
                    Traduire = "Pluie / Vent"
                End If
                If txt = "Rain and Snow" Then
                    Traduire = "Pluie et Neige"
                End If
                If txt = "Rain Early" Then
                    Traduire = "Pluie matinale"
                End If
                If txt = "Rain Fog/Mist" Then
                    Traduire = "Pluie Brouillard/brume"
                End If
                If txt = "Rain Fog" Then
                    Traduire = "Pluie Brouillard"
                End If
                If txt = "Rain Freezing Drizzle" Then
                    Traduire = "Pluie Crachin verglassant"
                End If
                If txt = "Rain Freezing Rain" Then
                    Traduire = "Pluie Pluie Verglassante"
                End If
                If txt = "Rain Ice Pellets" Then
                    Traduire = "Pluie Grêlons"
                End If
                If txt = "Rain Shower" Then
                    Traduire = "Pluie Averses"
                End If
                If txt = "Rain Showers Fog/Mist" Then
                    Traduire = "Pluie Averses Brouillard/brume"
                End If
                If txt = "Rain Showers in Vicinity Fog/Mist" Then
                    Traduire = "Pluie Averses dans les environs Brouillard/brume"
                End If
                If txt = "Rain Showers in Vicinity" Then
                    Traduire = "Pluie Averses dans les environs"
                End If
                If txt = "Rain Snow" Then
                    Traduire = "Pluie Neige"
                End If
                If txt = "Rain to Snow" Then
                    Traduire = "Pluie vers Neige"
                End If
                If txt = "Sand Storm" Then
                    Traduire = "Tempête de Sable"
                End If
                If txt = "Sand Storm in Vicinity" Then
                    Traduire = "Tempête de Sable dans les environs"
                End If
                If txt = "Sand" Then
                    Traduire = "Sable"
                End If
                If txt = "Shallow Fog" Then
                    Traduire = "Brouillard superficiel"
                End If
                If txt = "Shallow Fog in Vicinity" Then
                    Traduire = "Brouillard superficiel dans les environs"
                End If
                If txt = "Scattered Showers" Then
                    Traduire = "Averses éparses"
                End If
                If txt = "Scattered Showers / Wind" Then
                    Traduire = "Averses éparses / Vent"
                End If
                If txt = "Scattered Snow Showers" Then
                    Traduire = "Averses neigeuses éparses"
                End If
                If txt = "Scattered Snow Showers / Wind" Then
                    Traduire = "Averses neigeuses éparses / Vent"
                End If
                If txt = "Scattered Strong Storms" Then
                    Traduire = "Violents orages locals"
                End If
                If txt = "Scattered T-Storms" Then
                    Traduire = "Orages éparses"
                End If
                If txt = "Showers" Then
                    Traduire = "Averses"
                End If
                If txt = "Showers / Wind" Then
                    Traduire = "Averses / Vent"
                End If
                If txt = "Showers Hail" Then
                    Traduire = "Averses Grêle"
                End If
                If txt = "Showers Ice Pellets" Then
                    Traduire = "Averses Grêlons"
                End If
                If txt = "Showers in the Vicinity" Then
                    Traduire = "Averses dans les environs"
                End If
                If txt = "Showers in Vicinity Fog/Mist" Then
                    Traduire = "Averses dans les environs Brouillard/brume"
                End If
                If txt = "Showers in Vicinity Fog" Then
                    Traduire = "Averses dans les environs Brouillard"
                End If
                If txt = "Showers in Vicinity Haze" Then
                    Traduire = "Averses dans les environs Brume légère"
                End If
                If txt = "Showers in Vicinity Snow" Then
                    Traduire = "Averses dans les environs Neige"
                End If
                If txt = "Showers Early" Then
                    Traduire = "Averses matinales"
                End If
                If txt = "Showers Late" Then
                    Traduire = "Averses tardives"
                End If
                If txt = "Showers Rain" Then
                    Traduire = "Averses Pluie"
                End If
                If txt = "Showers Rain Fog/Mist" Then
                    Traduire = "Averses Pluie Brouillard/brume"
                End If
                If txt = "Showers Rain in Vicinity" Then
                    Traduire = "Averses Pluie dans les environs"
                End If
                If txt = "Showers Rain in Vicinity Fog/Mist" Then
                    Traduire = "Averses Pluie dans les environs Brouillard/brume"
                End If
                If txt = "Showers Snow" Then
                    Traduire = "Averses Neige"
                End If
                If txt = "Showers Snow Fog" Then
                    Traduire = "Averses Neige Brouillard"
                End If
                If txt = "Showers Snow Fog/Mist" Then
                    Traduire = "Averses Neige Brouillard/brume"
                End If
                If txt = "Smoke" Then
                    Traduire = "fumée"
                End If
                If txt = "Snow" Then
                    Traduire = "Neige"
                End If
                If txt = "Snow / Rain Icy Mix" Then
                    Traduire = "Mélange Neige / Pluie Verglassante"
                End If
                If txt = "Snow and Fog" Then
                    Traduire = "Neige et Brouillard"
                End If
                If txt = "Snow Drizzle" Then
                    Traduire = "Neige Crachin"
                End If
                If txt = "Snow Fog/Mist" Then
                    Traduire = "Neige Brouillard/brume"
                End If
                If txt = "Snow Freezing Drizzle" Then
                    Traduire = "Neige Crachin verglassant"
                End If
                If txt = "Snow Freezing Rain" Then
                    Traduire = "Neige Pluie verglassante"
                End If
                If txt = "Snow Rain" Then
                    Traduire = "Neige Pluie"
                End If
                If txt = "Snow Shower" Then
                    Traduire = "Averses de neige"
                End If
                If txt = "Snow Shower / Wind" Then
                    Traduire = "Averses de neige / vent"
                End If
                If txt = "Snow Shower Early" Then
                    Traduire = "Averses de neige matinales"
                End If
                If txt = "Snow Showers Fog/Mist" Then
                    Traduire = "Averses de neige Brouillard/brume"
                End If
                If txt = "Snow Showers Fog" Then
                    Traduire = "Averses de neige Brouillard"
                End If
                If txt = "Snow Showers in Vicinity" Then
                    Traduire = "Averses de neige dans les environs"
                End If
                If txt = "Snow Showers in Vicinity Fog" Then
                    Traduire = "Averses de neige dans les environs Brouillard"
                End If
                If txt = "Snow Showers in Vicinity Fog/Mist" Then
                    Traduire = "Averses de neige dans les environs Brouillard/brume"
                End If
                If txt = "Snow Showers Late" Then
                    Traduire = "Averses de neige tardives"
                End If
                If txt = "Snow to Rain" Then
                    Traduire = "Neige vers Pluie"
                End If
                If txt = "Snowflakes" Then
                    Traduire = "Flocons de neige"
                End If
                If txt = "Sunny" Then
                    Traduire = "Ensoleillé"
                End If
                If txt = "Sunny / Wind" Then
                    Traduire = "Ensoleillé / Vent"
                End If
                If txt = "Sunny Day" Then
                    Traduire = "Journée ensoleillé"
                End If
                If txt = "Thunder" Then
                    Traduire = "Tonnerre"
                End If
                If txt = "Thunder in the Vicinity" Then
                    Traduire = "Tonnerre dans les environs"
                End If
                If txt = "T-Storms" Then
                    Traduire = "Orages"
                End If
                If txt = "T-Storms Early" Then
                    Traduire = "Orages matinaux"
                End If
                If txt = "T-Storms Fog" Then
                    Traduire = "Orages Brouillard"
                End If
                If txt = "T-Storms Hail Fog" Then
                    Traduire = "Orages Grêle Brouillard"
                End If
                If txt = "T-Storms Hail" Then
                    Traduire = "Orages Grêle"
                End If
                If txt = "T-Storms Haze in Vicinity Hail" Then
                    Traduire = "Orages brume dans les environs Grêle"
                End If
                If txt = "T-Storms Haze in Vicinity" Then
                    Traduire = "Orages brume dans les environs"
                End If
                If txt = "T-Storms Heavy Rain" Then
                    Traduire = "Orages Grosse Pluie"
                End If
                If txt = "T-Storms Heavy Rain Fog" Then
                    Traduire = "Orages Grosse Pluie Brouillard"
                End If
                If txt = "T-Storms Heavy Rain Fog/Mist" Then
                    Traduire = "Orages Grosse pluie Brouillard/brume"
                End If
                If txt = "T-Storms Heavy Rain Hail Fog" Then
                    Traduire = "Orages Grosse Pluie Grêle Brouillard"
                End If
                If txt = "T-Storms Heavy Rain Hail Fog/Mist" Then
                    Traduire = "Orages Grosse Pluie Grêle Brouillard/brume"
                End If
                If txt = "T-Storms Heavy Rain Hail Haze" Then
                    Traduire = "Orages Grosse Pluie Grêle Brume légère"
                End If
                If txt = "T-Storms Heavy Rain Hail" Then
                    Traduire = "Orages Grosse Pluie Grêle"
                End If
                If txt = "T-Storms Heavy Rain Haze" Then
                    Traduire = "Orages Grosse Pluie Brume légère"
                End If
                If txt = "T-Storms Ice Pellets" Then
                    Traduire = "Orages Grêlons"
                End If
                If txt = "T-Storms in Vicinity" Then
                    Traduire = "Orages dans les environs"
                End If
                If txt = "T-Storms in Vicinity Fog" Then
                    Traduire = "Orages dans les environs Brouillard"
                End If
                If txt = "T-Storms in Vicinity Fog/Mist" Then
                    Traduire = "Orages dans les environs Brouillard/brume"
                End If
                If txt = "T-Storms in Vicinity Hail Fog/Mist" Then
                    Traduire = "Orages dans les environs Grêle Brouillard/brume"
                End If
                If txt = "T-Storms in Vicinity Hail Haze" Then
                    Traduire = "Orages dans les environs Grêle Brume légère"
                End If
                If txt = "T-Storms in Vicinity Hail" Then
                    Traduire = "Orages dans les environs Grêle"
                End If
                If txt = "T-Storms in Vicinity Haze" Then
                    Traduire = "Orages dans les environs Brume légère"
                End If
                If txt = "T-Storms Light Rain" Then
                    Traduire = "Orages Légère pluie"
                End If
                If txt = "T-Storms Light Rain Fog" Then
                    Traduire = "Orages Légère pluie Brouillard"
                End If
                If txt = "T-Storms Light Rain Fog/Mist" Then
                    Traduire = "Orages Légère pluie Brouillard/brume"
                End If
                If txt = "T-Storms Light Rain Hail" Then
                    Traduire = "Orages Légère pluie Grêle"
                End If
                If txt = "T-Storms Light Rain Hail Fog" Then
                    Traduire = "Orages Légère pluie Grêle Brouillard"
                End If
                If txt = "T-Storms Light Rain Hail Fog/Mist" Then
                    Traduire = "Orages Légère pluie Grêle Brouillard/brume"
                End If
                If txt = "T-Storms Light Rain Hail Haze" Then
                    Traduire = "Orages Légère pluie Grêle Brume légère"
                End If
                If txt = "T-Storms Light Rain Haze" Then
                    Traduire = "Orages Légère pluie Brume légère"
                End If
                If txt = "T-Storms Rain Fog/Mist" Then
                    Traduire = "Orages Pluie Brouillard/brume"
                End If
                If txt = "T-Storms Rain Hail Fog/Mist" Then
                    Traduire = "Orages Pluie Grêle Brouillard/brume"
                End If
                If txt = "T-Storms Showers in Vicinity" Then
                    Traduire = "Orages Averses dans les environs"
                End If
                If txt = "T-Storms Showers in Vicinity Hail" Then
                    Traduire = "Orages Averses dans les environs Grêle"
                End If
                If txt = "T-Storms Snow" Then
                    Traduire = "Orages Neige"
                End If
                If txt = "Windy" Then
                    Traduire = "Grand vent"
                End If
                If txt = "Windy / Snowy" Then
                    Traduire = "Grand vent / neigeux"
                End If
                If txt = "Windy Rain" Then
                    Traduire = "giboulées"
                End If
                If txt = "Wintry Mix" Then
                    Traduire = "Mélanges pluie/neige"
                End If
                If txt = "Wintry Mix / Wind" Then
                    Traduire = "Mélanges pluie/neige / vent"
                End If

            End Function

        End Class
#End Region

#Region "Audio"
        Public Class Audio

#Region "AudioZone"
            <Serializable()> Public Class AudioZone
                Dim _ID As String
                Dim _Name As String
                Dim _Picture As String
                Dim _DriverID As String
                Dim _Adresse As String
                Dim _Enable As Boolean
                Dim mClass As String = "audiozone"
                Dim _Status As String = "?"
                Dim _File As String

                Public Driver As New Object

                Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As Object)

                Public Property ID() As String
                    Get
                        Return _ID
                    End Get
                    Set(ByVal Value As String)
                        _ID = Value
                    End Set
                End Property

                Public Property Name() As String
                    Get
                        Return _Name
                    End Get
                    Set(ByVal Value As String)
                        _Name = Value
                    End Set
                End Property

                Public Property Picture() As String
                    Get
                        Return _Picture
                    End Get
                    Set(ByVal Value As String)
                        _Picture = Value
                    End Set
                End Property

                Public Property DriverID() As String
                    Get
                        Return _DriverID
                    End Get
                    Set(ByVal Value As String)
                        _DriverID = Value
                    End Set
                End Property

                Public Property Adresse() As String
                    Get
                        Return _Adresse
                    End Get
                    Set(ByVal Value As String)
                        _Adresse = Value
                        If File.Exists(Value) = False Then
                            _Log.AddToLog(Log.TypeLog.ERREUR, "DEVICE AUDIO", "Le fichier foobar n'existe pas: " & Value)
                        End If
                    End Set
                End Property

                Public Property Enable() As Boolean
                    Get
                        Return _Enable
                    End Get
                    Set(ByVal Value As Boolean)
                        _Enable = Value
                    End Set
                End Property

                Public ReadOnly Property TypeClass() As String
                    Get
                        Return mClass
                    End Get
                End Property

                Public Property Status() As String
                    Get
                        Return _Status
                    End Get
                    Set(ByVal value As String)
                        _Status = value
                        RaiseEvent DeviceChanged(_ID, "Status", value)
                    End Set
                End Property

                Public Property Fichier() As String
                    Get
                        Return _File
                    End Get
                    Set(ByVal value As String)
                        _File = value
                    End Set
                End Property

                Public Sub Play()
                    Try
                        If Fichier = "" Then Exit Sub
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /hide", AppWinStyle.Hide)
                        System.Threading.Thread.Sleep(1000)
                        ProcId = Shell(Adresse & " /add " & Fichier, AppWinStyle.Hide)
                        System.Threading.Thread.Sleep(3000)
                        ProcId = Shell(Adresse & " /play", AppWinStyle.Hide)
                        Status = "PLAY"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PLAY: " & ex.Message)
                    End Try
                End Sub

                Public Sub Pause()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /pause", AppWinStyle.Hide)
                        Status = "PAUSE"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PAUSE: " & ex.Message)
                    End Try
                End Sub

                Public Sub [Stop]()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /command:Clear", AppWinStyle.Hide)
                        System.Threading.Thread.Sleep(500)
                        ProcId = Shell(Adresse & " /stop", AppWinStyle.Hide)
                        System.Threading.Thread.Sleep(500)
                        ProcId = Shell(Adresse & " /exit", AppWinStyle.Hide)
                        Status = "STOP"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " STOP: " & ex.Message)
                    End Try
                End Sub

                Public Sub Random()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /random", AppWinStyle.Hide)
                        Status = "RANDOM"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " RANDOM: " & ex.Message)
                    End Try
                End Sub

                Public Sub [Next]()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /next", AppWinStyle.Hide)
                        Status = "NEXT"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " NEXT: " & ex.Message)
                    End Try
                End Sub

                Public Sub Previous()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /previous", AppWinStyle.Hide)
                        Status = "PREVIOUS"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " PREVIOUS: " & ex.Message)
                    End Try
                End Sub

                Public Sub VolumeDown()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /Volume Down", AppWinStyle.Hide)
                        Status = "VOLUME DOWN"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VOLUME DOWN: " & ex.Message)
                    End Try
                End Sub

                Public Sub VolumeUp()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /Volume Up", AppWinStyle.Hide)
                        Status = "VOLUME UP"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VOLUME UP: " & ex.Message)
                    End Try
                End Sub

                Public Sub VolumeMute()
                    Try
                        Dim ProcId As Object
                        ProcId = Shell(Adresse & " /Volume mute", AppWinStyle.Hide)
                        Status = "VOLUME MUTE"
                    Catch ex As Exception
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Erreur " & Me.Name & " VOLUME MUTE: " & ex.Message)
                    End Try
                End Sub
            End Class
#End Region

        End Class
#End Region

#Region "MESURE"
        <Serializable()> Public Class Temperature
            Dim _ID As String
            Dim _Name As String
            Dim _Picture As String
            Dim _DriverID As String
            Dim _Adresse As String
            Dim _Enable As Boolean
            Dim mClass As String = "temperature"
            Dim _Value As String

            Dim x As New Timers.Timer

            Public Driver As Object
            Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As String)

            Public Property ID() As String
                Get
                    Return _ID
                End Get
                Set(ByVal Value As String)
                    _ID = Value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    _Name = Value
                End Set
            End Property

            Public Property Picture() As String
                Get
                    Return _Picture
                End Get
                Set(ByVal Value As String)
                    _Picture = Value
                End Set
            End Property

            Public Property DriverID() As String
                Get
                    Return _DriverID
                End Get
                Set(ByVal Value As String)
                    _DriverID = Value
                End Set
            End Property

            Public Property Adresse() As String
                Get
                    Return _Adresse
                End Get
                Set(ByVal Value As String)
                    _Adresse = Value
                End Set
            End Property

            Public Property Temperature() As String
                Get
                    Return _Value
                End Get
                Set(ByVal value As String)
                    _Value = value
                    RaiseEvent DeviceChanged(_ID, "Temperature", value)
                End Set
            End Property

            Public Property Enable() As Boolean
                Get
                    Return _Enable
                End Get
                Set(ByVal Value As Boolean)
                    _Enable = Value
                    If Value = True Then
                        Dim y As New Thread(AddressOf MAJ)
                        y.Start()
                        x.Interval = 30000
                        AddHandler x.Elapsed, AddressOf TimerTick
                        x.Enabled = True
                    Else
                        x.Enabled = False
                    End If
                End Set
            End Property

            Private Sub TimerTick()
                Dim x As New Thread(AddressOf MAJ)
                x.Start()
                x = Nothing
            End Sub

            Public ReadOnly Property TypeClass() As String
                Get
                    Return mClass
                End Get
            End Property

            Public Sub MAJ()
                Try
                    If Driver IsNot Nothing Then
                        If Driver.isconnect = True Then
                            Dim retour As String = Driver.temp_get_save(_Adresse)
                            Temperature = retour
                            If IsNumeric(Driver.temp_get_save(_Adresse)) = True Then
                                Temperature = Driver.temp_get_save(_Adresse)
                            Else
                                Temperature = "?"
                            End If
                        Else
                            Temperature = "?"
                        End If
                    End If
                Catch ex As Exception
                    _Log.AddToLog(Log.TypeLog.ERREUR, "DEVICE TEMPERATURE", "Erreur Maj " & _Name & " " & ex.Message)
                End Try
            End Sub

            Protected Overrides Sub Finalize()
                x.Enabled = False
                MyBase.Finalize()
            End Sub

        End Class

        <Serializable()> Public Class Contact
            Dim _ID As String
            Dim _Name As String
            Dim _Picture As String
            Dim _DriverID As String
            Dim _Adresse As String
            Dim _Enable As Boolean
            Dim mClass As String = "contact"
            Dim _Status As Boolean

            Public Driver As New Object
            Public Event DeviceChanged(ByVal Id As String, ByVal Reason As String, ByVal Parametre As String)

            Public Property ID() As String
                Get
                    Return _ID
                End Get
                Set(ByVal Value As String)
                    _ID = Value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    _Name = Value
                End Set
            End Property

            Public Property Picture() As String
                Get
                    Return _Picture
                End Get
                Set(ByVal Value As String)
                    _Picture = Value
                End Set
            End Property

            Public Property DriverID() As String
                Get
                    Return _DriverID
                End Get
                Set(ByVal Value As String)
                    _DriverID = Value
                End Set
            End Property

            Public Property Adresse() As String
                Get
                    Return _Adresse
                End Get
                Set(ByVal Value As String)
                    _Adresse = Value
                End Set
            End Property

            Public Property Enable() As Boolean
                Get
                    Return _Enable
                End Get
                Set(ByVal Value As Boolean)
                    _Enable = Value
                End Set
            End Property

            Public Property Status() As Boolean
                Get
                    Return _Status
                End Get
                Set(ByVal value As Boolean)
                    _Status = value
                    RaiseEvent DeviceChanged(_ID, "Status", value)
                End Set
            End Property

            Public ReadOnly Property TypeClass() As String
                Get
                    Return mClass
                End Get
            End Property

        End Class
#End Region

#Region "Web"
        <Serializable()> Public Class PageWeb
            Dim _ID As String
            Dim _Name As String
            Dim _Picture As String
            Dim _DriverID As String
            Dim _Adresse As String
            Dim _Enable As Boolean
            Dim mClass As String = "web"
            Dim _Value As String

            Public Driver As New Object

            Public Property ID() As String
                Get
                    Return _ID
                End Get
                Set(ByVal Value As String)
                    _ID = Value
                End Set
            End Property

            Public Property Name() As String
                Get
                    Return _Name
                End Get
                Set(ByVal Value As String)
                    _Name = Value
                End Set
            End Property

            Public Property Picture() As String
                Get
                    Return _Picture
                End Get
                Set(ByVal Value As String)
                    _Picture = Value
                End Set
            End Property

            Public Property DriverID() As String
                Get
                    Return _DriverID
                End Get
                Set(ByVal Value As String)
                    _DriverID = Value
                End Set
            End Property

            Public Property Adresse() As String
                Get
                    Return _Adresse
                End Get
                Set(ByVal Value As String)
                    _Adresse = Value
                End Set
            End Property

            Public Property Enable() As Boolean
                Get
                    Return _Enable
                End Get
                Set(ByVal Value As Boolean)
                    _Enable = Value
                End Set
            End Property

            Public ReadOnly Property TypeClass() As String
                Get
                    Return mClass
                End Get
            End Property

        End Class

#End Region
    End Class
End Namespace