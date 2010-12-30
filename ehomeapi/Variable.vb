Namespace eHomeApi
#Region "Structure XML"
    ' <variable>
    '   <id>12</id>
    '   <name>V0</name>
    '   <value>#</value>
    '   <type>Boolean</type>
    '   <defaultvalue>0</defaultvalue>
#End Region

    Public Class Variable
        Dim _Id As String
        Dim _Name As String
        Dim _Value As Object
        Dim _Type As String
        Dim _DefaultValue As Object

        Public Property ID() As String
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

        Public Property Type() As String
            Get
                Return _Type
            End Get
            Set(ByVal value As String)
                _Type = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return _Value
            End Get
            Set(ByVal value As String)
                _Value = value
            End Set
        End Property

        Public Property DefaultValue() As String
            Get
                Return _DefaultValue
            End Get
            Set(ByVal value As String)
                _DefaultValue = value
            End Set
        End Property
    End Class

End Namespace
