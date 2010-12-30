
Namespace eHomeApi

    Namespace Drivers

        <Serializable()> Public Class X10
            'Dim frm As New Frmx10
            Dim _ID As String
            Dim _IsConnect As Boolean = False
            Dim _Nom As String = "x10"
            Dim _Version As String = "1.0"
            Dim _Protocol As String = "X10"
            Dim _Port As String = "2"
            Dim _AutoStart As Boolean = False
            Dim _Parametres As String = "0"

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
                Dim retour As String
                Try
                    'frm.Show()
                    'frm.Visible = False
                    Dim _result As Short
                    'AddHandler frm.Axcontrolcm1.X10Event, AddressOf X10Events
                    'AddHandler frm.Axcontrolcm1.X10SingleEvent, AddressOf X10SingleEvent
                    'frm.Axcontrolcm1.comport = _Port
                    '_result = frm.Axcontrolcm1.Init
                    If _result <> 0 Then
                        retour = "NON CONNECTE"
                    Else
                        retour = "CONNECTE"
                    End If
                    Return retour
                Catch ex As Exception
                    Start = "ERREUR: " & ex.Message
                    _IsConnect = False
                End Try
            End Function

            Public Function [Stop]() As String

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

            'Private Sub X10Events(ByVal sender As Object, ByVal e As Axcm11a.__controlcm_X10EventEvent)

            'End Sub

            'Private Sub X10SingleEvent(ByVal sender As Object, ByVal e As Axcm11a.__controlcm_X10SingleEventEvent)

            'End Sub
#End Region

            Protected Overrides Sub Finalize()
                'frm.Axcontrolcm1.Dispose()
                'frm.Close()
                'frm = Nothing
                MyBase.Finalize()
            End Sub
        End Class

    End Namespace
End Namespace

