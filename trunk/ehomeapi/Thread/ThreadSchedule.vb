Imports eHomeApi.eHomeApi


'******************************
'Traitement des Schedules
'******************************
Public Class ThreadSchedule
    Inherits Server

    Dim _ScheduleId As String
    Dim _Schedules As ArrayList

    Public Sub Traite()
        For i As Integer = 0 To _Schedules.Count - 1
            If _Schedules.Item(i).id = _ScheduleId Then
                Dim x As New Schedule
                x = _Schedules.Item(i)
                If x.Enable = True Then
                    Select Case x.TriggerType 'Suivant type de trigger

                        'Custom
                        Case Schedule.EnumScheduleTriggerType.Custom 'Custom
                            Dim retour As Boolean = ReturnPatternListCustom(x)
                            If retour = True Then
                                _Log.AddToLog(Log.TypeLog.MESSAGE, "Serveur", "Schedule détecté ID: " & x.ID)
                                For j As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(j))
                                Next
                            End If
                            retour = Nothing
                            'Levé Soleil
                        Case Schedule.EnumScheduleTriggerType.SunRise 'Levé soleil
                            Dim retour As Boolean = ReturnPatternListSunRise(x)
                            If retour = True Then
                                _Log.AddToLog(Log.TypeLog.MESSAGE, "Serveur", "Schedule détecté ID: " & x.ID)
                                For j As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(j))
                                Next
                            End If
                            retour = Nothing
                            'Couché de soleil
                        Case Schedule.EnumScheduleTriggerType.SunSet 'Couché de soleil
                            Dim retour As Boolean = ReturnPatternListSunSet(x)
                            If retour = True Then
                                _Log.AddToLog(Log.TypeLog.MESSAGE, "Serveur", "Schedule détecté ID: " & x.ID)
                                For j As Integer = 0 To x.ListScript.Count - 1
                                    RunScript(x.ListScript.Item(j))
                                Next
                            End If
                            retour = Nothing
                    End Select
                End If
                x = Nothing
                Exit Sub
            End If
        Next

    End Sub

    Private Function ReturnPatternListCustom(ByVal Schedule As Schedule) As Boolean

        'on vérifie que s'il y a une fin qu'on est toujours inférieur
        If Schedule.AsEnd = True Then
            If Now > Schedule.EndDateTime Then
                Return False
                Exit Function
            End If
        End If

        Select Case Schedule.RecurrencePatternList
            'Au jour
            Case Schedule.EnumScheduleTriggerRecPat.Jour

                'on vérifie que le jour actuel ne correspond pas au LastSchedule
                If Schedule.LastSchedule.ToShortDateString <> Now.ToShortDateString Then
                    If Schedule.StartDateTime.Hour = Now.Hour And Schedule.StartDateTime.Minute = Now.Minute And Now.Second = 0 Then
                        Schedule.LastSchedule = Now
                        Schedule.State = True
                        Return True
                        Exit Function
                    End If
                End If

                'A la semaine
            Case eHomeApi.Schedule.EnumScheduleTriggerRecPat.Semaine
                If Schedule.Jour(0) = True And Now.DayOfWeek = DayOfWeek.Monday Or Schedule.Jour(1) = True And Now.DayOfWeek = DayOfWeek.Tuesday Or Schedule.Jour(2) = True And Now.DayOfWeek = DayOfWeek.Wednesday Or Schedule.Jour(3) = True And Now.DayOfWeek = DayOfWeek.Thursday Or Schedule.Jour(4) = True And Now.DayOfWeek = DayOfWeek.Friday Or Schedule.Jour(5) = True And Now.DayOfWeek = DayOfWeek.Saturday Or Schedule.Jour(6) = True And Now.DayOfWeek = DayOfWeek.Sunday Then
                    If Schedule.StartDateTime.Hour = Now.Hour And Schedule.StartDateTime.Minute = Now.Minute And Now.Second = 0 Then
                        Schedule.LastSchedule = Now
                        Schedule.State = True
                        Return True
                        Exit Function
                    End If
                End If

                'Au mois
            Case eHomeApi.Schedule.EnumScheduleTriggerRecPat.Mois
                If Schedule.LastSchedule.Month <> Now.Month Then
                    'Par rapport à un jour précis
                    If Schedule.Jour(0) = True And Now.DayOfWeek = DayOfWeek.Monday Or Schedule.Jour(1) = True And Now.DayOfWeek = DayOfWeek.Tuesday Or Schedule.Jour(2) = True And Now.DayOfWeek = DayOfWeek.Wednesday Or Schedule.Jour(3) = True And Now.DayOfWeek = DayOfWeek.Thursday Or Schedule.Jour(4) = True And Now.DayOfWeek = DayOfWeek.Friday Or Schedule.Jour(5) = True And Now.DayOfWeek = DayOfWeek.Saturday Or Schedule.Jour(6) = True And Now.DayOfWeek = DayOfWeek.Sunday Then
                        If Schedule.StartDateTime.Hour = Now.Hour And Schedule.StartDateTime.Minute = Now.Minute And Now.Second = 0 Then
                            Schedule.LastSchedule = Now
                            Schedule.State = True
                            Return True
                            Exit Function
                        End If
                    Else
                        'Par rapport à une date précise
                        If Schedule.StartDateTime.Day = Now.Day And Schedule.StartDateTime.Hour = Now.Hour And Schedule.StartDateTime.Minute = Now.Minute And Now.Second = 0 Then
                            Schedule.LastSchedule = Now
                            Schedule.State = True
                            Return True
                            Exit Function
                        End If
                    End If
                End If

                'A l'année
            Case eHomeApi.Schedule.EnumScheduleTriggerRecPat.Annee
                If Schedule.LastSchedule.Year <> Now.Year Then
                    'Par rapport à une date précise
                    If Schedule.StartDateTime.Month = Now.Month And Schedule.StartDateTime.Day = Now.Day And Schedule.StartDateTime.Hour = Now.Hour And Schedule.StartDateTime.Minute = Now.Minute And Now.Second = 0 Then
                        Schedule.LastSchedule = Now
                        Schedule.State = True
                        Return True
                        Exit Function
                    End If
                End If
        End Select
    End Function

    Private Function ReturnPatternListSunRise(ByVal Schedule As Schedule) As Boolean
        Dim _datetime As DateTime

        If Schedule.SunRiseBefore = True Then 'temps avant le soleil
            _datetime = heurop(var_soleil_lever2, Schedule.SunRiseBeforeTime, 1)
        Else
            'temps après le soleil
            _datetime = heurop(var_soleil_lever2, Schedule.SunRiseBeforeTime, 0)
        End If

        If _datetime.Hour = Now.Hour And _datetime.Minute = Now.Minute And Now.Second = 0 Then
            Schedule.LastSchedule = Now
            Schedule.State = True
            Return True
            Exit Function
        End If
    End Function

    Private Function ReturnPatternListSunSet(ByVal Schedule As Schedule) As Boolean
        Dim _datetime As DateTime

        If Schedule.SunSetBefore = True Then 'temps avant le soleil
            _datetime = heurop(var_soleil_coucher2, Schedule.SunsetBeforeTime, 1)
        Else
            'temps après le soleil
            _datetime = heurop(var_soleil_coucher2, Schedule.SunsetBeforeTime, 0)
        End If

        If _datetime.Hour = Now.Hour And _datetime.Minute = Now.Minute And Now.Second = 0 Then
            Schedule.LastSchedule = Now
            Schedule.State = True
            Return True
            Exit Function
        End If
    End Function

    'pour une addition operation=0, pour une soustraction operation=1
    Private Function heurop(ByVal heure1 As String, ByVal heure2 As String, ByVal operation As Integer) As String
        Dim somme
        Dim diff
        Dim retour As String = ""
        If operation = 0 Then
            somme = Val(DatePart("h", heure1)) * 3600 + Val(DatePart("n", heure1)) * 60 + Val(DatePart("s", heure1)) + Val(DatePart("h", heure2)) * 3600 + Val(DatePart("n", heure2)) * 60 + Val(DatePart("s", heure2))
            retour = Str(somme \ 3600) + ":" + Str((somme - (somme \ 3600) * 3600) \ 60) + ":" _
            + Str(somme - ((somme \ 3600) * 3600) - ((somme - (somme \ 3600) * 3600) \ 60) * 60)
        ElseIf operation = 1 Then
            diff = Val(DatePart("h", heure1)) * 3600 + Val(DatePart("n", heure1)) * 60 + Val(DatePart("s", heure1)) - Val(DatePart("h", heure2)) * 3600 - Val(DatePart("n", heure2)) * 60 - Val(DatePart("s", heure2))
            retour = Str(diff \ 3600) + ":" + Str((diff - (diff \ 3600) * 3600) \ 60) + ":" + _
           Str(diff - ((diff \ 3600) * 3600) - ((diff - (diff \ 3600) * 3600) \ 60) * 60)
        End If
        somme = Nothing
        diff = Nothing
        Return retour
    End Function

    Sub New(ByVal Schedules As ArrayList, ByVal scheduleId As String)
        _ScheduleId = scheduleId
        _Schedules = Schedules
    End Sub

    Protected Overrides Sub Finalize()
        _Schedules = Nothing
        _ScheduleId = Nothing
        MyBase.Finalize()
        _Schedules = Nothing
        _ScheduleId = Nothing
    End Sub
End Class
