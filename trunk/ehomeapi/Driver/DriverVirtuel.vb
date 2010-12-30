Namespace eHomeApi

    Namespace Drivers

        <Serializable()> Public Class DriverVirtuel
            Dim m_Name As String = "virtuel"
            Dim m_Description As String = "Virtuel"
            Dim m_Port As String = "N/A"
            Dim m_Protocole = "N/A"
            Dim _Version As String = "1.0"
            Dim m_StartAuto As Boolean = True
            Dim _Protocol As String = "Virtuel"
            Dim _parametres As String = ""
            Dim m_ID As String

            Public Event SendMessage(ByVal PluginName As String, ByVal TypeMessage As String, ByVal Value As String)

            Public ReadOnly Property PluginName() As String
                Get
                    Return m_Name
                End Get
            End Property
            Public ReadOnly Property PluginVersion() As String
                Get
                    Return _Version
                End Get
            End Property
            Public Property Parametres() As String
                Get
                    Return _parametres
                End Get
                Set(ByVal value As String)
                    _parametres = value
                End Set
            End Property

            Public Property ID() As String
                Get
                    Return m_ID
                End Get
                Set(ByVal value As String)
                    m_ID = value
                End Set
            End Property
            Public Property Port() As String
                Get
                    Return m_Port
                End Get
                Set(ByVal value As String)
                    m_Port = value
                End Set
            End Property
            Public Property Protocol() As String
                Get
                    Return _Protocol
                End Get
                Set(ByVal value As String)
                    _Protocol = value
                End Set
            End Property
            Public Property AutoStart() As Boolean
                Get
                    Return m_StartAuto
                End Get
                Set(ByVal value As Boolean)
                    m_StartAuto = value
                    If value = True Then

                    End If
                End Set
            End Property

            Public Function Start() As String
                Dim retour As String = ""
                retour = "START"
                Return retour
            End Function

            Public Function [Stop]() As String
                Dim retour As String
                retour = "OK"
                Return retour
            End Function

            Public Function Restart() As String
                Dim r As String = [Stop]()
                Restart = Start()
            End Function

        End Class
    End Namespace
End Namespace