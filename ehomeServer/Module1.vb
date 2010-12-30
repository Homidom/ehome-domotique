Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Http
Imports System.Runtime.Remoting.Lifetime
Imports eHomeApi.eHomeApi

Module Module1
    Dim obj As Server = New Server()

    Sub Main()
        Try

            Console.WriteLine("******************************")
            Console.WriteLine("DEMARRAGE DU SERVEUR**********")
            Console.WriteLine("******************************")
            Console.WriteLine(" ")

            Console.WriteLine(Now & " Chargement de la configuration")
            'Chargement de la config
            Console.WriteLine(Now & obj.LoadConfig("C:\ehome\config\"))

            'Démarrage du serviceWeb
            Console.WriteLine(Now & " ")
            Console.WriteLine(Now & " Start ServiceWeb")
            Dim chnl As New HttpChannel(obj.PortTCP)
            ChannelServices.RegisterChannel(chnl, False)
            LifetimeServices.LeaseTime = Nothing
            RemotingServices.Marshal(obj, "RemoteObjectServer.soap")
            Console.WriteLine(Now & " ")
            Console.WriteLine(Now & " ServiceWeb Démarré sur port:" & obj.PortTCP)

            Console.WriteLine("******************************")
            Console.WriteLine("SERVEUR DEMARRE **************")
            Console.WriteLine("******************************")
            Console.WriteLine(" ")

            If Console.ReadLine = "end" Then
                End
            End If
        Catch ex As Exception
            Console.WriteLine(Now & " ERREUR " & ex.Message)
        End Try
    End Sub

End Module
