Imports System.IO

Namespace eHomeApi

    Public Class Log
        Dim _File As String
        Dim _MaxFileSize As Long = 5120000 'en bytes

        Public Property Fichier() As String
            Get
                Return _File
            End Get
            Set(ByVal value As String)
                _File = value
            End Set
        End Property

        Public Property MaxFileSize() As Long
            Get
                Return _MaxFileSize
            End Get
            Set(ByVal value As Long)
                _MaxFileSize = value
            End Set
        End Property

        Public Enum TypeLog
            ERREUR
            INFO
            MESSAGE
        End Enum

        Public Sub AddToLog(ByVal TypLog As TypeLog, ByVal Service As String, ByVal Message As String)
            Try
                Dim Fichier As FileInfo

                'Vérifie si le fichier log existe sinon le crée
                If File.Exists(_File) = False Then
                    File.Create(_File)
                End If

                Fichier = New FileInfo(_File)

                'Vérifie si le fichier est trop gros si oui le supprime
                If Fichier.Length > _MaxFileSize Then
                    File.Delete(_File)
                End If

                'Ecrire le log
                Dim SW As New StreamWriter(_File, True)
                SW.WriteLine(Now.ToString & "|" & TypLog & "|" & Service & "|" & Message)
                SW.Close()
                Console.WriteLine(Now & " " & Message)

                Fichier = Nothing
                SW = Nothing
            Catch ex As Exception

            End Try
        End Sub
    End Class
End Namespace