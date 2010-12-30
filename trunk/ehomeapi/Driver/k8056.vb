Imports System.IO.Ports

Namespace eHomeApi
    Namespace Drivers

        <Serializable()> Public Class k8056
            Dim _ID As String
            Dim _IsConnect As Boolean = False
            Dim _Nom As String = "k8056"
            Dim _Version As String = "1.0"
            Dim _Protocol As String = "K8056"
            Dim _Port As String = "3"
            Dim _AutoStart As Boolean = False
            Dim _Parametres As String = "0" 'N° de la carte
            Dim rs232 As New System.IO.Ports.SerialPort
            Dim tRelais(7, 7) As Boolean

            Public Event SendMessage(ByVal PluginName As String, ByVal TypeMessage As String, ByVal Value As String)
            '****************************************************************************
            'DECLARATION DES PROPRIETES
            '**************************************************************************
#Region "Property"
            Public Property Id() As String
                Get
                    Return _ID
                End Get
                Set(ByVal value As String)
                    _ID = value
                End Set
            End Property

            Public ReadOnly Property PluginName() As String
                Get
                    Return _Nom
                End Get
            End Property

            Public ReadOnly Property PluginVersion() As String
                Get
                    Return _Version
                End Get
            End Property

            Public Property Protocol() As String
                Get
                    Return _Protocol
                End Get
                Set(ByVal value As String)
                    _Protocol = value
                End Set
            End Property

            Public Property Port() As String
                Get
                    Return _Port
                End Get
                Set(ByVal value As String)
                    _Port = value
                End Set
            End Property

            Public Property AutoStart() As Boolean
                Get
                    Return _AutoStart
                End Get
                Set(ByVal value As Boolean)
                    _AutoStart = value
                End Set
            End Property

            Public ReadOnly Property IsConnect() As Boolean
                Get
                    Return _IsConnect
                End Get
            End Property

            Public Function Start() As String
                'cree l'objet pour usbuirt
                Try
                    With rs232
                        .PortName = _Parametres
                        .Open()
                    End With
                    _IsConnect = True
                    Start = _IsConnect
                Catch ex As Exception
                    Start = "ERREUR: " & ex.Message
                    _IsConnect = False
                End Try
            End Function

            Public Function [Stop]() As String
                Try
                    rs232.Close()
                    _IsConnect = False
                    Return ""
                Catch ex As Exception
                    Return "ERREUR: " & ex.Message
                End Try
            End Function


            Public Function Restart() As String
                Dim r As String = [Stop]()
                Restart = Start()
            End Function

            Public Property Parametres() As String
                Get
                    Return _Parametres
                End Get
                Set(ByVal value As String)
                    _Parametres = value
                End Set
            End Property
#End Region


            Private Sub MAJ(ByVal Carte As Integer, ByVal Relais As Integer, ByVal Value As Boolean)
                If Carte < 0 Then Carte = 0
                If Carte > 7 Then Carte = 7
                If Relais < 0 Then Relais = 0
                If Relais > 7 Then Relais = 7
                tRelais(Carte, Relais) = Value
            End Sub
            Private Function ArretUrgence() As String
                Dim i As Integer
                Dim j As Integer
                Call EnvoyerTrame(1, "E", 1)
                For i = 0 To 7
                    For j = 0 To 7
                        MAJ(i, j, False)
                    Next
                Next
                Return "Arrêt d'urgence effectué"
            End Function

            Private Function ClearRelais(ByVal Carte As Integer, ByVal Relais As Integer) As String
                Call EnvoyerTrame(Carte, "C", Relais)
                MAJ(Carte, Relais, False)
                Return "Relais:" & Carte & "x" & Relais & " Etat:0"
            End Function

            Private Function GetEtatRelais(ByVal Carte As Integer, ByVal Relais As Integer) As Integer
                GetEtatRelais = tRelais(Carte, Relais)
            End Function

            Private Function ResetAll() As String
                Dim i As Integer
                For i = 1 To 255
                    ResetCarte(i)
                Next
                Return "Reset effectué sur toutes les cartes"
            End Function

            Private Function ResetCarte(ByVal Carte As Integer) As String
                Call EnvoyerTrame(Carte, "C", 9)
                Dim i As Integer
                For i = 0 To 7
                    MAJ(Carte, i, False)
                Next
                Return "Carte: " & Carte & " - Reset effectué"
            End Function

            Private Function SetAdresseCarte(ByVal Carte As Integer, ByVal NewAdresse As Integer) As String
                Call EnvoyerTrame(Carte, "A", NewAdresse)
                Return "Carte: " & Carte & " - Nouvelle adresse: " & NewAdresse
            End Function

            Private Function SetCarte(ByVal Carte As Integer) As String
                Call EnvoyerTrame(Carte, "S", 9)
                Dim i As Integer
                For i = 0 To 7
                    MAJ(Carte, i, True)
                Next
                Return "Carte: " & Carte & " - Set effectué"
            End Function

            Private Function SetRelais(ByVal Carte As Integer, ByVal Relais As Integer) As String
                Call EnvoyerTrame(Carte, "S", Relais)
                MAJ(Carte, Relais, True)
                Return "Relais:" & Carte & "x" & Relais & " Etat:1"
            End Function

            Private Function ShowAdrCarte() As String
                Call EnvoyerTrame(1, "D", 1)
                Return "Afficher les adresses des cartes"
            End Function

            'Envoi de la trame à la carte
            Private Sub EnvoyerTrame(ByVal AdresseCarte As String, ByVal Instruction As String, ByVal Adresse As String)
                Dim Entete As String = Chr(13)
                Dim m_Adresse As Integer
                Dim Trame As String = ""
                Dim i As Byte = 0
                Dim checksum As Integer

                m_Adresse = CInt(AdresseCarte)
                'Calcul du CheckSum
                checksum = ((255 - (13 + m_Adresse + Asc(Instruction) + Asc(Adresse)) + 1)) 'complément à 2 des 4 bytes
                Try
                    Do While i < 3
                        Trame = Entete & Chr(m_Adresse) & Instruction & Adresse & Chr(checksum)
                        'Envoyer la trame
                        rs232.Write(Trame)
                        System.Threading.Thread.Sleep(10)
                        i = i + 1
                    Loop
                Catch ex As Exception
                    '                    WriteLog(m_ServiceNom, "Erreur lors de l'envoi de la trame: " & Trame & " " & ex.ToString)
                End Try
            End Sub

        End Class

    End Namespace
End Namespace

