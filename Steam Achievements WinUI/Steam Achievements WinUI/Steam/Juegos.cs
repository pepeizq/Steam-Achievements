using Herramientas;
using Interfaz;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Steam_Achievements_WinUI.MainWindow;

namespace Steam
{
    public static class Juegos
    {
        private static string dominioImagen = "https://cdn.cloudflare.steamstatic.com";
        private static string dominioIcono = "https://steamcdn-a.akamaihd.net";

        public static async void CargarSv()
        {
            ObjetosVentana.svJuegos.ViewChanging += CargarMasJuegos;
        }

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
                        if (json.Datos.Juegos != null)
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
                }
               
                if (juegos.Count > 0)
                {
                    juegos.Sort(delegate (SteamJuego c1, SteamJuego c2) { return c1.Titulo.CompareTo(c2.Titulo); });

                    SteamCuentayJuegos cuentayJuegos = new SteamCuentayJuegos
                    {
                        Cuenta = cuenta,
                        Juegos = juegos,
                        JuegosComprobados = 50
                    };

                    ObjetosVentana.gvJuegos.Tag = cuentayJuegos;

                    int i = 0;
                    foreach (SteamJuego juego in juegos)
                    {
                        if (i < 50)
                        {
                            SteamCuentayJuego cuentayJuego = new SteamCuentayJuego
                            {
                                Cuenta = cuenta,
                                Juego = juego
                            };

                            Image imagen = new Image
                            {
                                Source = new BitmapImage(new Uri(juego.Imagen)),
                                Stretch = Stretch.UniformToFill,
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Tag = cuentayJuego,
                                MinWidth = 200,
                                MinHeight = 300
                            };

                            imagen.ImageOpened += ImagenCarga;

                            Button2 botonJuego = new Button2
                            {
                                Content = imagen,
                                Tag = cuentayJuego,
                                Padding = new Thickness(5),
                                Background = new SolidColorBrush(Colors.Transparent)
                            };

                            ObjetosVentana.gvJuegos.Items.Add(botonJuego);
                        }

                        i += 1;
                        


                        //--------------------------------------------------------

                        //juego.Logros = logros;
                        //juego.Escaneado = true;

                        //StorageFolder carpetaJugador = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\" + cuenta.ID64);
                        //StorageFile ficheroJuego = null;

                        //try
                        //{
                        //    ficheroJuego = await carpetaJugador.GetFileAsync(juego.ID + ".json");
                        //}
                        //catch
                        //{
                        //    ficheroJuego = await carpetaJugador.CreateFileAsync(juego.ID + ".json");
                        //}

                        //string textoLogros = JsonConvert.SerializeObject(juego, Formatting.Indented);
                        //await FileIO.WriteTextAsync(ficheroJuego, textoLogros);
                    }
                }
            }

            ObjetosVentana.prJuegos.Visibility = Visibility.Collapsed;
        }

        public static void CargarMasJuegos(object sender, ScrollViewerViewChangingEventArgs e)
        {
            SteamCuentayJuegos cuentayJuegos = ObjetosVentana.gvJuegos.Tag as SteamCuentayJuegos;
            List<SteamJuego> juegos = cuentayJuegos.Juegos;

            int i = cuentayJuegos.JuegosComprobados;
            foreach (SteamJuego juego in juegos)
            {
                if (i < cuentayJuegos.JuegosComprobados + 50)
                {
                    SteamCuentayJuego cuentayJuego = new SteamCuentayJuego
                    {
                        Cuenta = cuentayJuegos.Cuenta,
                        Juego = juego
                    };

                    Image imagen = new Image
                    {
                        Source = new BitmapImage(new Uri(juego.Imagen)),
                        Stretch = Stretch.UniformToFill,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Tag = cuentayJuego,
                        MinWidth = 200,
                        MinHeight = 300
                    };

                    imagen.ImageOpened += ImagenCarga;

                    Button2 botonJuego = new Button2
                    {
                        Content = imagen,
                        Tag = cuentayJuego,
                        Padding = new Thickness(5),
                        Background = new SolidColorBrush(Colors.Transparent)
                    };

                    ObjetosVentana.gvJuegos.Items.Add(botonJuego);
                }

                i += 1;
            }
        }

        public static async void ImagenCarga(object sender, RoutedEventArgs e)
        {
            Image imagen = sender as Image;
            SteamCuentayJuego cuentayJuego = imagen.Tag as SteamCuentayJuego;

            List<Logro> logros = await Logros.ComprobarJuego(cuentayJuego.Cuenta.ID64, cuentayJuego.Juego.ID);
       
            if (logros.Count == 0)
            {
                foreach (Button2 botonJuego in ObjetosVentana.gvJuegos.Items)
                {
                    SteamCuentayJuego juegoGv = botonJuego.Tag as SteamCuentayJuego;

                    if (juegoGv.Juego.ID == cuentayJuego.Juego.ID)
                    {
                        ObjetosVentana.gvJuegos.Items.Remove(botonJuego);
                    }
                }        
            }
        }
    }

    //----------------------------------------------

    public class SteamCuentayJuegos
    {
        public SteamCuenta Cuenta { get; set; }
        public List<SteamJuego> Juegos { get; set; }
        public int JuegosComprobados { get; set; }
    }

    public class SteamCuentayJuego
    {
        public SteamCuenta Cuenta { get; set; }
        public SteamJuego Juego { get; set; }
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
