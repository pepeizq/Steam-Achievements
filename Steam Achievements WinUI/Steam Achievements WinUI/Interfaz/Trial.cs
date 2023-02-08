using System.Collections.Generic;
using Windows.Services.Store;
using Windows.System;
using System;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;

namespace Interfaz
{
    public static class Trial
    {
        public static async Task<bool> Detectar()
        {
            bool enTrial = false;

            IReadOnlyList<User> usuarios = await User.FindAllAsync();

            if (usuarios != null)
            {
                if (usuarios.Count > 0)
                {
                    User usuario = usuarios[0];
                    StoreContext contexto = StoreContext.GetForUser(usuario);
                    StoreAppLicense licencia = await contexto.GetAppLicenseAsync();

                    if (licencia.IsActive == true && licencia.IsTrial == false)
                    {
                        enTrial = false;           
                    }
                    else
                    {
                        enTrial = true;
                    }           
                }
            }

            return enTrial;
        }

        public static async void BotonAbrirCompra(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?ProductId=9MZZX81TDT20"));
        }
    }
}
