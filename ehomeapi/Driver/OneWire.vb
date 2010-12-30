
Namespace eHomeApi
    Namespace Drivers

        <Serializable()> Public Class OneWire
            '****************************************************************************
            'DECLARATION DES VARIABLES
            '**************************************************************************
            'Variables propres à class 1WIRE
            <NonSerialized()> Private wir_adapter As com.dalsemi.onewire.adapter.DSPortAdapter
            <NonSerialized()> Private adapter_present = 0 '=1 si adapteur présent sinon =0

            Public Event SendMessage(ByVal PluginName As String, ByVal TypeMessage As String, ByVal e_code As String)

            Dim _ID As String
            Dim _IsConnect As Boolean = False
            Dim _Nom As String = "1wire"
            Dim _Version As String = "1.0"
            Dim _Protocol As String = "1wire"
            Dim _Port As String = "1"
            Dim _Commentaire As String = "Driver 1-WIRE v1.0"
            Dim _AutoStart As Boolean = True 'si le service doit être automatiquement démarré
            Dim _Parametres As String = ""
            Dim _IsReady As Boolean = False
            Dim _Flag_Read As Boolean
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
                    Return _Parametres
                End Get
                Set(ByVal value As String)
                    _Parametres = value
                End Set
            End Property

            Public ReadOnly Property IsConnect() As Boolean
                Get
                    Return _IsConnect
                End Get
            End Property
