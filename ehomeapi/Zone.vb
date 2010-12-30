Namespace eHomeApi
    '   <zone>
    '       <id>0</id>
    '       <name>Nom</name>
    '       <deviceid>0</deviceid>
    '       <deviceid>1</deviceid>
    '   </zone>
    <Serializable()> Public Class Zone
        Dim _Id As String
        Dim _Name As String
        Dim _ListDeviceId As New ArrayList
        Dim _Image As String

        Public Property ID() As String
            Get
                Return _Id
            End Get
            Set(ByVal value As String)
                _Id = value
            End Set
        End Property
        Public Property Image() As String
            Get
                Return _Image
            End Get
            Set(ByVal value As String)
                _Image = value
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

        Public Property ListDeviceId() As ArrayList
            Get
                Return _ListDeviceId
            End Get
            Set(ByVal value As ArrayList)
                _ListDeviceId = value
            End Set
        End Property
    End Class

End Namespace
