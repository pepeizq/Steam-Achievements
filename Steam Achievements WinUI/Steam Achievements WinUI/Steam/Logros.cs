using Herramientas;
using Interfaz;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Search;
using static Steam_Achievements_WinUI.MainWindow;

namespace Steam
{
    public static class Logros
    {
        public static void Cargar()
        {
            ObjetosVentana.imagenLogrosCabecera.ImageFailed += CabeceraImagenFalla;
        }

        public static async Task<List<Logro>> ComprobarJuego(string idJugador, string idJuego)
        {
            List<Logro> logros = new List<Logro>();

            StorageFolder carpetaJugador = await StorageFolder.GetFolderFromPathAsync(ApplicationData.Current.LocalFolder.Path + "\\" + idJugador);
            StorageFile ficheroJuego = null;

            try
            {
                ficheroJuego = await carpetaJugador.GetFileAsync(idJuego + ".json");
            }
            catch { }

            if (ficheroJuego != null)
            {
                string contenidoJuego = await FileIO.ReadTextAsync(ficheroJuego);

                if (contenidoJuego != string.Empty)
                {
                    SteamJuego juego = JsonConvert.DeserializeObject<SteamJuego>(contenidoJuego);

                    logros = juego.Logros;

                    if (logros == null)
                    {
                        logros = new List<Logro>();
                    }
                }
            }

            if (logros.Count == 0)
            {
                string htmlJugador = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + idJugador + "&appid=" + idJuego);

                if (htmlJugador != null)
                {
                    SteamJugadorLogrosAPI jugadorLogrosAPI = JsonConvert.DeserializeObject<SteamJugadorLogrosAPI>(htmlJugador);

                    if (jugadorLogrosAPI != null)
                    {
                        if (jugadorLogrosAPI.Datos != null)
                        {
                            if (jugadorLogrosAPI.Datos.Logros != null)
                            {
                                if (jugadorLogrosAPI.Datos.Logros.Count > 0)
                                {
                                    foreach (SteamJugadorLogrosAPIDatosLogro logroAPI in jugadorLogrosAPI.Datos.Logros)
                                    {
                                        Logro logro = new Logro
                                        {
                                            ID = logroAPI.NombreAPI,
                                            Nombre = null,
                                            Descripcion = null,
                                            Estado = logroAPI.Estado
                                        };

                                        logros.Add(logro);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return logros;
        }

        public static async void CargarJuego(object sender, RoutedEventArgs e)
        {
            await Task.Yield();

            Button2 boton = sender as Button2;
            SteamCuentayJuego cuentayJuego = boton.Tag as SteamCuentayJuego;

            Pestañas.Visibilidad(ObjetosVentana.gridLogros);
            Pestañas.CrearSeparador(4);
            Pestañas.CreadorItems(cuentayJuego.Juego.Icono, cuentayJuego.Juego.Titulo, 5);

            ObjetosVentana.spJuegosBuscador.Visibility = Visibility.Collapsed;

            //-------------------------------------------------------

            string imagenCabecera = Juegos.dominioImagen + "/steam/apps/" + cuentayJuego.Juego.ID + "/logo.png";
            string fondoCabecera = Juegos.dominioImagen + "/steam/apps/" + cuentayJuego.Juego.ID + "/library_hero.jpg";

            ObjetosVentana.imagenLogrosCabecera.Source = new BitmapImage(new Uri(imagenCabecera));
            ObjetosVentana.imagenLogrosCabecera.Tag = imagenCabecera;
            ObjetosVentana.imagenLogrosCabecera.Margin = new Thickness(50);

            ImageBrush fondoImagen = new ImageBrush();
            fondoImagen.ImageSource = new BitmapImage(new Uri(fondoCabecera));
            fondoImagen.Stretch = Stretch.UniformToFill;

            ObjetosVentana.spLogrosCabecera.Background = fondoImagen;

            //-------------------------------------------------------

            string idJugador = cuentayJuego.Cuenta.ID64;
            string idJuego = cuentayJuego.Juego.ID;

            List<Logro> logros = new List<Logro>();
            
            string htmlJugador = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUserStats/GetPlayerAchievements/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamid=" + idJugador + "&appid=" + idJuego);
        
            if (htmlJugador != null)
            {
                SteamJugadorLogrosAPI jugadorLogrosAPI = JsonConvert.DeserializeObject<SteamJugadorLogrosAPI>(htmlJugador);
            
                if (jugadorLogrosAPI != null) 
                {
                    if (jugadorLogrosAPI.Datos != null)
                    {
                        if (jugadorLogrosAPI.Datos.Logros != null)
                        {
                            if (jugadorLogrosAPI.Datos.Logros.Count > 0)
                            {
                                string htmlLogros = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUserStats/GetSchemaForGame/v2/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&appid=" + idJuego);

                                if (htmlLogros != null)
                                {
                                    SteamJuegoLogrosAPI juegoLogrosAPI = JsonConvert.DeserializeObject<SteamJuegoLogrosAPI>(htmlLogros);

                                    if (juegoLogrosAPI != null)
                                    {
                                        foreach (SteamJuegoLogrosAPILogro juegoLogroAPI in juegoLogrosAPI.Datos.Datos2.Logros)
                                        {
                                            Logro logro = new Logro
                                            {
                                                ID = juegoLogroAPI.NombreAPI,
                                                Nombre = juegoLogroAPI.NombreMostrar,
                                                Descripcion = juegoLogroAPI.Descripcion
                                            };

                                            foreach (SteamJugadorLogrosAPIDatosLogro jugadorLogroAPI in jugadorLogrosAPI.Datos.Logros)
                                            {
                                                if (jugadorLogroAPI.NombreAPI == juegoLogroAPI.NombreAPI)
                                                {
                                                    logro.Estado = jugadorLogroAPI.Estado;

                                                    if (logro.Estado == "1")
                                                    {
                                                        logro.Imagen = juegoLogroAPI.IconoCompletado;
                                                    }
                                                    else
                                                    {
                                                        logro.Imagen = juegoLogroAPI.IconoPendiente;
                                                    }

                                                    break;
                                                }
                                            }

                                            logros.Add(logro);
                                        }
                                    }
                                }
                            }
                        }             
                    }     
                }
            }

            if (logros.Count > 0)
            {
                List<Logro> logrosCompletados = new List<Logro>();
                List<Logro> logrosPendientes = new List<Logro>();

                foreach (Logro logro in logros) 
                { 
                    if (logro.Estado == "1")
                    {
                        logrosCompletados.Add(logro);
                    }
                    else if (logro.Estado == "0")
                    {
                        logrosPendientes.Add(logro);
                    }
                }

                if (logrosCompletados.Count == 0)
                {
                    ObjetosVentana.expanderLogrosCompletados.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ObjetosVentana.expanderLogrosCompletados.Visibility = Visibility.Visible;

                    ObjetosVentana.tbLogrosCompletadosInfo.Text = logrosCompletados.Count.ToString() + "/" + logros.Count.ToString() + " • " + ((int)Math.Round((double)(100 * logrosCompletados.Count / logros.Count))).ToString() + "%";

                    ObjetosVentana.spLogrosCompletados.Children.Clear();

                    foreach (Logro logro in logrosCompletados)
                    {
                        ObjetosVentana.spLogrosCompletados.Children.Add(LogroEstilo(cuentayJuego.Juego, logro));
                    }
                }

                if (logrosPendientes.Count == 0)
                {
                    ObjetosVentana.expanderLogrosCompletados.Margin = new Thickness(0);
                    ObjetosVentana.expanderLogrosPendientes.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ObjetosVentana.expanderLogrosCompletados.Margin = new Thickness(0, 0, 0, 40);
                    ObjetosVentana.expanderLogrosPendientes.Visibility = Visibility.Visible;

                    ObjetosVentana.tbLogrosPendientesInfo.Text = logrosPendientes.Count.ToString() + "/" + logros.Count.ToString() + " • " + ((int)Math.Round((double)(100 * logrosPendientes.Count / logros.Count))).ToString() + "%";

                    ObjetosVentana.spLogrosPendientes.Children.Clear();

                    foreach (Logro logro in logrosPendientes)
                    {
                        ObjetosVentana.spLogrosPendientes.Children.Add(LogroEstilo(cuentayJuego.Juego, logro));
                    }
                }
            }
        }

        public static void CabeceraImagenFalla(object sender, RoutedEventArgs e)
        {
            Image imagen = sender as Image;
            string imagenCabecera = imagen.Tag as string;
            imagenCabecera = imagenCabecera.Replace("/logo.png", "/header.jpg");
            imagen.Source = new BitmapImage(new Uri(imagenCabecera));
            imagen.Margin = new Thickness(10);
        }

        public static Grid LogroEstilo(SteamJuego juego, Logro logro)
        {
            SteamJuegoyLogro juegoyLogro = new SteamJuegoyLogro
            {
                Juego = juego,
                Logro = logro
            };

            Grid grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Thickness(5)
            };

            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();

            col1.Width = new GridLength(1, GridUnitType.Auto);
            col2.Width = new GridLength(1, GridUnitType.Star);

            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);

            Image imagen = new Image
            {
                Source = new BitmapImage(new Uri(logro.Imagen)),
                Width = 48,
                Height = 48,
                Margin = new Thickness(0, 0, 20, 0),
                VerticalAlignment = VerticalAlignment.Top
            };

            imagen.SetValue(Grid.ColumnProperty, 0);

            grid.Children.Add(imagen);

            //----------------------------------------------

            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            TextBlock tbNombre = new TextBlock
            {
                Text = logro.Nombre,
                Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                FontSize = 17,
                TextWrapping = TextWrapping.Wrap
            };

            sp.Children.Add(tbNombre);

            if (logro.Descripcion != null)
            {
                TextBlock tbDescripcion = new TextBlock
                {
                    Text = logro.Descripcion,
                    Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 10, 0, 0)
                };

                sp.Children.Add(tbDescripcion);
            }

            //----------------------------------------------

            StackPanel spBotones = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 10, 0, 0)
            };

            Button2 botonYoutube = new Button2();
            botonYoutube.Tag = juegoyLogro;

            botonYoutube.Click += AbrirYoutube;


            spBotones.Children.Add(botonYoutube);

            sp.Children.Add(spBotones);

            //----------------------------------------------

            sp.SetValue(Grid.ColumnProperty, 1);

            grid.Children.Add(sp);

            return grid;
        }

        public static async void AbrirYoutube(object sender, RoutedEventArgs e)
        {




            Button2 boton = sender as Button2;
            SteamJuegoyLogro juegoyLogro = boton.Tag as SteamJuegoyLogro;

            YoutubeClient youtube = new YoutubeClient();
            IReadOnlyList<VideoSearchResult> videos = await youtube.Search.GetVideosAsync(juegoyLogro.Juego.Titulo + " " + juegoyLogro.Logro.Nombre);

            if (videos.Count > 0)
            {
                VideoLibrary.YouTube libreria = VideoLibrary.YouTube.Default;
                IEnumerable<VideoLibrary.YouTubeVideo> resultados = libreria.GetAllVideos(videos[0].Url);
                string enlace = string.Empty;

                foreach (var resultado in resultados)
                {
                    enlace = resultado.Uri;
                    break;
                }

                MediaPlayerElement reproductor = new MediaPlayerElement
                {
                    Source = MediaSource.CreateFromUri(new Uri(enlace)),
                    AutoPlay = true,
                    AreTransportControlsEnabled = true
                };

                ResourceLoader recursos = new ResourceLoader();

                ContentDialog ventana = new ContentDialog
                {
                    RequestedTheme = ElementTheme.Dark,
                    CloseButtonText = recursos.GetString("Close"),
                    Content = reproductor,
                    XamlRoot = ObjetosVentana.ventana.Content.XamlRoot
                };

                await ventana.ShowAsync();
            }
        }
    }

