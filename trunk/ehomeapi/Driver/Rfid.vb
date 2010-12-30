Imports UsbLibrary

Namespace eHomeApi

    Namespace Drivers

        <Serializable()> Public Class Rfid
            Dim usb1 As UsbLibrary.UsbHidPort
            Dim _ID As String
            Dim _IsConnect As Boolean = False
            Dim _Nom As String = "rfid"
            Dim _Version As String = "1.0"
            Dim _Protocol As String = "RFID"
            Dim _Port As String = "86"
            Dim _AutoStart As Boolean = False
            Dim _Parametres As String = ""

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
                    usb1 = New UsbLibrary.UsbHidPort
                    AddHandler usb1.OnSpecifiedDeviceRemoved, AddressOf usb_OnSpecifiedDeviceRemoved
                    AddHandler usb1.OnDeviceArrived, AddressOf usb_OnDeviceArrived
                    AddHandler usb1.OnDeviceRemoved, AddressOf usb_OnDeviceRemoved
                    AddHandler usb1.OnDataRecieved, AddressOf usb_OnDataRecieved
                    AddHandler usb1.OnSpecifiedDeviceArrived, AddressOf usb_OnSpecifiedDeviceArrived
                    Me.usb1.ProductId = Int32.Parse("1301", System.Globalization.NumberStyles.HexNumber)
                    Me.usb1.VendorId = Int32.Parse("1DA8", System.Globalization.NumberStyles.HexNumber)
                    Me.usb1.CheckDevicePresent()
                    Start = _IsConnect
                Catch ex As Exception
                    usb1 = Nothing
                    Start = ex.Message
                    _IsConnect = False
                End Try
            End Function

            Public Function [Stop]() As String
                usb1 = Nothing
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

            Public Sub New()

            End Sub

            Private Sub usb_OnDeviceArrived(ByVal sender As Object, ByVal e As EventArgs)
                'MsgBox("1")
            End Sub

            Private Sub usb_OnDeviceRemoved(ByVal sender As Object, ByVal e As EventArgs)
                'MsgBox("Device was removed")
            End Sub

            Private Sub usb_OnSpecifiedDeviceArrived(ByVal sender As Object, ByVal e As EventArgs)
                _IsConnect = True
            End Sub

            Private Sub usb_OnSpecifiedDeviceRemoved(ByVal sender As Object, ByVal e As EventArgs)
                'MsgBox("Device déconecté")
            End Sub

            Private Sub usb_OnDataRecieved(ByVal sender As Object, ByVal args As DataRecievedEventArgs)
                Dim different0 As Boolean = False
                Dim rec_data As String = "Data: "
                For Each myData As Byte In args.data
                    If myData <> 0 Then
                        different0 = True
                    End If

                    rec_data += myData.ToString("X") & " "
                Next

                If different0 Then
                    processMirrorData(args.data)
                End If
            End Sub

            Private Sub processMirrorData(ByVal mirrorData As Byte())
                processLaunch(mirrorData)
            End Sub

            Private Sub processLaunch(ByVal mirrorData As Byte())
                If mirrorData(1) = 1 Then
                    'action miroir 
                    If mirrorData(2) = 4 Then
                        'remis à l'endroit 
                        RaiseEvent SendMessage(_Nom, 38, "2|Remise à l'endroit du mir:ror")
                        'MsgBox("Remise à l'endroit du mir:ror")
                    ElseIf mirrorData(2) = 5 Then
                        ' mise à l'envers 
                        RaiseEvent SendMessage(_Nom, 38, "3|Retournement du mir:ror")
                        'MsgBox("Retournement du mir:ror")
                    End If
                ElseIf mirrorData(1) = 2 Then
                    'action ztamp 
                    Dim idZtamp As String = [String].Empty
                    For i As Integer = 3 To 13
                        idZtamp += mirrorData(i).ToString("X2")
                    Next
                    If mirrorData(2) = 1 Then
                        'dépot 
                        RaiseEvent SendMessage(_Nom, 38, "1|" & idZtamp)
                        'MsgBox("ID:" & idZtamp & " - POSE")
                    ElseIf mirrorData(2) = 2 Then
                        ' retrait 
                        RaiseEvent SendMessage(_Nom, 38, "0|" & idZtamp)
                        'MsgBox("ID:" & idZtamp & " - DEPOSE")
                    End If
                Else
                    Console.WriteLine("tiens, une erreur inconnue est survenue...")
                End If
            End Sub
        End Class
    End Namespace
End Namespace