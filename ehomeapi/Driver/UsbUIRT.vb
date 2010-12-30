Imports UsbUirt

Namespace eHomeApi
    Namespace Drivers

        <Serializable()> Public Class UsbUIRT
            '****************************************************************************
            'DECLARATION DES VARIABLES
            '**************************************************************************
            'Private kernel As New eHomeApi
            'Variables propres à class USBUIRT
            <NonSerialized()> Dim mc As Controller 'var pour l'usb uirt
            <NonSerialized()> Private learn_code_modifier As LearnCodeModifier = LearnCodeModifier.ForceStruct
            <NonSerialized()> Private code_format As CodeFormat = CodeFormat.Uuirt
            <NonSerialized()> Dim args As LearnCompletedEventArgs = Nothing     'arguments récup lors de l'apprentissage

            Private last_received_code As String        'dernier code recu

            Public Event SendMessage(ByVal PluginName As String, ByVal TypeMessage As String, ByVal e_code As String) 'raisé quand un code est recu
            '<NonSerialized()> Public Appareils As New List(Of Appareil)

            Dim _ID As String
            Dim _IsConnect As Boolean = False
            Dim _Nom As String = "usbuirt"
            Dim _Version As String = "1.2"
            Dim _Protocol As String = "IR"
            Dim _Port As String = "86"
            Dim _Commentaire As String = "Driver USBUIRT v1.0"
            Dim _AutoStart As Boolean = True 'si le service doit être automatiquement démarré
            Shared _RepertoireDriver As String = "C:\"

            Public Structure ircodeinfo
                Public code_to_send As String
                Public code_to_receive As String
            End Structure

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

            Public Property Commentaire() As String
                Get
                    Return _Commentaire
                End Get
                Set(ByVal value As String)
                    _Commentaire = value
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

            Public Property Parametres() As String
                Get
                    Return _RepertoireDriver
                End Get
                Set(ByVal value As String)
                    _RepertoireDriver = value
                End Set
            End Property
#End Region

         
            Public ReadOnly Property IsConnect() As Boolean
                Get
                    Return _IsConnect
                End Get
            End Property

            Public Function LearnCode() As String
                If _IsConnect = False Then
                    Return "Impossible driver non connecté"
                Else
                    Dim x As ircodeinfo
                    x = wait_for_code()
                    Return x.code_to_send
                End If
            End Function

            Public Function Start() As String
                'cree l'objet pour usbuirt
                Try
                    Me.mc = New Controller
                    'capte les events
                    AddHandler mc.Received, AddressOf handler_mc_received
                    _IsConnect = True
                    Start = ""
                Catch ex As Exception
                    Start = ex.Message
                    _IsConnect = False
                End Try
            End Function

            Public Function [Stop]() As String
                _IsConnect = False
                Me.mc = Nothing
                Return ""
            End Function

            Public Function Restart() As String
                Dim r As String = [Stop]()
                Restart = Start()
            End Function

            '*****************************************************************************
            'GESTION DE L'USBUIRT
            '*****************************************************************************
            'boucle qui attend kon recoive
            <System.STAThread()> _
            Private Function wait_for_code() As ircodeinfo

                'handler
                AddHandler mc.Learning, AddressOf handler_mc_learning
                AddHandler mc.LearnCompleted, AddressOf handler_mc_learning_completed
                Me.args = Nothing

                'lance l'apprentissage
                Try
                    Try
                        Me.mc.LearnAsync(Me.code_format, Me.learn_code_modifier, Me.args)
                        'Me.mc.Learn(Me.code_format, Me.learn_code_modifier, TimeSpan.Zero)
                    Catch ex As Exception
                        MsgBox(ex.Message)
                        Return Nothing
                    End Try

                    'attend que ce soit appris
                    Do While IsNothing(Me.args)
                        'Application.DoEvents()          !!!!!!!!MODIFIE A CAUSE DOEVENTS
                    Loop

                    'c appris !!!
                    RemoveHandler mc.Learning, AddressOf handler_mc_learning
                    RemoveHandler mc.LearnCompleted, AddressOf handler_mc_learning_completed

                Catch ex As Exception
                    'Debug.WriteLine("##Erreur ####3:" & ex.Message)
                    Return Nothing
                End Try

                'retourne le code
                wait_for_code.code_to_send = Me.args.Code
                wait_for_code.code_to_receive = last_received_code
                Return wait_for_code
            End Function

            '*****************************************************************************
            'emet un code infrarouge
            Public Function SendCode(ByVal ir_code As String, ByVal RepeatCount As Integer) As String
                Try
                    mc.Transmit(ir_code, CodeFormat.Uuirt, RepeatCount, TimeSpan.Zero)
                    Return "IR envoyé: " & ir_code & " repeat: " & RepeatCount
                Catch ex As Exception
                    Return "Problème de transmission: " & ex.Message
                End Try
            End Function

            '*****************************************************************************
            'handler code recu
            Private Sub handler_mc_received(ByVal sender As Object, ByVal e As ReceivedEventArgs)
                Debug.WriteLine("Code recu: " & e.IRCode)
                last_received_code = e.IRCode
                'Dim Msg As String = kernel.MakeXMLMessage("usbuirt", "RECEIVE_IR", e.IRCode)
                RaiseEvent SendMessage(_Nom, eHomeApi.IeHomeServer.Reason.MESSAGE_IR, e.IRCode)
            End Sub

            '*****************************************************************************
            'handler en apprentissage
            Private Sub handler_mc_learning(ByVal sender As Object, ByVal e As LearningEventArgs)
                Try
                    Debug.WriteLine("Learning: " & e.Progress & " freq=" & e.CarrierFrequency & " quality=" & e.SignalQuality)
                Catch ex As Exception
                    Debug.WriteLine("Aahhhhhhhhhhhhhhhhhhh")
                End Try
            End Sub

            '*****************************************************************************
            'handler a appris
            Private Sub handler_mc_learning_completed(ByVal sender As Object, ByVal e As LearnCompletedEventArgs)
                args = e
                Debug.WriteLine("Learning completed: " & e.Code)
                'Dim Msg As String = kernel.MakeXMLMessage("usbuirt", "RECEIVE_IR", e.Code)
                RaiseEvent SendMessage(_Nom, eHomeApi.IeHomeServer.Reason.MESSAGE_IR, e.Code)
            End Sub

        End Class

    End Namespace
End Namespace
