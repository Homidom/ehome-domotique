Namespace eHomeApi

    Public Interface IeHomeServer
        Event MessageArrivedHandler(ByVal msg As AuServerEventMsg)

        Function DeleteDevice(ByVal deviceId As String) As Integer
        Function DeleteDeviceCommand(ByVal deviceId As String, ByVal CmdName As String) As Integer
        Function DeleteMacro(ByVal macroID As String) As Integer
        Function DeleteSchedule(ByVal scheduleId As String) As Integer
        Function DeleteTrigger(ByVal trigId As String) As Integer
        Function DeleteZone(ByVal zoneId As String) As Integer
        Function GetSunSet() As String
        Function GetSunRise() As String
        Function GetTime() As DateTime
        Function GetVersion() As String
        Sub Initialize(ByVal Name As String)
        Function IsDeleteZoneAllowed(ByVal zoneId As String) As Boolean
        Sub MessageToServer(ByVal Message As String)
        Function MoveDeviceToNewZone(ByVal deviceId As String, ByVal newZoneId As String) As Boolean
        Function Ping(ByVal machineName As String) As Boolean
        Function RunMacro(ByVal macroId As String) As Boolean
        Function ReturnDeviceByID(ByVal Id As String) As Object
        Function ReturnDriverByID(ByVal Id As String) As Object
        Function ReturnScheduleByID(ByVal Id As String) As Object
        Function ReturnTriggerByID(ByVal Id As String) As Object
        Function ReturnScriptByID(ByVal Id As String) As Object
        Function ReturnZoneByID(ByVal Id As String) As Object
        Function SaveConfig(ByVal File As String) As String
        Function SaveDevice(ByVal deviceId As String, ByVal name As String, ByVal address As String, ByVal image As String, ByVal enable As Boolean, ByVal adapter As String, ByVal Parametres As String) As String
        Function SaveDeviceCommand(ByVal deviceId As String, ByVal CmdName As String, ByVal CmdData As String, ByVal CmdRepeat As String) As String
        Function SaveScript(ByVal scriptId As String, ByVal name As String, ByVal enable As Boolean, ByVal listaction As ArrayList) As String
        Function SaveSchedule(ByVal scheduleId As String, ByVal name As String, ByVal enable As Boolean, ByVal listscript As ArrayList, ByVal triggertype As Integer, ByVal recurrence As Integer, ByVal startdatetime As String, ByVal jour As String, ByVal asend As Boolean, ByVal enddatetime As String, ByVal sunrisebefore As Boolean, ByVal sunrisebeforetime As String, ByVal sunsetbefore As Boolean, ByVal sunsetbeforetime As String) As String
        Function SaveTrigger(ByVal triggerID As String, ByVal name As String, ByVal enable As Boolean, ByVal deviceid As String, ByVal status As String, ByVal condition As String, ByVal value As Object, ByVal listscript As ArrayList) As String
        Function SaveZone(ByVal zoneID As String, ByVal name As String, ByVal image As String, ByVal ListDevice As ArrayList) As String
        Sub SendCommand(ByVal [function] As String, ByVal deviceId As String)
        Function StartIrLearning() As String

        Structure AuServerEventMsg
            Dim Reason As Reason
            Dim EventTime As DateTime
            Dim Message As String
        End Structure

        Enum Reason
            SHUTTING_DOWN
            CLIENT_CONNECTED
            CLIENT_DISCONNECTED
            INSERT_ZONE
            INSERT_DEVICE
            INSERT_MACRO
            INSERT_ACTION
            INSERT_SCHEDULE
            INSERT_TRIGGER
            INSERT_COMMAND
            INSERT_CFGGRP
            INSERT_CFGITEM
            INSERT_BULK
            DELETE_ZONE
            DELETE_DEVICE
            DELETE_MACRO
            DELETE_ACTION
            DELETE_TRIGGER
            DELETE_SCHEDULE
            DELETE_COMMAND
            DELETE_CFGGRP
            DELETE_CFGITEM
            DELETE_BULK
            CHANGE_ZONE
            CHANGE_DEVICE
            CHANGE_MACRO
            CHANGE_ACTION
            CHANGE_TRIGGER
            CHANGE_COMMAND
            CHANGE_CFGGRP
            CHANGE_CFGITEM
            CHANGE_BULK
            MACRO_STARTED_MANUAL
            MACRO_STARTED_TRIGGER
            MACRO_STARTED_TIMER
            MACRO_STOPPED
            MACRO_PAUSED
            MACRO_RESUMED
            DEVICE_CHANGE_STATUS
            MESSAGE_IR
            MESSAGE_MCE
            MESSAGE_ALARM
            MESSAGE_THERMOSTAT
            MESSAGE_SENSOR
            MESSAGE_ENROLL
            MESSAGE_UNENROLL
            MESSAGE_CAMERA
            MESSAGE_EXTERNAL
            SUNSET 'Coucher soleil
            SUNRISE 'Lever soleil
            UNKNOWN_REASON
            NOTIFICATION
        End Enum

        Property Longitude() As Double
        Property Latitude() As Double
        Property HeureCorrectionCoucher() As Integer
        Property HeureCorrectionLever() As Integer
        Property LogFile() As String
        Property LogMaxSize() As Double
        Property HistorisationDayMax() As Integer

        Property Adapters() As ArrayList
        Property Devices() As ArrayList
        Property Zones() As ArrayList
        Property Schedules() As ArrayList
        Property Scripts() As ArrayList
        Property Triggers() As ArrayList
        Property Menus() As ArrayList
        Property Historisations() As ArrayList
        Property Variables() As ArrayList
    End Interface
End Namespace
