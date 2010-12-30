Namespace eHomeApi

    <Serializable()> Public Class Menu

        Dim _Id As String
        Dim _Name As String
        Dim _Image As String
        Dim _SubMenu As New ArrayList

        Public Property Id() As String
            Get
                Return _Id
            End Get
            Set(ByVal value As String)
                _Id = value
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

        Public Property Image() As String
            Get
                Return _Image
            End Get
            Set(ByVal value As String)
                _Image = value
            End Set
        End Property

        Public Property SubMenu() As ArrayList
            Get
                Return _SubMenu
            End Get
            Set(ByVal value As ArrayList)
                _SubMenu = value
            End Set
        End Property
    End Class

    <Serializable()> Public Class SubMenu
        Dim _Id As String
        Dim _Name As String
        Dim _Image As String
        Dim _Background As String
        Dim _Color As Double
        Dim _Rows As Integer
        Dim _ListDevice As New ArrayList
        Dim _ListScript As New ArrayList

        Public Property Id() As String
            Get
                Return _Id
            End Get
            Set(ByVal value As String)
                _Id = value
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

        Public Property Image() As String
            Get
                Return _Image
            End Get
            Set(ByVal value As String)
                _Image = value
            End Set
        End Property

        Public Property BackGround() As String
            Get
                Return _Background
            End Get
            Set(ByVal value As String)
                _Background = value
            End Set
        End Property

        Public Property Color() As Double
            Get
                Return _Color
            End Get
            Set(ByVal value As Double)
                _Color = value
            End Set
        End Property

        Public Property Rows() As Integer
            Get
                Return _Rows
            End Get
            Set(ByVal value As Integer)
                _Rows = value
            End Set
        End Property

        Public Property ListDevice() As ArrayList
            Get
                Return _ListDevice
            End Get
            Set(ByVal value As ArrayList)
                _ListDevice = value
            End Set
        End Property

        Public Property ListScript() As ArrayList
            Get
                Return _ListScript
            End Get
            Set(ByVal value As ArrayList)
                _ListScript = value
            End Set
        End Property
    End Class

End Namespace
