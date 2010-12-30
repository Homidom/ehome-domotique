Imports eHomeApi.eHomeApi

'******************************
'Traitement des Triggers
'******************************
Public Class ThreadTrigger
    Inherits Server

    Dim _DeviceId As String
    Dim _Reason As String
    Dim _Parametre As Object
    Dim _ListTriggers As ArrayList

    Public Sub Traite()
        For j As Integer = 0 To _ListTriggers.Count - 1
            Dim x As New Trigger
            x = _ListTriggers.Item(j)
            If x.Enable = True Then 'Que les triggers enable
                If x.DeviceId = _DeviceId And x.Status = _Reason Then
                    Select Case x.Condition
                        Case "="
                            If _Parametre = x.Value Then
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Trigger déclenché: " & x.Name)
                                For i As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(i))
                                Next
                                Exit Select
                            End If
                        Case "<="
                            If _Parametre <= x.Value Then
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Trigger déclenché: " & x.Name)
                                For i As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(i))
                                Next
                                Exit Select
                            End If
                        Case ">="
                            If _Parametre >= x.Value Then
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Trigger déclenché: " & x.Name)
                                For i As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(i))
                                Next
                                Exit Select
                            End If
                        Case "<"
                            If _Parametre < x.Value Then
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Trigger déclenché: " & x.Name)
                                For i As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(i))
                                Next
                                Exit Select
                            End If
                        Case ">"
                            If _Parametre > x.Value Then
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Trigger déclenché: " & x.Name)
                                For i As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(i))
                                Next
                                Exit Select
                            End If
                        Case "<>"
                            If _Parametre <> x.Value Then
                                _Log.AddToLog(Log.TypeLog.INFO, "Serveur", "Trigger déclenché: " & x.Name)
                                For i As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(i))
                                Next
                                Exit Select
                            End If
                    End Select
                End If
            End If
        Next
    End Sub

    Sub New(ByVal DeviceId As String, ByVal Reason As String, ByVal Parametre As Object, ByVal ListTriggers As ArrayList)
        _DeviceId = DeviceId
        _Reason = Reason
        _Parametre = Parametre
        _ListTriggers = ListTriggers
    End Sub

    Protected Overrides Sub Finalize()
        _DeviceId = Nothing
        _Reason = Nothing
        _Parametre = Nothing
        _ListTriggers = Nothing
        MyBase.Finalize()

    End Sub
End Class
