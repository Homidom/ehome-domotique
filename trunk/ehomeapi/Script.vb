Namespace eHomeApi

#Region "Structure XML"
    '<script>
    '   <id>mID</id>
    '   <name>mName</name>
    '   <enable>boolean</enable>
    '   <action>
    '       <typeaction>0</typeaction> !DEVICE
    '       <deviceid>0</deviceid>
    '       <function>0</function>
    '       <value>0</value>
    '   <action>
    '   <action>
    '       <typeaction>1</typeaction> !MAIL
    '       <smtp>0</smtp>
    '       <identification>0</identification>
    '       <login>0</login>
    '       <password>0</password>
    '       <from>0</from>
    '       <to>0</to>
    '       <sujet>0</sujet>
    '       <message>0</message>
    '   <action>
    '   <action>
    '       <typeaction>2</typeaction> !EXIT
    '   <action>
    '   <action>
    '       <typeaction>3</typeaction> !PAUSE
    '       <heure>0</heure>
    '       <minute>0</minute>
    '       <seconde>0</seconde>
    '       <mseconde>0</mseconde>
    '   <action>
    '   <action>
    '       <typeaction>3</typeaction> !SCRIPT VB
    '       <script>0</script>
    '   <action>
    '   <action>
    '       <typeaction>4</typeaction> !IF
    '           <condition>
    '               <typecondition>0</typecondition>  !indique si c sur device ou heure
    '               <itemid>0</itemid>
    '               <parametre>0</parametre>
    '               <value>0</value>
    '               <operateur>EQ</operateur>
    '           </condition>
    '           <then>
    '               <action>
    '                   ...
    '               </action>
    '           </then>
    '           <else>
    '               <action>
    '                   ...
    '               </action>
    '           </else>
    '   <action>
    '</script>
