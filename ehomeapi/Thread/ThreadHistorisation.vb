Imports eHomeApi.eHomeApi
Imports System.IO

'******************************
'Traitement des Historisations
'******************************

Public Class ThreadHistorisation
    Inherits Server

    Dim _File As String

    Public Sub Traite()
        For i As Integer = 0 To m_ListHistorisations.Count - 1
            If m_ListHistorisations.Item(i).enable = True Then
                Dim x As Object = Nothing
                x = ReturnDeviceById(m_ListHistorisations.Item(i).deviceid)
                If x IsNot Nothing Then
                    If x.enable = True Then
                        Dim retour As Object = CallByName(x, m_ListHistorisations.Item(i).propertydevice, CallType.Method, Nothing)
                        Enregistre(x.id, m_ListHistorisations.Item(i).propertydevice, retour)
                    End If
                    x = Nothing
                End If
            End If
        Next
    End Sub

    Private Sub Enregistre(ByVal DeviceID As String, ByVal PropertyDevice As String, ByVal Value As Object)
        'Ecrire le log
        Dim SW As New StreamWriter(_File, True)
        SW.WriteLine(Now.ToString & ";" & DeviceID & ";" & PropertyDevice & ";" & Value.ToString)
        SW.Close()
        SW = Nothing
    End Sub

    Public Sub New()
        Dim flag As Boolean = False
        Dim di As New IO.DirectoryInfo("C:\ehome\data")
        Dim aryFi As IO.FileInfo() = di.GetFiles("*.csv")
        Dim fi As IO.FileInfo

        For Each Fi In aryFi
            Dim Fichier As String = fi.FullName
            Fi = New FileInfo(Fichier)  'on instance ce FileInfo avec comme paramètre le nom du fichier
            Dim _span As TimeSpan
            _span = Now - Fi.CreationTime
            If _span.Days < Server._HistorisationDayMax Then
                _File = Fichier
                flag = True
                Exit For
            End If
        Next

        If flag = False Then
            _File = "C:\ehome\data\histo" & Now.Year & Format(Now.Month, "00") & Format(Now.Day, "00") & ".csv"
            File.Create(_File)
        End If

        di = Nothing
        aryFi = Nothing
        fi = Nothing
    End Sub
End Class
