using CommunityToolkit.WinUI.UI.Controls;
using Herramientas;
using Interfaz;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.Resources;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Steam_Achievements_WinUI.MainWindow;

namespace Steam
{
    public static class Juegos
    {
        private static string dominioImagen = "https://cdn.cloudflare.steamstatic.com";
        private static string dominioIcono = "https://steamcdn-a.akamaihd.net";

        public static async void Cargar(object sender, RoutedEventArgs e)
        {
            Button2 boton = sender as Button2;
            SteamCuenta cuenta = boton.Tag as SteamCuenta;

            ResourceLoader recursos = new ResourceLoader();

            Pestañas.Visibilidad(ObjetosVentana.gridJuegos);
            Pestañas.CrearSeparador(2);

            string cuentaPestaña = cuenta.Nombre;

            if (cuentaPestaña.Length > 30)
            {
                cuentaPestaña = cuentaPestaña.Remove(30, cuentaPestaña.Length - 30);
                cuentaPestaña = cuentaPestaña + "...";
            }

            cuentaPestaña = cuentaPestaña + " • " + recursos.GetString("Games");

            Pestañas.CreadorItems(cuenta.Avatar, cuentaPestaña, 3);

            //-------------------------------------------------------

            ObjetosVentana.gvJuegos.Items.Clear();
            ObjetosVentana.prJuegos.Visibility = Visibility.Visible;

            await Task.Delay(100);

            string html = await Decompiladores.CogerHtml("https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + cuenta.ID64 + "&include_appinfo=1&include_played_free_games=1");

            if (html != null)
            {
                List<SteamJuego> juegos = new List<SteamJuego>();

                SteamJuegosAPI json = JsonConvert.DeserializeObject<SteamJuegosAPI>(html);

                if (json != null) 
                { 
                    if (json.Datos != null)
                    {
                        if (json.Datos.Juegos.Count > 0)
                        {
                            foreach (SteamJuegosAPIJuego juego in json.Datos.Juegos)
                            {
                                string id = juego.ID;
                                bool añadir = true;

                                if (juegos.Count > 0)
                                {                                  
                                    int k = 0;
                                    while (k < juegos.Count)
                                    {
                                        if (juegos[k].ID == juego.ID)
                                        {
                                            añadir = false;
                                        }

                                        k += 1;
                                    }
                                }

                                if (añadir == true)
                                {
                                    SteamJuego juegoAñadir = new SteamJuego
                                    {
                                        ID = juego.ID,
                                        Titulo = juego.Titulo,
                                        Imagen = dominioImagen + "/steam/apps/" + juego.ID + "/library_600x900.jpg",
                                        Icono = dominioIcono + "/steamcommunity/public/images/apps/" + juego.ID + "/" + juego.Icono + ".jpg",
                                        Escaneado = false
                                    };

                                    juegos.Add(juegoAñadir);
                                }
                            }
                        }
                    }
                }

                if (juegos.Count > 0)
                {
                    Logros.ComprobarCarpetaUsuario(cuenta.ID64);

                    juegos.Sort(delegate (SteamJuego c1, SteamJuego c2) { return c1.Titulo.CompareTo(c2.Titulo); });

                    foreach (SteamJuego juego in juegos)
                    {


                        ImageEx imagen = new ImageEx
                        {
                            Source = juego.Imagen,
                            IsCacheEnabled = true,
                            EnableLazyLoading = true,
                            CornerRadius = new CornerRadius(5)
                        };

                        ObjetosVentana.gvJuegos.Items.Add(imagen);
                    }
                }
            }

            ObjetosVentana.prJuegos.Visibility = Visibility.Collapsed;
        }
    }

    //----------------------------------------------

    public class SteamJuego
    {
        public string ID { get; set; }
        public string Titulo { get; set; }
        public string Imagen { get; set; }
        public string Icono { get; set; }
        public bool Escaneado { get; set; }
        public List<Logro> Logros { get; set; }
    }

    //----------------------------------------------

    public class SteamJuegosAPI
    {
        [JsonProperty("response")]
        public SteamJuegosAPIDatos Datos { get; set; }
    }

    public class SteamJuegosAPIDatos
    {
        [JsonProperty("game_count")]
        public string CantidadJuegos { get; set; }

        [JsonProperty("games")]
        public List<SteamJuegosAPIJuego> Juegos { get; set; }
    }

    public class SteamJuegosAPIJuego
    {
        [JsonProperty("appid")]
        public string ID { get; set; }

        [JsonProperty("name")]
        public string Titulo { get; set; }

        [JsonProperty("img_icon_url")]
        public string Icono { get; set; }
    }
}
