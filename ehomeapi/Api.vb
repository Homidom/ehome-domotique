Namespace eHomeApi

    Public Module Api

        Public Function GenerateGUID() As String
            Dim sGUID As String
            sGUID = System.Guid.NewGuid.ToString()
            GenerateGUID = sGUID
        End Function

        'Liste les méthodes public d'une fonction
        'Nom_de_la_methode|parametre1:Type|parametre2:type...
        Public Function ListMethod(ByVal Objet As Object) As ArrayList
            Dim X As String
            Dim Info As Reflection.MethodInfo
            Dim paraminfos() As Object
            Dim _Tabl As New ArrayList

            For Each Info In Objet.GetType.GetMethods()
                X = (Info.ReturnType.ToString) 'retourne le type string, boolean
                If Info.Attributes = 6 And X = "System.Void" Then 'on prend que les méthodes public
                    X = Info.Name 'Nom de la méthode
                    paraminfos = Info.GetParameters()

                    Dim tabl() As String
                    For i As Integer = 0 To paraminfos.Count - 1
                        tabl = paraminfos(i).ToString.Split(" ")
                        X &= "|" & tabl(1) & ":" & tabl(0)
                    Next

                    _Tabl.Add(X)
                End If
            Next
            Return _Tabl
        End Function

        'Liste les fonctions public d'une fonction
        'Nom_de_la_fonction|Type_retourné
        Public Function ListFunction(ByVal Objet As Object) As ArrayList
            Dim X As String
            Dim Info As Reflection.MethodInfo
            Dim _Tabl As New ArrayList

            For Each Info In Objet.GetType.GetMethods()
                X = (Info.ReturnType.ToString) 'retourne le type string, boolean

                If Info.Attributes = 6 And X <> "System.Void" Then
                    X = Info.Name & ":" & X
                    _Tabl.Add(X)
                End If
            Next
            Return _Tabl
        End Function

        Public Function ListProperty(ByVal OBjet As Object) As ArrayList
            Dim X As String
            Dim Info As Reflection.PropertyInfo
            Dim _Tabl As New ArrayList

            For Each Info In OBjet.GetType.GetProperties
                If Info.Name.ToString = "ID" Or Info.Name.ToString = "DriverID" Or Info.Name.ToString = "Name" Or Info.Name.ToString = "Enable" Or Info.Name.ToString = "TypeClass" Or Info.Name.ToString = "Adresse" Or Info.Name.ToString = "Picture" Then
                    'On prend pas en compte les propriétés communes
                Else
                    X = (Info.Name.ToString) 'retourne le type string, boolean
                    X &= "|" & Info.PropertyType.FullName.Replace("System.", "")
                    _Tabl.Add(X)
                End If
            Next
            Return _Tabl
        End Function
    End Module


End Namespace