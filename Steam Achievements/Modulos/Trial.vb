Imports Windows.Services.Store
Imports Windows.Storage
Imports Windows.System

Module Trial

    Public Async Function Detectar() As Task(Of Boolean)

        Dim config As ApplicationDataContainer = ApplicationData.Current.LocalSettings

        Dim usuarios As IReadOnlyList(Of User) = Await User.FindAllAsync

        If Not usuarios Is Nothing Then
            If usuarios.Count > 0 Then
                Dim usuario As User = usuarios(0)

                Dim contexto As StoreContext = StoreContext.GetForUser(usuario)
                Dim licencia As StoreAppLicense = Await contexto.GetAppLicenseAsync

                If licencia.IsActive = True And licencia.IsTrial = False Then
                    config.Values("Estado_App") = 1
                Else
                    config.Values("Estado_App") = 0
                End If
            End If
        End If

        If config.Values("Estado_App") = 1 Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Async Sub ComprarAppClick(sender As Object, e As RoutedEventArgs)

        Dim usuarios As IReadOnlyList(Of User) = Await User.FindAllAsync

        If Not usuarios Is Nothing Then
            If usuarios.Count > 0 Then
                Dim usuario As User = usuarios(0)

                Dim contexto As StoreContext = StoreContext.GetForUser(usuario)
                Await contexto.RequestPurchaseAsync("9NSD0JRNLMB7")
            End If
        End If

    End Sub

End Module