    //----------------------------------------------

    public class SteamJuegoyLogro
    {
        public SteamJuego Juego { get; set; }
        public Logro Logro { get; set; }
    }

    //----------------------------------------------

    public class Logro
    {
        public string ID { get; set; }
        public string Estado { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Imagen { get; set; }
        public string Fecha { get; set; }
    }

    //----------------------------------------------

    public class SteamJugadorLogrosAPI
    {
        [JsonProperty("playerstats")]
        public SteamJugadorLogrosAPIDatos Datos { get; set; }
    }

    public class SteamJugadorLogrosAPIDatos
    {
        [JsonProperty("success")]
        public bool Estado { get; set; }

        [JsonProperty("steamID")]
        public string UsuarioID { get; set; }

        [JsonProperty("gameName")]
        public string TituloJuego { get; set; }

        [JsonProperty("achievements")]
        public List<SteamJugadorLogrosAPIDatosLogro> Logros { get; set; }
    }

    public class SteamJugadorLogrosAPIDatosLogro
    {
        [JsonProperty("apiname")]
        public string NombreAPI { get; set; }

        [JsonProperty("achieved")]
        public string Estado { get; set; }

        [JsonProperty("unlocktime")]
        public string Fecha { get; set; }
    }

    //----------------------------------------------

    public class SteamJuegoLogrosAPI
    {
        [JsonProperty("game")]
        public SteamJuegoLogrosAPIDatos Datos { get; set; }
    }

    public class SteamJuegoLogrosAPIDatos
    {
        [JsonProperty("gameName")]
        public string TituloJuego { get; set; }

        [JsonProperty("availableGameStats")]
        public SteamJuegoLogrosAPIDatos2 Datos2 { get; set; }
    }

    public class SteamJuegoLogrosAPIDatos2
    {
        [JsonProperty("achievements")]
        public List<SteamJuegoLogrosAPILogro> Logros { get; set; }
    }

    public class SteamJuegoLogrosAPILogro
    {
        [JsonProperty("name")]
        public string NombreAPI { get; set; }

        [JsonProperty("displayName")]
        public string NombreMostrar { get; set; }

        [JsonProperty("description")]
        public string Descripcion { get; set; }

        [JsonProperty("icon")]
        public string IconoCompletado { get; set; }

        [JsonProperty("icongray")]
        public string IconoPendiente { get; set; }
    }
}