#End Region

    <Serializable()> Public Class Script

        Dim _ID As String 'ID du script
        Dim _Name As String 'nom du script
        Dim _Enable As Boolean 'Activation du script
        Dim _ListAction As New ArrayList 'Liste des actions du script

        Public Property ID() As String
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
        Public Property ListAction() As ArrayList
            Get
                Return _ListAction
            End Get
            Set(ByVal value As ArrayList)
                _ListAction = value
            End Set
        End Property
        Public Sub Run()
            Server.RunScript(_ID)
        End Sub

        '***************************************
        'ACTION
        '***************************************
        'Action Script, lance un script
        <Serializable()> Public Class ClassScript
            Dim mIDScript As Boolean
            Public ReadOnly TypeClass As String = "SCRIPT"

            Public Property IDScript() As Boolean
                Get
                    Return mIDScript
                End Get
                Set(ByVal value As Boolean)
                    mIDScript = value
                End Set
            End Property
            
        End Class

        'Action Device
        <Serializable()> Public Class ClassDevice
            Dim mIDDevice As String
            Dim mFunction As String
            Dim mValue As Object
            Public ReadOnly TypeClass As String = "DEVICE"

            Public Property IDDevice() As String
                Get
                    Return mIDDevice
                End Get
                Set(ByVal value As String)
                    mIDDevice = value
                End Set
            End Property
            Public Property DeviceFunction() As String
                Get
                    Return mFunction
                End Get
                Set(ByVal value As String)
                    mFunction = value
                End Set
            End Property
            Public Property Value() As Object
                Get
                    Return mValue
                End Get
                Set(ByVal value As Object)
                    mValue = value
                End Set
            End Property
        End Class

        'Envoi un mail
        <Serializable()> Public Class ClassEmail
            Public ReadOnly TypeClass As String = "MAIL"
            Dim mMailServerSMTP As String
            Dim mMailServerIdentification As Boolean 'True si besoin d'un login & password
            Dim mMailServerLogin As String
            Dim mMailServerPassword As String
            Dim mFrom As String
            Dim mTo As String
            Dim mSujet As String
            Dim mMessage As String

            Public Property MailServerSMTP() As String
                Get
                    Return mMailServerSMTP
                End Get
                Set(ByVal value As String)
                    mMailServerSMTP = value
                End Set
            End Property
            Public Property MailServerIdentification() As Boolean
                Get
                    Return mMailServerIdentification
                End Get
                Set(ByVal value As Boolean)
                    mMailServerIdentification = value
                End Set
            End Property
            Public Property MailServerLogin() As String
                Get
                    Return mMailServerLogin
                End Get
                Set(ByVal value As String)
                    mMailServerLogin = value
                End Set
            End Property
            Public Property MailServerPassword() As String
                Get
                    Return mMailServerPassword
                End Get
                Set(ByVal value As String)
                    mMailServerPassword = value
                End Set
            End Property
            Public Property From() As String
                Get
                    Return mFrom
                End Get
                Set(ByVal value As String)
                    mFrom = value
                End Set
            End Property
            Public Property A() As String
                Get
                    Return mTo
                End Get
                Set(ByVal value As String)
                    mTo = value
                End Set
            End Property
            Public Property Sujet() As String
                Get
                    Return mSujet
                End Get
                Set(ByVal value As String)
                    mSujet = value
                End Set
            End Property
            Public Property Message() As String
                Get
                    Return mMessage
                End Get
                Set(ByVal value As String)
                    mMessage = value
                End Set
            End Property
            Sub Execute()
                'Dim email As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
                'email.From = New MailAddress(mFrom)
                'email.To.Add(mTo)
                'email.Subject = mSujet
                'email.Body = mMessage
                'Dim mailSender As New System.Net.Mail.SmtpClient()
                ''SmtpMail.SmtpServer = mMailServerSMTP
                'If mMailServerIdentification = True Then
                '    mailSender.Credentials = New Net.NetworkCredential(mMailServerLogin, mMailServerPassword)
                '    '
                '    'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1")
                '    'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mMailServerLogin)
                '    'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", mMailServerPassword)
                'End If
                'mailSender.Send(email)
            End Sub
        End Class

        'Insert an EXIT action into a conditional situation where the intent is to
        'not execute the rest of the script if the condition is not met.
        <Serializable()> Public Class ClassExit
            Public ReadOnly TypeClass As String = "EXIT"

            Sub Execute()
            End Sub
        End Class

        'Insert a C# or Visual Basic code block into the script. Once the code
        'block is added, you can paste or type C# or Visual Basic code into the
        'script.
        <Serializable()> Public Class ClassVBScript
            Dim mScript As String 'Script VB
            Public ReadOnly TypeClass As String = "VBSCRIPT"

            Public Property Script() As String
                Get
                    Return mScript
                End Get
                Set(ByVal value As String)
                    mScript = value
                End Set
            End Property

            Sub Execute()
            End Sub
        End Class

        'Insert a conditional action into the script. For example, you may
        'icon to set house lights to come on if an alarm is triggered.
        <Serializable()> Public Class ClassIf
            'Dim mOrder As Integer 'position dans la liste des actions
            'Dim mListCondition As ArrayList 'liste de condition
            Public ReadOnly TypeClass As String = "IF"
            Dim _ThenListAction As New ArrayList
            Dim _ElseListAction As New ArrayList
            Dim _ListCondition As New ArrayList

            Public Property ThenListAction() As ArrayList
                Get
                    Return _ThenListAction
                End Get
                Set(ByVal value As ArrayList)
                    _ThenListAction = value
                End Set
            End Property
            Public Property ElseListAction() As ArrayList
                Get
                    Return _ElseListAction
                End Get
                Set(ByVal value As ArrayList)
                    _ElseListAction = value
                End Set
            End Property
            Public Property ListCondition() As ArrayList
                Get
                    Return _ListCondition
                End Get
                Set(ByVal value As ArrayList)
                    _ListCondition = value
                End Set
            End Property

            <Serializable()> Public Class Condition
                Public TypeCondition As String
                Public ItemId As String
                Public Parametre As String
                Public Value As Object
                Public Operateur As String
            End Class
            'Public Enum EModeCondition
            '    Time
            '    DeviceToValue
            '    Variable
            'End Enum
            'Public Enum EOpCondition
            '    eAnd
            '    eOR
            '    eNone
            'End Enum

            'Public Function AddCondition(ByVal sModeCondition As EModeCondition, ByVal sVariable As String, ByVal sParametre As Object, ByVal sTypeCondition As ListCondition, ByVal sValueCondition As Object, ByVal sOperateurCondition As EOpCondition) As ClassCondition
            '    Dim objE As New ClassCondition(sModeCondition, sVariable, sParametre, sTypeCondition, sValueCondition, sOperateurCondition)
            '    ListCondition.Add(objE)
            '    AddCondition = objE
            'End Function

            'Public ReadOnly Property ConditionItem(ByVal iIndex As Integer) As ClassCondition
            '    Get
            '        ConditionItem = ListCondition.Item(iIndex)
            '    End Get
            'End Property

            'Public ReadOnly Property Count() As Integer
            '    Get
            '        Count = ListCondition.Count
            '    End Get
            'End Property

            'Public Sub Remove(ByVal sKey As Integer)
            '    ListCondition.RemoveAt(sKey)
            'End Sub

            'Public Sub Execute()
            'End Sub

            'Public Class ClassCondition
            '    Dim mModeCondition As EModeCondition 'type de condition: date&heure, valeur d'un device
            '    Dim mVariable As String 'Device ou variable
            '    Dim mParametre As String 'parametre a vérifier
            '    Dim mTypeCondition As ListCondition '> < =
            '    Dim mValueCondition As Object 'valeur à comparer au parametre
            '    Dim mOperateurCondition As EOpCondition 'And Or None avec le prochaine condition

            '    Public Property ModeCondition() As EModeCondition
            '        Get
            '            Return mModeCondition
            '        End Get
            '        Set(ByVal value As EModeCondition)
            '            mModeCondition = value
            '        End Set
            '    End Property
            '    Public Property Variable() As String
            '        Get
            '            Return mVariable
            '        End Get
            '        Set(ByVal value As String)
            '            mVariable = value
            '        End Set
            '    End Property
            '    Public Property Parametre() As String
            '        Get
            '            Return mParametre
            '        End Get
            '        Set(ByVal value As String)
            '            mParametre = value
            '        End Set
            '    End Property
            '    Public Property TypeCondition() As ListCondition
            '        Get
            '            Return mTypeCondition
            '        End Get
            '        Set(ByVal value As ListCondition)
            '            mTypeCondition = value
            '        End Set
            '    End Property
            '    Public Property ValueCondition() As Object
            '        Get
            '            Return mValueCondition
            '        End Get
            '        Set(ByVal value As Object)
            '            mValueCondition = value
            '        End Set
            '    End Property
            '    Public Property OperateurCondition() As EOpCondition
            '        Get
            '            Return mOperateurCondition
            '        End Get
            '        Set(ByVal value As EOpCondition)
            '            mOperateurCondition = value
            '        End Set
            '    End Property
            '    Public Function IsTrue() As Boolean
            '    End Function
        End Class

        'Action Pause
        <Serializable()> Public Class ClassPause
            Public ReadOnly TypeClass As String = "PAUSE"
            Dim mHeure As Integer
            Dim mMinute As Integer
            Dim mSeconde As Integer
            Dim mMilliSeconde As Integer

            Public Property Heure() As Integer
                Get
                    Return mHeure
                End Get
                Set(ByVal value As Integer)
                    mHeure = value
                End Set
            End Property
            Public Property Minute() As Integer
                Get
                    Return mMinute
                End Get
                Set(ByVal value As Integer)
                    mMinute = value
                End Set
            End Property
            Public Property Seconde() As Integer
                Get
                    Return mSeconde
                End Get
                Set(ByVal value As Integer)
                    mSeconde = value
                End Set
            End Property
            Public Property MilliSeconde() As Integer
                Get
                    Return mMilliSeconde
                End Get
                Set(ByVal value As Integer)
                    mMilliSeconde = value
                End Set
            End Property

            Sub Execute()

            End Sub

        End Class

        'Action Parle un message
        <Serializable()> Public Class ClassVoice
            Public ReadOnly TypeClass As String = "PARLER"
            Dim mMessage As String

            Public Property Message() As String
                Get
                    Return mMessage
                End Get
                Set(ByVal value As String)
                    mMessage = value
                End Set
            End Property
        End Class
    End Class
End Namespace