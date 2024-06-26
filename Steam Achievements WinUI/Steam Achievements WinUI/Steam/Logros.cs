﻿using FontAwesome6;
using FontAwesome6.Fonts;
using Herramientas;
using Interfaz;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoLibrary;
using Windows.Media.Core;
using Windows.Storage;
using Windows.System;
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

            //---------------------------------

            int i = 0;
            foreach (object boton in ObjetosVentana.spLogrosBotones.Children)
            {
                if (boton.GetType() == typeof(Button2))
                {
                    Button2 boton2 = boton as Button2;

                    boton2.Tag = i;
                    boton2.Click += CambiarPestaña;
                    boton2.PointerEntered += Animaciones.EntraRatonBoton2;
                    boton2.PointerExited += Animaciones.SaleRatonBoton2;

                    i += 1;
                }

            }

            CambiarPestaña(1);

            //---------------------------------

            ObjetosVentana.botonLogrosSteam.Click += AbrirLogrosJuego;
            ObjetosVentana.botonLogrosSteam.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonLogrosSteam.PointerExited += Animaciones.SaleRatonBoton2;

            ObjetosVentana.botonLogrosDetallesSteam.Click += AbrirDetallesJuego;
            ObjetosVentana.botonLogrosDetallesSteam.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonLogrosDetallesSteam.PointerExited += Animaciones.SaleRatonBoton2;

            ObjetosVentana.botonLogrosHubSteam.Click += AbrirHubJuego;
            ObjetosVentana.botonLogrosHubSteam.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonLogrosHubSteam.PointerExited += Animaciones.SaleRatonBoton2;

            ObjetosVentana.botonLogrosGuiasSteam.Click += AbrirGuiasJuego;
            ObjetosVentana.botonLogrosGuiasSteam.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonLogrosGuiasSteam.PointerExited += Animaciones.SaleRatonBoton2;
        }

        private static void CambiarPestaña(object sender, RoutedEventArgs e)
        {
            Button2 botonPulsado = sender as Button2;
            int pestañaMostrar = (int)botonPulsado.Tag;
            CambiarPestaña(pestañaMostrar);
        }

        private static void CambiarPestaña(int botonPulsado)
        {
            SolidColorBrush colorPulsado = new SolidColorBrush((Color)Application.Current.Resources["ColorPrimario"]);
            colorPulsado.Opacity = 0.6;

            int i = 0;
            foreach (object boton in ObjetosVentana.spLogrosBotones.Children)
            {
                if (boton.GetType() == typeof(Button2))
                {
                    Button2 boton2 = boton as Button2;

                    if (i == botonPulsado)
                    {
                        boton2.Background = colorPulsado;
                    }
                    else
                    {
                        boton2.Background = new SolidColorBrush(Colors.Transparent);
                    }

                    i += 1;
                }                    
            }

            foreach (StackPanel sp in ObjetosVentana.spLogrosPestañas.Children)
            {
                sp.Visibility = Visibility.Collapsed;
            }

            StackPanel spMostrar = ObjetosVentana.spLogrosPestañas.Children[botonPulsado] as StackPanel;
            spMostrar.Visibility = Visibility.Visible;
        }

        public static async void AbrirLogrosJuego(object sender, RoutedEventArgs e)
        {
            string id = ObjetosVentana.botonLogrosSteam.Tag as string;

            await Launcher.LaunchUriAsync(new Uri("steam://url/SteamIDAchievementsPage/" + id));
        }

        public static async void AbrirDetallesJuego(object sender, RoutedEventArgs e)
        {
            string id = ObjetosVentana.botonLogrosDetallesSteam.Tag as string;

            await Launcher.LaunchUriAsync(new Uri("steam://nav/games/details/" + id));
        }

        public static async void AbrirHubJuego(object sender, RoutedEventArgs e)
        {
            string id = ObjetosVentana.botonLogrosHubSteam.Tag as string;

            await Launcher.LaunchUriAsync(new Uri("steam://url/GameHub/" + id + "/Guides/"));
        }

        public static async void AbrirGuiasJuego(object sender, RoutedEventArgs e)
        {
            string id = ObjetosVentana.botonLogrosGuiasSteam.Tag as string;

            await Launcher.LaunchUriAsync(new Uri("https://steamcommunity.com/app/" + id + "/guides/?browsefilter=trend&requiredtags%5B%5D=Achievements"));
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

            CambiarPestaña(1);

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

            ObjetosVentana.botonLogrosSteam.Tag = idJuego;
            ObjetosVentana.botonLogrosDetallesSteam.Tag = idJuego;
            ObjetosVentana.botonLogrosHubSteam.Tag = idJuego;
            ObjetosVentana.botonLogrosGuiasSteam.Tag = idJuego;

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
                    ObjetosVentana.spLogrosBotones.Children[0].Visibility = Visibility.Collapsed;

                    CambiarPestaña(1);

                    Button2 botonPosicion1 = ObjetosVentana.spLogrosBotones.Children[1] as Button2;
                    botonPosicion1.Margin = new Thickness(0);
                }
                else
                {
                    ObjetosVentana.spLogrosBotones.Children[0].Visibility = Visibility.Visible;

                    Button2 botonPosicion1 = ObjetosVentana.spLogrosBotones.Children[1] as Button2;
                    botonPosicion1.Margin = new Thickness(0, 10, 0, 0);

                    ObjetosVentana.tbLogrosCompletadosInfo.Text = logrosCompletados.Count.ToString() + "/" + logros.Count.ToString() + " • " + ((int)Math.Round((double)(100 * logrosCompletados.Count / logros.Count))).ToString() + "%";

                    ObjetosVentana.spLogrosCompletados.Children.Clear();

                    int i = 0;
                    foreach (Logro logro in logrosCompletados)
                    {                        
                        if (i == 0)
                        {
                            ObjetosVentana.spLogrosCompletados.Children.Add(LogroEstilo(cuentayJuego.Juego, logro, true));
                        }
                        else
                        {
                            ObjetosVentana.spLogrosCompletados.Children.Add(LogroEstilo(cuentayJuego.Juego, logro, false));
                        }

                        i += 1;
                    }
                }

                if (logrosPendientes.Count == 0)
                {
                    ObjetosVentana.spLogrosBotones.Children[1].Visibility = Visibility.Collapsed;

                    CambiarPestaña(0);
                }
                else
                {
                    ObjetosVentana.spLogrosBotones.Children[1].Visibility = Visibility.Visible;

                    ObjetosVentana.tbLogrosPendientesInfo.Text = logrosPendientes.Count.ToString() + "/" + logros.Count.ToString() + " • " + ((int)Math.Round((double)(100 * logrosPendientes.Count / logros.Count))).ToString() + "%";

                    ObjetosVentana.spLogrosPendientes.Children.Clear();

                    int i = 0;
                    foreach (Logro logro in logrosPendientes)
                    {
                        if (i == 0)
                        {
                            ObjetosVentana.spLogrosPendientes.Children.Add(LogroEstilo(cuentayJuego.Juego, logro, true));
                        }
                        else
                        {
                            ObjetosVentana.spLogrosPendientes.Children.Add(LogroEstilo(cuentayJuego.Juego, logro, false));
                        }
                        
                        i += 1;
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

        public static Grid LogroEstilo(SteamJuego juego, Logro logro, bool primero)
        {
            SteamJuegoyLogro juegoyLogro = new SteamJuegoyLogro
            {
                Juego = juego,
                Logro = logro
            };

            SolidColorBrush colorFondo = new SolidColorBrush
            {
                Color = (Color)Application.Current.Resources["ColorPrimario"],
                Opacity = 0.6
            };

            Grid grid = new Grid
            {
                VerticalAlignment = VerticalAlignment.Top,
                Padding = new Thickness(30),
                CornerRadius = new CornerRadius(5),
                Background = colorFondo
            };

            if (primero == false)
            {
                grid.Margin = new Thickness(0, 10, 0, 0);
            }

            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            ColumnDefinition col3 = new ColumnDefinition();

            col1.Width = new GridLength(1, GridUnitType.Auto);
            col2.Width = new GridLength(1, GridUnitType.Star);
            col3.Width = new GridLength(1, GridUnitType.Auto);

            grid.ColumnDefinitions.Add(col1);
            grid.ColumnDefinitions.Add(col2);
            grid.ColumnDefinitions.Add(col3);

            Image imagen = new Image
            {
                Source = new BitmapImage(new Uri(logro.Imagen)),
                Width = 48,
                Height = 48,
                Margin = new Thickness(0, 0, 20, 0),
                VerticalAlignment = VerticalAlignment.Center
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

            sp.SetValue(Grid.ColumnProperty, 1);

            grid.Children.Add(sp);

            //----------------------------------------------

            StackPanel spBotones = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            StackPanel spYoutube = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            FontAwesome iconoYoutube = new FontAwesome
            {
                Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                Icon = EFontAwesomeIcon.Brands_Youtube
            };

            spYoutube.Children.Add(iconoYoutube);

            ResourceLoader recursos = new ResourceLoader();

            Button2 botonYoutube = new Button2
            {
                Tag = juegoyLogro,
                Content = spYoutube,
                Background = new SolidColorBrush((Color)Application.Current.Resources["ColorPrimario"]),
                CornerRadius = new CornerRadius(5),
                RequestedTheme = ElementTheme.Light,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 12, 15, 12),
                Margin = new Thickness(20, 0, 0, 0)
            };

            botonYoutube.Click += AbrirYoutube;
            botonYoutube.PointerEntered += Animaciones.EntraRatonBoton2;
            botonYoutube.PointerExited += Animaciones.SaleRatonBoton2;

            TextBlock tooltipYoutube = new TextBlock
            {
                Text = recursos.GetString("TooltipYoutube")
            };

            ToolTipService.SetToolTip(botonYoutube, tooltipYoutube);
            ToolTipService.SetPlacement(botonYoutube, PlacementMode.Bottom);

            spBotones.Children.Add(botonYoutube);

            //----------------------------------------------

            StackPanel spGoogle = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            FontAwesome iconoGoogle = new FontAwesome
            {
                Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                Icon = EFontAwesomeIcon.Brands_Google
            };

            spGoogle.Children.Add(iconoGoogle);

            Button2 botonGoogle = new Button2
            {
                Tag = juegoyLogro,
                Content = spGoogle,
                Background = new SolidColorBrush((Color)Application.Current.Resources["ColorPrimario"]),
                CornerRadius = new CornerRadius(5),
                RequestedTheme = ElementTheme.Light,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 12, 15, 12),
                Margin = new Thickness(20, 0, 0, 0)
            };

            botonGoogle.Click += AbrirGoogle;
            botonGoogle.PointerEntered += Animaciones.EntraRatonBoton2;
            botonGoogle.PointerExited += Animaciones.SaleRatonBoton2;

            TextBlock tooltipGoogle = new TextBlock
            {
                Text = recursos.GetString("TooltipGoogle")
            };

            ToolTipService.SetToolTip(botonGoogle, tooltipGoogle);
            ToolTipService.SetPlacement(botonGoogle, PlacementMode.Bottom);

            spBotones.Children.Add(botonGoogle);

            StackPanel spBing = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            FontAwesome iconoBing = new FontAwesome
            {
                Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                Icon = EFontAwesomeIcon.Brands_Microsoft
            };

            spBing.Children.Add(iconoBing);

            Button2 botonBing = new Button2
            {
                Tag = juegoyLogro,
                Content = spBing,
                Background = new SolidColorBrush((Color)Application.Current.Resources["ColorPrimario"]),
                CornerRadius = new CornerRadius(5),
                RequestedTheme = ElementTheme.Light,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(15, 12, 15, 12),
                Margin = new Thickness(20, 0, 0, 0)
            };

            botonBing.Click += AbrirBing;
            botonBing.PointerEntered += Animaciones.EntraRatonBoton2;
            botonBing.PointerExited += Animaciones.SaleRatonBoton2;

            TextBlock tooltipBing = new TextBlock
            {
                Text = recursos.GetString("TooltipBing")
            };

            ToolTipService.SetToolTip(botonBing, tooltipBing);
            ToolTipService.SetPlacement(botonBing, PlacementMode.Bottom);

            spBotones.Children.Add(botonBing);

            spBotones.SetValue(Grid.ColumnProperty, 2);

            grid.Children.Add(spBotones);

            return grid;
        }

        public static async void AbrirYoutube(object sender, RoutedEventArgs e)
        {
            ResourceLoader recursos = new ResourceLoader();
            ObjetosVentana.tTipLogros.Subtitle = recursos.GetString("SearchingYoutube");
            ObjetosVentana.tTipLogros.IsOpen = true;
            ActivaryDesactivarYoutube(false, ObjetosVentana.spLogrosCompletados);
            ActivaryDesactivarYoutube(false, ObjetosVentana.spLogrosPendientes);

            //----------------------------------------------

            Button2 boton = sender as Button2;
            SteamJuegoyLogro juegoyLogro = boton.Tag as SteamJuegoyLogro;

            YoutubeClient youtube = new YoutubeClient();
            IReadOnlyList<VideoSearchResult> videos = null;

            await Task.Run(async () =>
            {
                videos = await youtube.Search.GetVideosAsync(juegoyLogro.Juego.Titulo + " " + juegoyLogro.Logro.Nombre);
            });

            ObjetosVentana.tTipLogros.IsOpen = false;
            ActivaryDesactivarYoutube(true, ObjetosVentana.spLogrosCompletados);
            ActivaryDesactivarYoutube(true, ObjetosVentana.spLogrosPendientes);

            if (videos.Count > 0)
            {
                YouTube libreria = YouTube.Default;
                IEnumerable<YouTubeVideo> resultados = libreria.GetAllVideos(videos[0].Url);
                string enlace = string.Empty;
                int resolucion = 0;

                foreach (YouTubeVideo resultado in resultados)
                {
                    resolucion = resultado.Resolution;
                    enlace = resultado.Uri;

                    if (resolucion >= 720)
                    {
                        break;
                    }          
                }

                MediaPlayerElement reproductor = new MediaPlayerElement
                {
                    Source = MediaSource.CreateFromUri(new Uri(enlace)),
                    AutoPlay = true,
                    AreTransportControlsEnabled = true
                };

                ContentDialog ventana = new ContentDialog
                {
                    RequestedTheme = ElementTheme.Dark,
                    SecondaryButtonText = recursos.GetString("OpenVideoYoutube"),
                    CloseButtonText = recursos.GetString("Close"),
                    Content = reproductor,
                    XamlRoot = ObjetosVentana.ventana.Content.XamlRoot,
                    Tag = videos[0].Url
                };

                ventana.SecondaryButtonClick += AbrirVideoYoutube;
                ventana.CloseButtonClick += CerrarVentanaYoutube;

                await ventana.ShowAsync();
            }
        }

        private static void ActivaryDesactivarYoutube(bool estado, StackPanel spYoutube)
        {
            foreach (Grid grid in spYoutube.Children)
            {
                StackPanel sp = grid.Children[2] as StackPanel;
                Button2 boton2 = sp.Children[0] as Button2;
                boton2.IsEnabled = estado;
            }
        }

        private static async void AbrirVideoYoutube(ContentDialog ventana, ContentDialogButtonClickEventArgs e)
        {
            string video = ventana.Tag as string;
            await Launcher.LaunchUriAsync(new Uri(video));

            MediaPlayerElement reproductor = ventana.Content as MediaPlayerElement;
            reproductor.AutoPlay = false;
            reproductor.MediaPlayer.Pause();
        }

        private static void CerrarVentanaYoutube(ContentDialog ventana, ContentDialogButtonClickEventArgs e) 
        {
            MediaPlayerElement reproductor = ventana.Content as MediaPlayerElement;
            reproductor.AutoPlay = false;
            reproductor.MediaPlayer.Pause();
        }

        public static async void AbrirGoogle(object sender, RoutedEventArgs e)
        {
            Button2 boton = sender as Button2;
            SteamJuegoyLogro juegoyLogro = boton.Tag as SteamJuegoyLogro;

            string busqueda = juegoyLogro.Juego.Titulo + " " + juegoyLogro.Logro.Nombre;
            busqueda = busqueda.Replace(" ", "+");

            await Launcher.LaunchUriAsync(new Uri("https://www.google.com/search?q=" + busqueda));
        }

        public static async void AbrirBing(object sender, RoutedEventArgs e)
        {
            Button2 boton = sender as Button2;
            SteamJuegoyLogro juegoyLogro = boton.Tag as SteamJuegoyLogro;

            string busqueda = juegoyLogro.Juego.Titulo + " " + juegoyLogro.Logro.Nombre;
            busqueda = busqueda.Replace(" ", "+");

            await Launcher.LaunchUriAsync(new Uri("https://www.bing.com/search?q=" + busqueda));
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
