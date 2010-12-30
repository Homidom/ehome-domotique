Namespace eHomeApi
#Region "Structure XML"
    '<histo>
    '   <id>mID</id>
    '   <enable>boolean</enable>
    '   <deviceid>0</deviceid>
    '   <propertydevice>0</propertydevice>
    '</histo>
#End Region

    'Historise pour chaque deviceId sa valeur de propertydevice toutes les min pdt une rétention X dans un fichier .csv
    <Serializable()> Public Class Historisation
        Dim _Enable As Boolean
        Dim _Id As String
        Dim _DeviceId As String
        Dim _PropertyDevice As String

        Public Property ID() As String
            Get
                Return _Id
            End Get
            Set(ByVal value As String)
                _Id = value
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
                Return _DeviceId
            End Get
            Set(ByVal value As String)
                _DeviceId = value
            End Set
        End Property

        Public Property PropertyDevice() As String
            Get
                Return _PropertyDevice
            End Get
            Set(ByVal value As String)
                _PropertyDevice = value
            End Set
        End Property
    End Class

End Namespace
