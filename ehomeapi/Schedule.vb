Namespace eHomeApi

#Region "Structure XML"
    '<schedule>
    '   <id>mID</id>
    '   <name>mName</name>
    '   <enable>boolean</enable>
    '   <triggertype>0</triggertype>
    '   <recurrence>0<recurrence>
    '   <startdatetime>mstartdattime</startdatetime>
    '   <jour>00000000</jour>
    '   <asend>boolean</end>
    '   <enddatetime>menddatetime</enddatetime>
    '   <sunrisebefore>msunrisebefore</sunrisebefore>
    '   <sunrisebeforetime>hh:mm:ss</sunrisebefore>
    '   <sunsetbefore>msunsetbefore</sunsetbefore>
    '   <sunsetbeforetime>hh:mm:ss</sunsetbeforetime>
    '   <lastschedule>yy/dd/mm hh:mm:ss</lastschedule>
    '   <scripts>
    '       <scriptid>0</scriptid>
    '       <scriptid>1</scriptid>
    '       <scriptid>2</scriptid>
    '   </scripts>
    '</schedule>
#End Region

    <Serializable()> Public Class Schedule
        Dim _ID As String 'ID du schedule
        Dim _Name As String 'nom du schedule
        Dim _Enable As Boolean 'Activation du schedule
        Dim _TriggerType As EnumScheduleTriggerType 'type de trigger 
        Dim _RecurrencePatternList As EnumScheduleTriggerRecPat 'Recurrence
        Dim _StartDateTime As DateTime 'Date de début
        Dim _Jour(6) As Boolean 'Jour à prendre en compte
        Dim _AsEnd As Boolean 'Si fin
        Dim _EndDateTime As DateTime 'Si fin oui, date de fin
        Dim _SunRiseBefore As Boolean 'si true before si false after
        Dim _SunRiseBeforeTime As DateTime
        Dim _SunSetBefore As Boolean 'si true before si false after
        Dim _SunsetBeforeTime As DateTime
        Dim _LastSchedule As DateTime 'dernière valeur du schedule qui sert dans le calcul de récurrence
        Dim _ListScript As New ArrayList
        Public State As Boolean

        Public Enum EnumScheduleTriggerType
            Custom
            SunRise
            SunSet
        End Enum
        Public Enum EnumScheduleTriggerRecPat
            Jour
            Semaine
            Mois
            Annee
        End Enum

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

        Public Property ListScript() As ArrayList
            Get
                Return _ListScript
            End Get
            Set(ByVal value As ArrayList)
                _ListScript = value
            End Set
        End Property

        Public Property TriggerType() As EnumScheduleTriggerType
            Get
                Return _TriggerType
            End Get
            Set(ByVal value As EnumScheduleTriggerType)
                _TriggerType = value
            End Set
        End Property

        Public Property Jour(ByVal NbJour As Integer) As Boolean
            Get
                Return _Jour(NbJour)
            End Get
            Set(ByVal value As Boolean)
                _Jour(NbJour) = value
            End Set
        End Property

        Public Property RecurrencePatternList() As EnumScheduleTriggerRecPat
            Get
                Return _RecurrencePatternList
            End Get
            Set(ByVal value As EnumScheduleTriggerRecPat)
                _RecurrencePatternList = value
            End Set
        End Property

        Public Property StartDateTime() As DateTime
            Get
                Return _StartDateTime
            End Get
            Set(ByVal value As DateTime)
                _StartDateTime = value
            End Set
        End Property

        Public Property AsEnd() As Boolean
            Get
                Return _AsEnd
            End Get
            Set(ByVal value As Boolean)
                _AsEnd = value
            End Set
        End Property

        Public Property EndDateTime() As DateTime
            Get
                Return _EndDateTime
            End Get
            Set(ByVal value As DateTime)
                _EndDateTime = value
            End Set
        End Property

        Public Property SunRiseBefore() As Boolean
            Get
                Return _SunRiseBefore
            End Get
            Set(ByVal value As Boolean)
                _SunRiseBefore = value
            End Set
        End Property

        Public Property SunRiseBeforeTime() As DateTime
            Get
                Return _SunRiseBeforeTime
            End Get
            Set(ByVal value As DateTime)
                _SunRiseBeforeTime = value
            End Set
        End Property

        Public Property SunSetBefore() As Boolean
            Get
                Return _SunSetBefore
            End Get
            Set(ByVal value As Boolean)
                _SunSetBefore = value
            End Set
        End Property

        Public Property SunsetBeforeTime() As DateTime
            Get
                Return _SunsetBeforeTime
            End Get
            Set(ByVal value As DateTime)
                _SunsetBeforeTime = value
            End Set
        End Property

        Public Property LastSchedule() As DateTime
            Get
                Return _LastSchedule
            End Get
            Set(ByVal value As DateTime)
                _LastSchedule = value
            End Set
        End Property

    End Class

End Namespace
