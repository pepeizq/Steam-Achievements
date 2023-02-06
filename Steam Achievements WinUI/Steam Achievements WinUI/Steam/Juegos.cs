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
using Windows.Storage;
using Windows.UI;
using static Steam_Achievements_WinUI.MainWindow;

namespace Steam
{
    public static class Juegos
    {
        private static string dominioImagen = "https://cdn.cloudflare.steamstatic.com";
        private static string dominioIcono = "https://steamcdn-a.akamaihd.net";

        public static void Cargar()
        {
            ObjetosVentana.tbJuegosBuscador.TextChanged += BuscadorBuscaAsync;
            ObjetosVentana.svJuegos.ViewChanging += CargarMasJuegos;
        }

        public static async void BuscadorBuscaAsync(object sender, TextChangedEventArgs e)
        {
            await Task.Yield();

            TextBox tb = sender as TextBox;

            if (tb.Text.Trim().Length > 3)
            {
                tb.IsEnabled = false;

                SteamCuentayJuegos cuentayJuegos = ObjetosVentana.gvJuegos.Tag as SteamCuentayJuegos;
                List<SteamJuego> resultados = new List<SteamJuego>();

                foreach (SteamJuego juego in cuentayJuegos.Juegos)
                {
                    if (juego.Titulo.ToLower().Contains(tb.Text.ToLower().Trim()) == true)
                    {
                        resultados.Add(juego);
                    }
                }

                if (resultados.Count > 0) 
                {
                    resultados.Sort(delegate (SteamJuego c1, SteamJuego c2) { return c1.Titulo.CompareTo(c2.Titulo); });

                    ObjetosVentana.gvJuegos.Items.Clear();

                    foreach (SteamJuego juego in resultados)
                    {
                        bool añadir = true;

                        if (ObjetosVentana.gvJuegos.Items.Count > 0)
                        {
                            foreach (Button2 boton in ObjetosVentana.gvJuegos.Items)
                            {
                                SteamCuentayJuego cuentayJuego = boton.Tag as SteamCuentayJuego;

                                if (juego.ID == cuentayJuego.Juego.ID)
                                {
                                    añadir = false;
                                    break;
                                }
                            }
                        }

                        if (añadir == true)
                        {
                            ObjetosVentana.gvJuegos.Items.Add(await BotonEstilo(cuentayJuegos.Cuenta, juego));
                        }                        
                    }
                }

                tb.IsEnabled = true;
            }
            else
            {

            }
        }

        public static async void CargarLista(object sender, RoutedEventArgs e)
        {
            await Task.Yield();

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
            ObjetosVentana.spJuegosBuscador.Visibility = Visibility.Visible;
            ObjetosVentana.tbJuegosBuscador.IsEnabled = false;

            //-------------------------------------------------------

            ObjetosVentana.gvJuegos.Items.Clear();
            ObjetosVentana.tTipJuegos.IsOpen = true;
            ObjetosVentana.tTipJuegos.Subtitle = recursos.GetString("LoadingGames");

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
                                            Escaneado = false,
                                            Logros = null
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
                        JuegosComprobados = 100
                    };

                    ObjetosVentana.gvJuegos.Tag = cuentayJuegos;