#End Region

            Public Function Start() As String
                'Initialisation de la cle USB 1-WIRE
                'adapteurname = {DS9490B}
                'port = USB1
                Dim retour As String = ""
                Try
                    Dim adapteurname As String = _Parametres
                    'If (adapteurname = "") Then
                    ' wir_adapter = com.dalsemi.onewire.OneWireAccessProvider.getDefaultAdapter
                    'Else
                    wir_adapter = com.dalsemi.onewire.OneWireAccessProvider.getAdapter("{DS9097U_DS9480}", "COM1") 'adapteurname, Port)
                    'End If
                    adapter_present = 1
                    _IsConnect = True
                    retour = "Adapter " & wir_adapter.getAdapterName & " " & wir_adapter.getPortName & " Connect=" & _IsConnect
                Catch ex As Exception
                    _IsConnect = False
                    adapter_present = 0
                    retour = "ERR: Initialisation : " & ex.ToString
                End Try
                Return retour
            End Function

            Public Function [Stop]() As String
                Dim retour As String
                Try
                    wir_adapter.freePort()
                    adapter_present = 0
                    retour = "OK"
                    Return retour
                    _IsConnect = False
                Catch ex As Exception
                    _IsConnect = False
                    retour = "ERR: Close : " & ex.ToString
                    Return retour
                End Try
            End Function

            Public Function Restart() As String
                Dim r As String = [Stop]()
                Restart = Start()
            End Function

            '********************************
            'FONCTIONS PROPRES AU DRIVER
            '********************************

            Public Function temp_get_save(ByVal adresse As String) As String
                ' Renvoi la temperature du capteur X
                Dim resolution As Double = 0.1 'resolution de la temperature : 0.1 ou 0.5
                Dim retour As String = ""
                Dim state As Object
                Dim tc As com.dalsemi.onewire.container.TemperatureContainer
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                If adapter_present Then
                    'Attente si déjà en cours de lecture
                    Do While _Flag_Read = True

                    Loop
                    _Flag_Read = True
                    'demande l'acces exclusif au reseau
                    Try
                        wir_adapter.beginExclusive(False)
                        owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                        If owd.isPresent() Then
                            Try
                                tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer) 'creer la connexion
                                state = tc.readDevice 'lit le capteur
                                tc.setTemperatureResolution(resolution, state) 'modifie la resolution à 0.1 degré (0.5 par défaut)
                                tc.doTemperatureConvert(state) 'converti la valeur obtenu en temperature
                                state = tc.readDevice 'lit la conversion
                                retour = Math.Round(tc.getTemperature(state), 1)
                            Catch ex As Exception
                                retour = "ERR: temp_get : Readdevice : " & ex.ToString
                            End Try
                        Else
                            retour = "ERR: temp_get : Capteur non présent"
                        End If
                        wir_adapter.endExclusive()
                    Catch ex As Exception
                        retour = "ERR: temp_get : " & ex.ToString
                    End Try
                Else
                    retour = "ERR: temp_get : Adaptateur non présent"
                End If
                _Flag_Read = False
                Return retour

            End Function

            Public Function temp_get(ByVal adresse As String, ByVal resolution As Double) As String
                ' Renvoi la temperature du capteur X
                'resolution = Domos.WIR_res --> resolution de la temperature : 0.1 ou 0.5
                Dim retour As String = ""
                Dim state As Object
                Dim result As Boolean = False
                Dim tc As com.dalsemi.onewire.container.TemperatureContainer
                Dim owd As com.dalsemi.onewire.container.OneWireContainer

                If adapter_present Then
                    'demande l'acces exclusif au reseau
                    Try
                        result = wir_adapter.beginExclusive(False)
                        If result Then
                            'wir_adapter.reset()
                            owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                            tc = DirectCast(owd, com.dalsemi.onewire.container.TemperatureContainer) 'creer la connexion
                            If owd.isPresent() Then
                                state = tc.readDevice() 'lit le capteur
                                tc.setTemperatureResolution(resolution, state) 'modifie la resolution à 0.1 degré (0.5 par défaut)
                                tc.doTemperatureConvert(state)
                                state = tc.readDevice 'lit la conversion
                                retour = Math.Round(tc.getTemperature(state), 1)
                            Else
                                retour = "ERR: temp_get : Capteur non présent"
                            End If
                            wir_adapter.endExclusive()
                        Else
                            retour = "ERR: temp_get : Acces Exclusif refusé"
                        End If
                    Catch ex As Exception
                        retour = "ERR: temp_get : " & ex.ToString
                    End Try
                Else
                    retour = "ERR: temp_get : Adaptateur non présent"
                End If
                Return retour
            End Function

            Public Function switch_get(ByVal adresse As String) As String
                ' Renvoie l'etat et l'activité du switch (ex : 100 pour etat=On level=false activity=False)
                Dim retour As String = ""
                Dim state As Object
                Dim owd As com.dalsemi.onewire.container.OneWireContainer12
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state, switch_activity, switch_level
                Try
                    If adapter_present Then
                        If wir_adapter.isPresent(adresse) Then
                            wir_adapter.beginExclusive(True) 'demande l'acces exclusif au reseau
                            owd = wir_adapter.getDeviceContainer(adresse) 'recupere le composant
                            tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer) 'creer la connexion
                            state = tc.readDevice()  'lit les infos du composant
                            switch_state = tc.getLatchState(0, state) 'recup l'etat du switch
                            switch_level = tc.getLevel(0, state) 'recup le level du switch
                            switch_activity = tc.getSensedActivity(0, state) 'recup l'activité du switch
                            If switch_state Then retour = "1" Else retour = "0"
                            If switch_level Then retour = retour & "1" Else retour = retour & "0"
                            If switch_activity Then retour = retour & "1" Else retour = retour & "0"
                            'switch_state = tc.getLatchState(0, state) 'recup l'etat du switch
                            wir_adapter.endExclusive() 'rend l'accés au reseau
                        Else
                            retour = "ERR: switch_get : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: switch_get : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: switch_get : " & ex.Message
                End Try
                Return retour
            End Function

            Public Function switchs_get(ByVal adresse As String) As String
                ' Récupere l'etat et activité d'un multiswitch
                Dim retour As String = ""
                Dim state As Object
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_activity, switch_state
                Try
                    If adapter_present Then
                        If wir_adapter.isPresent(adresse) Then
                            wir_adapter.beginExclusive(True)
                            owd = wir_adapter.getDeviceContainer(adresse)
                            tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                            state = tc.readDevice()
                            Dim number_of_switches = tc.getNumberChannels(state)
                            For i = 0 To (number_of_switches - 1)
                                switch_state = tc.getLatchState(i, state) 'recup l'etat du switch
                                switch_activity = tc.getSensedActivity(i, state) 'recup l'activité du switch
                                If i <> 0 Then retour = retour & "-"
                                If switch_state Then retour = retour & "1" Else retour = retour & "0"
                                If switch_activity Then retour = retour & "1" Else retour = retour & "0"
                            Next
                            wir_adapter.endExclusive()
                        Else
                            retour = "ERR: switchs_get : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: switchs_get : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: switchs_get : " & ex.ToString
                End Try
                Return retour
            End Function

            Public Function switch_switchstate(ByVal adresse As String) As String
                ' Change l'etat d'un switch et renvoi le nouveau etat (ex :0 ==> Off)
                Dim retour As String = ""
                Dim state As Object
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                Try
                    If adapter_present Then
                        If wir_adapter.isPresent(adresse) Then
                            wir_adapter.beginExclusive(True)
                            owd = wir_adapter.getDeviceContainer(adresse)
                            tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                            state = tc.readDevice()
                            switch_state = tc.getLatchState(0, state)
                            If switch_state Then retour = "0" Else retour = "1"
                            tc.setLatchState(0, Not switch_state, False, state)
                            tc.writeDevice(state)
                            wir_adapter.endExclusive()
                        Else
                            retour = "ERR: switch_switchstate : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: switch_switchstate : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: switch_switchstate : " & ex.ToString
                End Try
                Return retour
            End Function

            Public Function switchs_switchstate(ByVal adresse As String, ByVal channel As Integer) As String
                ' Change l'etat d'un switch
                Dim retour As String = ""
                Dim state As Object
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                Try
                    If adapter_present Then
                        If wir_adapter.isPresent(adresse) Then
                            wir_adapter.beginExclusive(True)
                            owd = wir_adapter.getDeviceContainer(adresse)
                            tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                            state = tc.readDevice()
                            Dim number_of_switches = tc.getNumberChannels(state)
                            For i = 0 To (number_of_switches - 1)
                                If i = channel Then
                                    switch_state = tc.getLatchState(i, state)
                                    If i <> 0 Then retour = retour & "-"
                                    If switch_state Then retour = retour & "0" Else retour = retour & "1"
                                    tc.setLatchState(i, Not switch_state, False, state)
                                End If
                            Next
                            tc.writeDevice(state)
                            wir_adapter.endExclusive()
                        Else
                            retour = "ERR: switchs_switchstate : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: switchs_switchstate : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: switchs_switchstate : " & ex.ToString
                End Try
                Return retour
            End Function

            Public Function switch_setstate(ByVal adresse As String, ByVal etat As Boolean) As String
                ' Change l'etat d'un switch et renvoi le nouvel etat
                Dim retour As String = ""
                Dim state As Object
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                Try
                    If adapter_present Then
                        If wir_adapter.isPresent(adresse) Then
                            wir_adapter.beginExclusive(True)
                            owd = wir_adapter.getDeviceContainer(adresse)
                            tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                            state = tc.readDevice()
                            switch_state = tc.getLatchState(0, state)
                            If etat Then retour = "1" Else retour = "0"
                            tc.setLatchState(0, etat, False, state)
                            tc.writeDevice(state)
                            wir_adapter.endExclusive()
                        Else
                            retour = "ERR: switch_setstate : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: switch_setstate : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: switch_setstate : " & ex.ToString
                End Try
                Return retour
            End Function

            Public Function switchs_setstate(ByVal adresse As String, ByVal channel As Integer, ByVal etat As Boolean) As String
                ' Change l'etat du channel x du switch Y et renvoi le nouvel etat
                Dim retour As String = ""
                Dim state As Object
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_state
                Try
                    If adapter_present Then
                        If wir_adapter.isPresent(adresse) Then
                            wir_adapter.beginExclusive(True)
                            owd = wir_adapter.getDeviceContainer(adresse)
                            tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                            state = tc.readDevice()
                            Dim number_of_switches = tc.getNumberChannels(state)
                            For i = 0 To (number_of_switches - 1)
                                If i = channel Then
                                    switch_state = tc.getLatchState(i, state)
                                    If etat Then retour = "1" Else retour = "0"
                                    tc.setLatchState(i, etat, False, state)
                                End If
                            Next
                            tc.writeDevice(state)
                            wir_adapter.endExclusive()
                        Else
                            retour = "ERR: switchs_setstate : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: switchs_setstate : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: switchs_setstate : " & ex.ToString
                End Try
                Return retour
            End Function

            Public Function switch_clearactivity(ByVal adresse As String) As String
                ' Récupere l'etat et activité d'un switch
                Dim retour As String = ""
                Dim state As Object
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim tc As com.dalsemi.onewire.container.SwitchContainer
                Dim switch_activity, switch_state
                Try
                    If adapter_present Then
                        If wir_adapter.isPresent(adresse) Then
                            wir_adapter.beginExclusive(True)
                            owd = wir_adapter.getDeviceContainer(adresse)
                            tc = DirectCast(owd, com.dalsemi.onewire.container.SwitchContainer)
                            state = tc.readDevice()
                            Dim number_of_switches = tc.getNumberChannels(state)
                            For i = 0 To (number_of_switches - 1)
                                switch_state = tc.getLatchState(i, state)
                                switch_activity = tc.getSensedActivity(i, state)
                                If Not switch_state Then
                                    retour = "Switch " & i & " => Activité " & switch_activity & " à False"
                                    'retour = "0"
                                    tc.clearActivity()
                                    tc.readDevice()
                                Else
                                    retour = "1"
                                End If
                            Next
                            wir_adapter.endExclusive()
                        Else
                            retour = "ERR: switch_clearactivity : Capteur non présent"
                        End If
                    Else
                        retour = "ERR: switch_clearactivity : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: switch_clearactivity : " & ex.ToString
                End Try
                Return retour
            End Function

            Public Function counter(ByVal adresse As String, ByVal countera As Boolean) As String
                'recupere la valeur du compteur A (true) ou B (false)
                Dim CounterContainer As com.dalsemi.onewire.container.OneWireContainer1D
                Dim owd As com.dalsemi.onewire.container.OneWireContainer
                Dim retour As String = ""
                Dim counterstate As Long
                Try
                    If adapter_present Then
                        wir_adapter.beginExclusive(True)
                        'owd = wir_adapter.getDeviceContainer(adresse)
                        'CounterContainer = New com.dalsemi.onewire.container.OneWireContainer1D(wir_adapter, adresse)
                        'If countera Then
                        '    counterstate = CounterContainer.readCounter(14)
                        'Else
                        '    counterstate = CounterContainer.readCounter(15)
                        'End If
                        owd = wir_adapter.getDeviceContainer(adresse)
                        CounterContainer = DirectCast(owd, com.dalsemi.onewire.container.OneWireContainer1D)
                        If countera Then
                            counterstate = CounterContainer.readCounter(14)
                        Else
                            counterstate = CounterContainer.readCounter(15)
                        End If
                        wir_adapter.endExclusive()
                        retour = counterstate.ToString
                    Else
                        retour = "ERR: counter : Adaptateur non présent"
                    End If
                Catch ex As Exception
                    retour = "ERR: counter : " & ex.ToString
                End Try
                Return retour
            End Function
        End Class

    End Namespace
End Namespace

