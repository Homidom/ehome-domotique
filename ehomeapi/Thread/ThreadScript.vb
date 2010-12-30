Imports eHomeApi.eHomeApi
Imports System.Net.Mail
Imports System.Speech

Public Class ThreadScript
    Inherits Server
    Dim _Script As Script

    Sub TraiteAction(ByVal ListAction As ArrayList)
        Try
            For j As Integer = 0 To ListAction.Count - 1
                'on traite les actions du script
                Select Case ListAction.Item(j).TypeClass
                    Case "DEVICE"
                        Dim x As Script.ClassDevice = ListAction.Item(j)
                        For i As Integer = 0 To m_ListDevices.Count - 1
                            If x.IDDevice = m_ListDevices.Item(i).id Then
                                If x.Value <> "" Then
                                    Dim retour As Object = CallByName(m_ListDevices.Item(i), x.DeviceFunction, CallType.Method, x.Value)
                                Else
                                    Dim retour As Object = CallByName(m_ListDevices.Item(i), x.DeviceFunction, CallType.Method)
                                End If
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script DEVICE:" & m_ListDevices.Item(i).name & " Fonction:" & x.DeviceFunction & " Value:" & x.Value)

                                Exit For
                            End If
                        Next
                        x = Nothing
                    Case "SCRIPT"
                        Dim x As Script.ClassScript = ListAction.Item(j)
                        For i As Integer = 0 To m_ListScripts.Count - 1
                            If x.IDScript = m_ListScripts.Item(i).id Then
                                RunScript(x.IDScript)
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script:" & m_ListScripts.Item(i).name & " Lancé")
                                Exit For
                            End If
                        Next
                        x = Nothing
                    Case "EXIT"
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script EXIT")
                        Exit For
                    Case "PAUSE"
                        Dim x As Script.ClassPause = ListAction.Item(j)
                        Dim t As DateTime = DateTime.Now
                        t = t.AddHours(x.Heure)
                        t = t.AddMinutes(x.Minute)
                        t = t.AddSeconds(x.Seconde)
                        t = t.AddMilliseconds(x.MilliSeconde)
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script PAUSE: " & x.Heure & ":" & x.Minute & ":" & x.Seconde & ":" & x.MilliSeconde)
                        x = Nothing
                        Do While DateTime.Now < t

                        Loop
                    Case "MAIL"
                        Dim x As Script.ClassEmail = ListAction.Item(j)
                        Dim email As System.Net.Mail.MailMessage = New System.Net.Mail.MailMessage()
                        email.From = New MailAddress(x.From)
                        email.To.Add(x.A)
                        email.Subject = x.Sujet
                        email.Body = x.Message
                        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script MAIL")
                        Dim mailSender As New System.Net.Mail.SmtpClient(x.MailServerSMTP)

                        'SmtpMail.SmtpServer = mMailServerSMTP
                        If x.MailServerIdentification = True Then
                            mailSender.Credentials = New Net.NetworkCredential(x.MailServerLogin, x.MailServerPassword)
                            '
                            'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1")
                            'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", mMailServerLogin)
                            'email.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", mMailServerPassword)
                        End If
                        mailSender.Send(email)
                        x = Nothing
                        email = Nothing
                        mailSender = Nothing
                    Case "PARLER"
                        Dim x As Script.ClassVoice = ListAction.Item(j)
                        Dim texte As String = x.Message
                        'remplace les balises par la valeur
                        texte = texte.Replace("{time}", Now.ToShortTimeString)
                        texte = texte.Replace("{date}", Now.ToLongDateString)
                        Dim i1 As Integer = InStr(texte, "{")
                        If i1 > 0 Then
                            Dim i2 As Integer = InStr(texte, "}")
                            Dim extract As String = Mid(texte, i1 + 1, i2 - i1)
                            If i2 > 0 Then
                                Dim i3 As Integer = InStr(extract, ".")
                                If i3 > 0 Then
                                    Dim _dev As String = Mid(extract, 1, i3 - 1)
                                    Dim _prty As String = Mid(extract, i3 + 1, Len(extract) - (i3 + 1))
                                    If _dev <> "" And _prty <> "" Then
                                        _dev = UCase(_dev)
                                        _prty = UCase(_prty)
                                        For i4 As Integer = 0 To m_ListDevices.Count - 1
                                            If UCase(m_ListDevices.Item(i4).name) = _dev Then
                                                Dim retour As Object = (CallByName(m_ListDevices.Item(i4), _prty, CallType.Method, Nothing))
                                                texte = Mid(texte, 1, i1 - 1) & " " & retour & Mid(texte, i2 + 1, Len(texte) - i2 + 1)
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End If
                        End If
                        Try
                            Dim lamachineaparler As New Speech.Synthesis.SpeechSynthesizer
                            _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script PARLER")
                            With lamachineaparler
                                .SelectVoice("ScanSoft Virginie_Dri40_16kHz")
                                '.SetOutputToWaveFile("C:\tet.wav")
                                '.SetOutputToWaveFile(File)
                                .SpeakAsync(texte)
                            End With
                            x = Nothing
                            texte = Nothing
                            lamachineaparler = Nothing
                        Catch ex As Exception
                            _Log.AddToLog(Log.TypeLog.ERREUR, "ThreadScript", "Erreur action PARLER: " & ex.Message)
                        End Try
                    Case "IF"
                        Dim x As Script.ClassIf = ListAction.Item(j)
                        Dim _ResultIf As Boolean = True
                        For i As Integer = 0 To x.ListCondition.Count - 1
                            Dim x2 As Script.ClassIf.Condition
                            x2 = x.ListCondition.Item(i)
                            Select Case x2.TypeCondition
                                Case "TEMPS"
                                    Dim val As Object = Nothing
                                    Select Case x2.Parametre
                                        Case "Date Heure"
                                            val = x2.Value
                                        Case "Lever Soleil"
                                            '?? UTILE?
                                        Case "Coucher Soleil"
                                            '?? UTILE?
                                        Case "Nuit"
                                            val = Server.IsNuit
                                        Case "Jour"
                                            val = Server.IsJour
                                    End Select
                                    Select Case x2.Operateur
                                        Case "="
                                            If val = x2.Value Then
                                                _ResultIf = _ResultIf And True
                                            Else
                                                _ResultIf = _ResultIf And False
                                            End If
                                        Case ">="
                                            If val >= x2.Value Then
                                                _ResultIf = _ResultIf And True
                                            Else
                                                _ResultIf = _ResultIf And False
                                            End If
                                        Case "<="
                                            If val <= x2.Value Then
                                                _ResultIf = _ResultIf And True
                                            Else
                                                _ResultIf = _ResultIf And False
                                            End If
                                        Case ">"
                                            If val > x2.Value Then
                                                _ResultIf = _ResultIf And True
                                            Else
                                                _ResultIf = _ResultIf And False
                                            End If
                                        Case "<"
                                            If val < x2.Value Then
                                                _ResultIf = _ResultIf And True
                                            Else
                                                _ResultIf = _ResultIf And False
                                            End If
                                        Case "<>"
                                            If val <> x2.Value Then
                                                _ResultIf = _ResultIf And True
                                            Else
                                                _ResultIf = _ResultIf And False
                                            End If
                                    End Select
                                Case "DEVICE"
                                    Dim obj As Object = Nothing
                                    For idx As Integer = 0 To m_ListDevices.Count - 1
                                        If m_ListDevices.Item(idx).id = x2.ItemId Then
                                            obj = m_ListDevices.Item(idx)
                                            Exit For
                                        End If
                                    Next
                                    If obj IsNot Nothing Then
                                        Dim retour As Object = (CallByName(obj, x2.Parametre, CallType.Method, Nothing))
                                        Select Case x2.Operateur
                                            Case "="
                                                If retour = x2.Value Then
                                                    _ResultIf = _ResultIf And True
                                                Else
                                                    _ResultIf = _ResultIf And False
                                                End If
                                            Case ">="
                                                If retour >= x2.Value Then
                                                    _ResultIf = _ResultIf And True
                                                Else
                                                    _ResultIf = _ResultIf And False
                                                End If
                                            Case "<="
                                                If retour <= x2.Value Then
                                                    _ResultIf = _ResultIf And True
                                                Else
                                                    _ResultIf = _ResultIf And False
                                                End If
                                            Case ">"
                                                If retour > x2.Value Then
                                                    _ResultIf = _ResultIf And True
                                                Else
                                                    _ResultIf = _ResultIf And False
                                                End If
                                            Case "<"
                                                If retour < x2.Value Then
                                                    _ResultIf = _ResultIf And True
                                                Else
                                                    _ResultIf = _ResultIf And False
                                                End If
                                            Case "<>"
                                                If retour <> x2.Value Then
                                                    _ResultIf = _ResultIf And True
                                                Else
                                                    _ResultIf = _ResultIf And False
                                                End If
                                        End Select
                                    End If
                            End Select
                            'THEN
                            If _ResultIf = True Then
                                TraiteAction(x.ThenListAction)
                                'ELSE
                            Else
                                TraiteAction(x.ElseListAction)
                            End If
                            x2 = Nothing
                        Next

                        x = Nothing

                End Select
            Next
        Catch ex As Exception
            _Log.AddToLog(Log.TypeLog.ERREUR, "ThradScript", "Erreur TraiteAction: " & ex.Message)
        End Try
    End Sub

    Sub Lance()
        TraiteAction(_Script.ListAction)
        _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Script Terminé")
    End Sub

    Public Sub New(ByVal Script As Script)
        _Script = Script
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        _Script = Nothing
    End Sub
End Class
