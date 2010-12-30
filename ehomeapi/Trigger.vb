Namespace eHomeApi

#Region "Structure XML"
    '<trigger>
    '   <id>ID</id>
    '   <name>Name</name>
    '   <enable>boolean</enable>
    '   <deviceid>..</deviceid>
    '   <status>..</status>
    '   <condition>EQ</condition>
    '   <value>0</value>
    '   <scripts>
    '       <scriptid>0</scriptid>
    '       <scriptid>1</scriptid>
    '       <scriptid>2</scriptid>
    '   </scripts>
    '</trigger>
#End Region

    <Serializable()> Public Class Trigger
        Dim _ID As String
        Dim _Name As String
        Dim _Enable As Boolean
        Dim _DeviceID As String
        Dim _Status As String
        Dim _Condition As String
        Dim _Value As Object
        Dim _ListScript As New ArrayList

        Public Property Id() As String
            Get
                Return _ID
            End Get
            Set(ByVal value As String)
                _ID = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property

        Public Property Enable() As Boolean
            Get
                Return _Enable
            End Get
            Set(ByVal value As Boolean)
                _Enable = value
            End Set
        End Property

        Public Property DeviceId() As String
            Get
                Return _DeviceID
            End Get
            Set(ByVal value As String)
                _DeviceID = value
            End Set
        End Property

        Public Property Status() As String
            Get
                Return _Status
            End Get
            Set(ByVal value As String)
                _Status = value
            End Set
        End Property

        Public Property Condition() As String
            Get
                Return _Condition
            End Get
            Set(ByVal value As String)
                _Condition = value
            End Set
        End Property

        Public Property Value() As Object
            Get
                Return _Value
            End Get
            Set(ByVal value As Object)
                _Value = value
            End Set
        End Property

        Public Property ListScript() As ArrayList
            Get
                Return _ListScript
            End Get
            Set(ByVal value As ArrayList)
                _ListScript.Clear()
                _ListScript = value
            End Set
        End Property
    End Class

End Namespace
