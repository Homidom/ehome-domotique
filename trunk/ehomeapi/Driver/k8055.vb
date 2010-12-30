Namespace eHomeApi

    Namespace Drivers

        <Serializable()> Public Class k8055
            Dim _ID As String
            Dim _IsConnect As Boolean = False
            Dim _Nom As String = "k8055"
            Dim _Version As String = "1.0"
            Dim _Protocol As String = "K8055"
            Dim _Port As String = "3"
            Dim _AutoStart As Boolean = False
            Dim _Parametres As String = "0" 'N° de la carte

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
                    Dim h As Long
                    Dim carte As Long = _Parametres
                    h = OpenDevice(carte)
                    Select Case h
                        Case 0, 1, 2, 3
                            Return 0
                            Start = "Card " + Str(h) + " connected"
                            _IsConnect = True
                        Case -1
                            Return -1
                            Start = "Card " + Str(carte) + " not found"
                            _IsConnect = False
                    End Select
                Catch ex As Exception
                    Start = "ERREUR: " & ex.Message
                    _IsConnect = False
                End Try
            End Function

            Public Function [Stop]() As String
                CloseDevice()
                _IsConnect = False
                Return ""
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

            Private Declare Function OpenDevice Lib "k8055d.dll" (ByVal CardAddress As Long) As Long
            Private Declare Sub CloseDevice Lib "k8055d.dll" ()
            Private Declare Sub WriteAllDigital Lib "k8055d.dll" (ByVal Data As Long)
            Private Declare Sub ClearDigitalChannel Lib "k8055d.dll" (ByVal Channel As Long)
            Private Declare Sub ClearAllDigital Lib "k8055d.dll" ()
            Private Declare Sub SetDigitalChannel Lib "k8055d.dll" (ByVal Channel As Long)
            Private Declare Sub SetAllDigital Lib "k8055d.dll" ()
            Private Declare Function ReadDigitalChannel Lib "k8055d.dll" (ByVal Channel As Long) As Boolean
            Private Declare Function ReadAllDigital Lib "k8055d.dll" () As Long
            Private Declare Function ReadAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long) As Long
            Private Declare Sub ReadAllAnalog Lib "k8055d.dll" (ByVal Data1 As Long, ByVal Data2 As Long)
            Private Declare Sub OutputAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long, ByVal Data As Long)
            Private Declare Sub OutputAllAnalog Lib "k8055d.dll" (ByVal Data1 As Long, ByVal Data2 As Long)
            Private Declare Sub ClearAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long)
            Private Declare Function ReadCounter Lib "k8055d.dll" (ByVal CounterNr As Long) As Long
            Private Declare Sub SetAllAnalog Lib "k8055d.dll" ()
            Private Declare Sub ClearAllAnalog Lib "k8055d.dll" ()
            Private Declare Sub ResetCounter Lib "k8055d.dll" (ByVal CounterNr As Long)
            Private Declare Sub SetAnalogChannel Lib "k8055d.dll" (ByVal Channel As Long)
            Private Declare Sub SetCounterDebounceTime Lib "k8055d.dll" (ByVal CounterNr As Long, ByVal DebounceTime As Long)


            '    '*********************************************************
            '    'PROPRE AU SERVICE
            '    '*********************************************************
            Public Sub ClearAllAnalogique()
                ClearAllAnalog()
            End Sub
            Public Sub ClearAllBinaire()
                ClearAllDigital()
            End Sub

            Public Sub ClearAnalogiqueChannel(ByVal Channel As Long)
                ClearAnalogChannel(Channel)
            End Sub

            Public Sub ClearBinaireChannel(ByVal Channel As Long)
                ClearDigitalChannel(Channel)
            End Sub

            Public Sub OutputAllAnalogique(ByVal Data1 As Long, ByVal Data2 As Long)
                OutputAllAnalog(Data1, Data2)
            End Sub

            Public Sub OutputAnalogiqueChannel(ByVal Channel As Long, ByVal Data As Long)
                OutputAnalogChannel(Channel, Data)
            End Sub

            Public Sub ReadAllAnalogique(ByVal Data1 As Long, ByVal Data2 As Long)
                ReadAllAnalog(Data1, Data2)
            End Sub

            Public Function ReadAllBinaire() As Long
                Return ReadAllDigital
            End Function

            Public Function ReadAnalogiqueChannel(ByVal Channel As Long) As Long
                Return ReadAnalogChannel(Channel)
            End Function

            Public Function ReadCompter(ByVal CounterNr As Long) As Long
                Return ReadCounter(CounterNr)
            End Function

            Public Function ReadBinaireChannel(ByVal Channel As Long) As Boolean
                Return ReadDigitalChannel(Channel)
            End Function

            Public Sub ResetCompteur(ByVal CounterNr As Long)
                ResetCounter(CounterNr)
            End Sub

            Public Sub SetAllAnalogique()
                SetAllAnalog()
            End Sub

            Public Sub SetAllBinaire()
                SetAllDigital()
            End Sub

            Public Sub SetAnalogiqueChannel(ByVal Channel As Long)
                SetAnalogChannel(Channel)
            End Sub

            Public Sub SetCompteurDebounceTime(ByVal CounterNr As Long, ByVal DebounceTime As Long)
                SetCounterDebounceTime(CounterNr, DebounceTime)
            End Sub

            Public Sub SetBinaireChannel(ByVal Channel As Long)
                SetDigitalChannel(Channel)
            End Sub

            Public Sub WriteAllBinaire(ByVal Channel As Long)
                WriteAllDigital(Channel)
            End Sub

        End Class

    End Namespace
End Namespace