                    int i = 0;
                    while (i < 100)
                    {
                        ObjetosVentana.gvJuegos.Items.Add(await BotonEstilo(cuenta, juegos[i]));
                        i += 1;
                    }                
                }
            }

            ObjetosVentana.tbJuegosBuscador.IsEnabled = true;
            ObjetosVentana.tTipJuegos.IsOpen = false;
        }

        public static async void CargarMasJuegos(object sender, ScrollViewerViewChangingEventArgs e)
        {
            if (ObjetosVentana.tbJuegosBuscador.Text.Length < 4)
            {
                ScrollViewer sv = sender as ScrollViewer;

                if (sv.ScrollableHeight < 400 || sv.VerticalOffset == sv.ScrollableHeight)
                {
                    ResourceLoader recursos = new ResourceLoader();

                    ObjetosVentana.tTipJuegos.IsOpen = true;
                    ObjetosVentana.tTipJuegos.Subtitle = recursos.GetString("LoadingGames");

                    SteamCuentayJuegos cuentayJuegos = ObjetosVentana.gvJuegos.Tag as SteamCuentayJuegos;
                    List<SteamJuego> juegos = cuentayJuegos.Juegos;

                    int i = cuentayJuegos.JuegosComprobados;

                    while (i < cuentayJuegos.JuegosComprobados + 100)
                    {
                        bool añadir = true;

                        if (ObjetosVentana.gvJuegos.Items.Count > 0)
                        {
                            foreach (Button2 boton in ObjetosVentana.gvJuegos.Items)
                            {
                                SteamCuentayJuego cuentayJuego = boton.Tag as SteamCuentayJuego;

                                if (juegos[i].ID == cuentayJuego.Juego.ID)
                                {
                                    añadir = false;
                                    break;
                                }
                            }
                        }

                        if (añadir == true)
                        {
                            ObjetosVentana.gvJuegos.Items.Add(await BotonEstilo(cuentayJuegos.Cuenta, juegos[i]));
                        }

                        i += 1;
                    }

                    cuentayJuegos.JuegosComprobados = cuentayJuegos.JuegosComprobados + 100;
                    ObjetosVentana.gvJuegos.Tag = cuentayJuegos;

                    ObjetosVentana.tTipJuegos.IsOpen = false;
                }
            }   
        }

        public static async Task<Button2> BotonEstilo(SteamCuenta cuenta, SteamJuego juego)
        {
            if (juego != null)
            {
                SteamCuentayJuego cuentayJuego = new SteamCuentayJuego
                {
                    Cuenta = cuenta,
                    Juego = juego
                };

                await Task.Run(async () =>
                {
                    juego = await LeerJuegoFichero(cuentayJuego);
                });

                if (juego != null)
                {
                    bool cargar = false;

                    if (juego.Escaneado == true)
                    {
                        if (juego.Logros != null)
                        {
                            cargar = true;
                        }                       
                    }

                    if (juego.Escaneado == false)
                    {
                        cargar = true;
                    }

                    if (cargar == true) 
                    {
                        StackPanel sp = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            Padding = new Thickness(10),
                            VerticalAlignment = VerticalAlignment.Stretch
                        };

                        Image imagen = new Image
                        {
                            Source = new BitmapImage(new Uri(juego.Imagen)),
                            Stretch = Stretch.Uniform,
                            Tag = cuentayJuego,
                            MinHeight = 250
                        };

                        imagen.ImageOpened += ImagenCarga;
                        imagen.ImageFailed += ImagenFalla;

                        sp.Children.Add(imagen);

                        TextBlock tb = new TextBlock
                        {
                            Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                            Margin = new Thickness(0, 5, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Right
                        };

                        sp.Children.Add(tb);

                        Button2 botonJuego = new Button2
                        {
                            Content = sp,
                            Tag = cuentayJuego,
                            Padding = new Thickness(0),
                            Background = new SolidColorBrush(Colors.Transparent),
                            BorderThickness = new Thickness(0)
                        };

                        botonJuego.PointerEntered += Animaciones.EntraRatonBoton2;
                        botonJuego.PointerExited += Animaciones.SaleRatonBoton2;

                        return botonJuego;
                    }                    
                }               
            }

            return null;
        }

        public static async void ImagenCarga(object sender, RoutedEventArgs e)
        {
            Image imagen = sender as Image;
            SteamCuentayJuego cuentayJuego = imagen.Tag as SteamCuentayJuego;

            List<Logro> logros = new List<Logro>();

            if (cuentayJuego.Juego.Escaneado == false)
            {
                await Task.Run(async () =>
                {
                    logros = await Logros.ComprobarJuego(cuentayJuego.Cuenta.ID64, cuentayJuego.Juego.ID);
                });

                cuentayJuego.Juego.Logros = logros;
                cuentayJuego.Juego.Escaneado = true;

                ActualizarJuegoFichero(cuentayJuego);
            }

            if (logros.Count == 0)
            {
                foreach (Button2 botonJuego in ObjetosVentana.gvJuegos.Items)
                {
                    SteamCuentayJuego juegoGv = botonJuego.Tag as SteamCuentayJuego;

                    if (juegoGv.Juego.ID == cuentayJuego.Juego.ID)
                    {
                        ObjetosVentana.gvJuegos.Items.Remove(botonJuego);
                        break;
                    }
                }        
            }
            else
            {
                foreach (Button2 botonJuego in ObjetosVentana.gvJuegos.Items)
                {
                    SteamCuentayJuego juegoGv = botonJuego.Tag as SteamCuentayJuego;

                    if (juegoGv.Juego.ID == cuentayJuego.Juego.ID)
                    {
                        StackPanel sp = botonJuego.Content as StackPanel;
                        TextBlock tb = sp.Children[1] as TextBlock;

                        int contadorLogrosHechos = 0;

                        foreach (Logro logro in logros)
                        {
                            if (logro.Estado == "1")
                            {
                                contadorLogrosHechos += 1;
                            }
                        }

                        tb.Text = contadorLogrosHechos.ToString() + "/" + logros.Count.ToString() + " • " + ((int)(100 / logros.Count * contadorLogrosHechos)).ToString() + "%";
                    }
                }
            }
        }

        public static void ImagenFalla(object sender, RoutedEventArgs e)
        {
            Image imagen = sender as Image;
            SteamCuentayJuego cuentayJuego = imagen.Tag as SteamCuentayJuego;

            if (cuentayJuego.Juego.Imagen.Contains("/library_600x900.jpg") == true)
            {
                string nuevaImagen = cuentayJuego.Juego.Imagen;
                nuevaImagen = nuevaImagen.Replace("/library_600x900.jpg", "/header.jpg");

                imagen.Source = new BitmapImage(new Uri(nuevaImagen));
                cuentayJuego.Juego.Imagen = nuevaImagen;

                ActualizarJuegoFichero(cuentayJuego);
            }
            else
            {
                foreach (Button2 botonJuego in ObjetosVentana.gvJuegos.Items)
                {
                    SteamCuentayJuego juegoGv = botonJuego.Tag as SteamCuentayJuego;

                    if (juegoGv.Juego.ID == cuentayJuego.Juego.ID)
                    {
                        ObjetosVentana.gvJuegos.Items.Remove(botonJuego);
                        break;
                    }
                }
            }
        }

        public static async Task<SteamJuego> LeerJuegoFichero(SteamCuentayJuego cuentayJuego)
        {
            StorageFolder carpetaJugador = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\" + cuentayJuego.Cuenta.ID64);
            StorageFile ficheroJuego = null;
            bool existeFichero = false;

            try
            {
                ficheroJuego = await carpetaJugador.GetFileAsync(cuentayJuego.Juego.ID + ".json");
                existeFichero = true;
            }
            catch { }

            if (existeFichero == false)
            {
                ActualizarJuegoFichero(cuentayJuego);
                return cuentayJuego.Juego;
            }
            else
            {
                string textoJuego = await FileIO.ReadTextAsync(ficheroJuego);
                SteamJuego juego = JsonConvert.DeserializeObject<SteamJuego>(textoJuego);
                return juego;
            }
        }

        public static async void ActualizarJuegoFichero(SteamCuentayJuego cuentayJuego)
        {
            StorageFolder carpetaJugador = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\" + cuentayJuego.Cuenta.ID64);
            StorageFile ficheroJuego = null;
            bool existeFichero = false;

            try
            {
                ficheroJuego = await carpetaJugador.GetFileAsync(cuentayJuego.Juego.ID + ".json");
                existeFichero = true;
            }
            catch { }

            if (existeFichero == false) 
            {
                try
                {
                    ficheroJuego = await carpetaJugador.CreateFileAsync(cuentayJuego.Juego.ID + ".json");
                }
                catch { }         
            }

            if (ficheroJuego != null)
            {
                string textoJuego = JsonConvert.SerializeObject(cuentayJuego.Juego, Formatting.Indented);

                try
                {
                    await FileIO.WriteTextAsync(ficheroJuego, textoJuego);
                }
                catch { }          
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
