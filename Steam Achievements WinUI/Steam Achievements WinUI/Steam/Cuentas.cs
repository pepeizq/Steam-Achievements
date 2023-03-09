using CommunityToolkit.WinUI.UI.Controls;
using FontAwesome6;
using FontAwesome6.Fonts;
using Herramientas;
using Interfaz;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
    public static class Cuentas
    {
        private static StorageFolder carpetaCuentas = ApplicationData.Current.LocalFolder;
        private static string nombreFicheroCuentas = "Accounts.json";

        public static void Cargar()
        {
            ObjetosVentana.tbCuentasAñadir.GotFocus += DetectarFoco;
            ObjetosVentana.tbCuentasAñadir.LostFocus += PerderFoco;
            ObjetosVentana.tbCuentasAñadir.TextChanged += DetectarEnlaceCuenta;

            ObjetosVentana.botonCuentasAvisoPermisos.Click += AbrirImagenPermisos;
            ObjetosVentana.botonCuentasAvisoPermisos.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonCuentasAvisoPermisos.PointerExited += Animaciones.SaleRatonBoton2;

            ObjetosVentana.botonCuentasAñadirSi.Click += AñadirCuenta;
            ObjetosVentana.botonCuentasAñadirSi.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonCuentasAñadirSi.PointerExited += Animaciones.SaleRatonBoton2;

            ObjetosVentana.botonCuentasAñadirNo.Click += NoAñadirCuenta;
            ObjetosVentana.botonCuentasAñadirNo.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonCuentasAñadirNo.PointerExited += Animaciones.SaleRatonBoton2;

            //-------------------------------------

            ActualizarCuentas(null);

            //-------------------------------------

            ApplicationDataContainer datos = ApplicationData.Current.LocalSettings;

            if (datos.Values["OpcionesCuentasMensajes"] == null)
            {
                datos.Values["OpcionesCuentasMensajes"] = true;
            }

            ObjetosVentana.tsOpcionesCuentasMensaje.IsOn = (bool)datos.Values["OpcionesCuentasMensajes"];
            ObjetosVentana.tsOpcionesCuentasMensaje.Toggled += MensajesEnseñar;
        }

        private static void MensajesEnseñar(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;

            ApplicationDataContainer datos = ApplicationData.Current.LocalSettings;
            datos.Values["OpcionesCuentasMensajes"] = ts.IsOn;
        }

        public static void DetectarFoco(object sender, RoutedEventArgs e)
        {
            ApplicationDataContainer datos = ApplicationData.Current.LocalSettings;

            if ((bool)datos.Values["OpcionesCuentasMensajes"] == true)
            {
                if (ObjetosVentana.tTipCuentasAviso.IsOpen == false)
                {
                    ResourceLoader recursos = new ResourceLoader();

                    ObjetosVentana.tTipCuentasAviso.Title = recursos.GetString("Advice");
                    ObjetosVentana.tTipCuentasAviso.Subtitle = recursos.GetString("AdvicePermissions");
                    ObjetosVentana.tTipCuentasAviso.IsOpen = true;
                    ObjetosVentana.tTipCuentasAviso.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorPrimario"]);
                    ObjetosVentana.spCuentasAvisoPermisos.Visibility = Visibility.Visible;
                }
            }           
        }

        public static void PerderFoco(object sender, RoutedEventArgs e)
        {
            ObjetosVentana.tTipCuentasAviso.IsOpen = false;
        }

        public static async void AbrirImagenPermisos(object sender, RoutedEventArgs e)
        {
            ImageEx imagen = new ImageEx
            {
                Source = "Assets\\cuentapermisos.jpg",
                Width = 818,
                Height = 544,
                IsCacheEnabled = true,
                EnableLazyLoading = true,
                CornerRadius = new CornerRadius(5)
            };

            ResourceLoader recursos = new ResourceLoader();

            ContentDialog ventana = new ContentDialog
            {
                RequestedTheme = ElementTheme.Dark,
                CloseButtonText = recursos.GetString("Close"),
                Content = imagen,
                XamlRoot = ObjetosVentana.ventana.Content.XamlRoot,
                Background = new SolidColorBrush((Color)Application.Current.Resources["ColorPrimario"])
            };

            await ventana.ShowAsync();
        }

        public static async void DetectarEnlaceCuenta(object sender, TextChangedEventArgs e)
        {           
            ActivarDesactivar(false);
            ObjetosVentana.prCuentasCargar.Visibility = Visibility.Visible;

            ObjetosVentana.tTipCuentasAviso.IsOpen = false;

            await Task.Delay(100);

            TextBox tb = sender as TextBox;

            if (tb.Text.Length > 0) 
            {
                string id64 = string.Empty;

                if (tb.Text.Contains("https://steamcommunity.com/id/") == true)
                {
                    string usuario = tb.Text;

                    usuario = usuario.Replace("https://steamcommunity.com/id/", null);
                    usuario = usuario.Replace("http://steamcommunity.com/id/", null);

                    if (usuario.Contains("?") == true) 
                    { 
                        int int1 = usuario.IndexOf("?");
                        usuario = usuario.Remove(int1, usuario.Length - int1);
                    }

                    if (usuario.Contains("/") == true)
                    {
                        int int1 = usuario.IndexOf("/");
                        usuario = usuario.Remove(int1, usuario.Length - int1);
                    }

                    string html = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUser/ResolveVanityURL/v1/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&vanityurl=" + usuario);

                    if (html != null)
                    {
                        SteamSacarID id = JsonConvert.DeserializeObject<SteamSacarID>(html);

                        if (id != null)
                        {
                            id64 = id.Datos.ID64;
                        }
                    }
                }
                else if (tb.Text.Contains("https://steamcommunity.com/profiles/") == true)
                {
                    string usuario = tb.Text;

                    usuario = usuario.Replace("https://steamcommunity.com/profiles/", null);
                    usuario = usuario.Replace("http://steamcommunity.com/profiles/", null);

                    if (usuario.Contains("?") == true)
                    {
                        int int1 = usuario.IndexOf("?");
                        usuario = usuario.Remove(int1, usuario.Length - int1);
                    }

                    if (usuario.Contains("/") == true)
                    {
                        int int1 = usuario.IndexOf("/");
                        usuario = usuario.Remove(int1, usuario.Length - int1);
                    }

                    id64 = usuario;
                }

                if (id64 != string.Empty)
                {
                    string html = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamids=" + id64);
                
                    if (html != null)
                    {
                        SteamCuentaAPI cuenta = JsonConvert.DeserializeObject<SteamCuentaAPI>(html);

                        if (cuenta != null)
                        {
                            if (cuenta.Datos.Jugador.Count > 0)
                            {
                                bool yaAñadida = false;

                                List<SteamCuenta> cuentasGuardadas = await LeerCuentas();

                                if (cuentasGuardadas.Count > 0)
                                {
                                    foreach (SteamCuenta cuentaGuardada in cuentasGuardadas)
                                    {
                                        if (cuentaGuardada.ID64 == cuenta.Datos.Jugador[0].ID64)
                                        {
                                            yaAñadida = true;
                                        }
                                    }
                                }

                                if (yaAñadida == false)
                                {
                                    ObjetosVentana.spCuentasAñadir.Visibility = Visibility.Collapsed;
                                    ObjetosVentana.spCuentasPreguntar.Visibility = Visibility.Visible;

                                    SteamCuenta cuentaAñadir = new SteamCuenta()
                                    {
                                        ID64 = cuenta.Datos.Jugador[0].ID64,
                                        Avatar = cuenta.Datos.Jugador[0].Avatar,
                                        Nombre = cuenta.Datos.Jugador[0].Nombre
                                    };

                                    ObjetosVentana.botonCuentasAñadirSi.Tag = cuentaAñadir;

                                    ObjetosVentana.imagenCuentasPreguntarAñadir.Source = cuentaAñadir.Avatar;
                                    ObjetosVentana.tbCuentasPreguntarAñadir.Text = cuentaAñadir.Nombre;
                                }
                                else
                                {
                                    ResourceLoader recursos = new ResourceLoader();

                                    ObjetosVentana.tTipCuentasAviso.Title = null;
                                    ObjetosVentana.tTipCuentasAviso.Subtitle = recursos.GetString("AccountAlreadyMessage");
                                    ObjetosVentana.tTipCuentasAviso.IsOpen = true;
                                    ObjetosVentana.tTipCuentasAviso.Background = new SolidColorBrush((Color)Application.Current.Resources["ColorAviso"]);
                                    ObjetosVentana.spCuentasAvisoPermisos.Visibility = Visibility.Collapsed;
                                }
                            }                          
                        }
                    }
                }
            }

            ObjetosVentana.prCuentasCargar.Visibility = Visibility.Collapsed;
            ActivarDesactivar(true);
        }

        public static async Task<List<SteamCuenta>> LeerCuentas()
        {
            List<SteamCuenta> cuentas = new List<SteamCuenta>();

            StorageFile ficheroCuentas = null;
            
            try
            {
                ficheroCuentas = await carpetaCuentas.GetFileAsync(nombreFicheroCuentas);
            }
            catch 
            {
                ficheroCuentas = await carpetaCuentas.CreateFileAsync(nombreFicheroCuentas, CreationCollisionOption.ReplaceExisting);
            }        
       
            if (ficheroCuentas != null)
            {
                string textoCuentas = await FileIO.ReadTextAsync(ficheroCuentas);
                
                if (textoCuentas != null && textoCuentas != string.Empty)
                {
                    cuentas = JsonConvert.DeserializeObject<List<SteamCuenta>>(textoCuentas);
                }
            }

            return cuentas;
        }

        public static async void AñadirCuenta(object sender, RoutedEventArgs e)
        {
            ActivarDesactivar(false);

            List<SteamCuenta> cuentas = await LeerCuentas();

            SteamCuenta cuentaAñadir = ObjetosVentana.botonCuentasAñadirSi.Tag as SteamCuenta;

            cuentas.Add(cuentaAñadir);

            string textoCuentasAñadido = JsonConvert.SerializeObject(cuentas, Formatting.Indented);
            StorageFile ficheroCuentas = await carpetaCuentas.GetFileAsync(nombreFicheroCuentas);
            await FileIO.WriteTextAsync(ficheroCuentas, textoCuentasAñadido);

            ActualizarCuentas(cuentaAñadir);

            ObjetosVentana.spCuentasAñadir.Visibility = Visibility.Visible;
            ObjetosVentana.spCuentasPreguntar.Visibility = Visibility.Collapsed;

            ObjetosVentana.tbCuentasAñadir.Text = string.Empty;

            ActivarDesactivar(true);
        }

        public static async void BorrarCuenta(object sender, RoutedEventArgs e)
        {
            ActivarDesactivar(false);

            List<SteamCuenta> cuentas = await LeerCuentas();

            Button2 botonBorrar = sender as Button2;
            SteamCuenta cuentaBorrar = botonBorrar.Tag as SteamCuenta;

            if (cuentas.Count > 0)
            {
                int i = 0;
                foreach (SteamCuenta cuenta in cuentas)
                {
                    if (cuenta.ID64 == cuentaBorrar.ID64)
                    {
                        break;
                    }

                    i += 1;
                }

                cuentas.RemoveAt(i);
            }

            string textoCuentasAñadido = JsonConvert.SerializeObject(cuentas, Formatting.Indented);
            StorageFile ficheroCuentas = await carpetaCuentas.GetFileAsync(nombreFicheroCuentas);
            await FileIO.WriteTextAsync(ficheroCuentas, textoCuentasAñadido);

            ActualizarCuentas(null);

            ActivarDesactivar(true);
        }

        public static async void ActualizarCuentas(SteamCuenta cuentaSeñalar)
        {
            ObjetosVentana.spCuentasAñadidas.Children.Clear();

            ApplicationDataContainer datos = ApplicationData.Current.LocalSettings;

            List<SteamCuenta> cuentas = await LeerCuentas();
           
            if (cuentas.Count > 0)
            {
                cuentas.Sort(delegate (SteamCuenta c1, SteamCuenta c2) { return c1.Nombre.CompareTo(c2.Nombre); });

                ObjetosVentana.gridCuentasAñadidas.Visibility = Visibility.Visible;

                foreach (SteamCuenta cuenta in cuentas)
                {
                    bool fallo = false;

                    try
                    {
                        await carpetaCuentas.GetFolderAsync(cuenta.ID64);
                    }
                    catch
                    {
                        fallo = true;
                    }

                    if (fallo == true)
                    {
                        await carpetaCuentas.CreateFolderAsync(cuenta.ID64, CreationCollisionOption.ReplaceExisting);
                    }

                    //-----------------------------------------------------

                    Grid grid = new Grid
                    {
                        MinWidth = 400,
                        Margin = new Thickness(0, 5, 0, 5)
                    };

                    ColumnDefinition col1 = new ColumnDefinition();
                    ColumnDefinition col2 = new ColumnDefinition();

                    col1.Width = new GridLength(1, GridUnitType.Star);
                    col2.Width = new GridLength(1, GridUnitType.Auto);

                    grid.ColumnDefinitions.Add(col1);
                    grid.ColumnDefinitions.Add(col2);

                    //-----------------------------------------------

                    StackPanel spBoton1 = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    ImageEx imagenBoton1 = new ImageEx
                    {
                        IsCacheEnabled = true,
                        EnableLazyLoading = true,
                        Source = cuenta.Avatar,
                        Width = 50,
                        Height = 50,
                        Margin = new Thickness(0, 0, 20, 0),
                        CornerRadius = new CornerRadius(5)
                    };

                    spBoton1.Children.Add(imagenBoton1);

                    TextBlock tbBoton1 = new TextBlock
                    {
                        Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                        Text = cuenta.Nombre,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    spBoton1.Children.Add(tbBoton1);

                    SolidColorBrush colorBoton1 = new SolidColorBrush
                    {
                        Color = (Color)Application.Current.Resources["ColorPrimario"],
                        Opacity = 0.6
                    };

                    Button2 boton1 = new Button2
                    {
                        Content = spBoton1,
                        RequestedTheme = ElementTheme.Light,
                        CornerRadius = new CornerRadius(5),
                        Background = colorBoton1,
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(20),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        HorizontalContentAlignment = HorizontalAlignment.Left,
                        Tag = cuenta
                    };

                    boton1.Click += Juegos.CargarLista;
                    boton1.PointerEntered += Animaciones.EntraRatonBoton2;
                    boton1.PointerExited += Animaciones.SaleRatonBoton2;

                    boton1.SetValue(Grid.ColumnProperty, 0);
                    grid.Children.Add(boton1);

                    if (datos.Values["OpcionesCuentasMensajes"] != null)
                    {
                        if ((bool)datos.Values["OpcionesCuentasMensajes"] == true)
                        {
                            if (cuentaSeñalar != null)
                            {
                                if (cuentaSeñalar.ID64 == cuenta.ID64)
                                {
                                    ResourceLoader recursos = new ResourceLoader();

                                    SymbolIconSource iconoConsejo = new SymbolIconSource
                                    {
                                        Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                                        Symbol = Symbol.Important
                                    };

                                    TeachingTip consejo = new TeachingTip
                                    {
                                        Target = boton1,
                                        IsOpen = true,
                                        RequestedTheme = ElementTheme.Dark,
                                        Subtitle = recursos.GetString("AdviceLoadAccount"),
                                        Title = recursos.GetString("Advice"),
                                        IconSource = iconoConsejo,
                                        Tag = grid,
                                        Background = new SolidColorBrush((Color)Application.Current.Resources["ColorPrimario"])
                                    };

                                    consejo.CloseButtonClick += Consejos.CerrarConsejo;

                                    grid.Children.Add(consejo);
                                }
                            }
                        }
                    }
                                           
                    //-----------------------------------------------

                    StackPanel spBoton2 = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    FontAwesome icono2 = new FontAwesome
                    {
                        Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                        Icon = EFontAwesomeIcon.Solid_Xmark
                    };

                    spBoton2.Children.Add(icono2);

                    Button2 boton2 = new Button2
                    {
                        Content = spBoton2,
                        RequestedTheme = ElementTheme.Light,
                        CornerRadius = new CornerRadius(5),
                        Background = colorBoton1,
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(12, 10, 12, 10),
                        Tag = cuenta,
                        Margin = new Thickness(20, 0, 0, 0)
                    };

                    boton2.Click += BorrarCuenta;
                    boton2.PointerEntered += Animaciones.EntraRatonBoton2;
                    boton2.PointerExited += Animaciones.SaleRatonBoton2;

                    boton2.SetValue(Grid.ColumnProperty, 1);
                    grid.Children.Add(boton2);

                    //-----------------------------------------------

                    ObjetosVentana.spCuentasAñadidas.Children.Add(grid);
                }
            }
            else
            {
                ObjetosVentana.gridCuentasAñadidas.Visibility = Visibility.Collapsed;
            }
        }

        public static void NoAñadirCuenta(object sender, RoutedEventArgs e)
        {
            ObjetosVentana.spCuentasAñadir.Visibility = Visibility.Visible;
            ObjetosVentana.spCuentasPreguntar.Visibility = Visibility.Collapsed;

            ObjetosVentana.tbCuentasAñadir.Text = string.Empty;
        }

        public static void ActivarDesactivar(bool estado)
        {
            ObjetosVentana.tbCuentasAñadir.IsEnabled = estado;

            ObjetosVentana.botonCuentasAñadirSi.IsEnabled = estado;
            ObjetosVentana.botonCuentasAñadirNo.IsEnabled = estado;

            foreach (Grid grid in ObjetosVentana.spCuentasAñadidas.Children)
            {
                foreach (Object objeto in grid.Children) 
                {
                    if (objeto.GetType() == typeof(Button2))
                    {
                        Button2 boton = objeto as Button2;
                        boton.IsEnabled = estado;
                    }
                }
            }
        }
    }

    //----------------------------------------------

    public class SteamCuenta
    {
        public string ID64 { get; set; }
        public string Nombre { get; set; }
        public string Avatar { get; set; }
    }

    //----------------------------------------------

    public class SteamCuentaAPI
    {
        [JsonProperty("response")]
        public SteamCuentaAPIDatos Datos { get; set; }
    }

    public class SteamCuentaAPIDatos
    {
        [JsonProperty("players")]
        public List<SteamCuentaAPIJugador> Jugador { get; set; }
    }

    public class SteamCuentaAPIJugador
    {
        [JsonProperty("steamid")]
        public string ID64 { get; set; }

        [JsonProperty("personaname")]
        public string Nombre { get; set; }

        [JsonProperty("avatarfull")]
        public string Avatar { get; set; }
    }

    //----------------------------------------------

    public class SteamSacarID
    {
        [JsonProperty("response")]
        public SteamSacarID2 Datos { get; set; }
    }

    public class SteamSacarID2
    {
        [JsonProperty("steamid")]
        public string ID64 { get; set; }
    }
}
