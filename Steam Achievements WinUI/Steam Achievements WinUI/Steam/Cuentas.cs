using CommunityToolkit.WinUI.UI.Controls;
using Herramientas;
using Interfaz;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
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
            ObjetosVentana.tbCuentasAñadir.TextChanged += DetectarEnlaceCuenta;

            ObjetosVentana.botonCuentasAñadirSi.Click += AñadirCuenta;
            ObjetosVentana.botonCuentasAñadirSi.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonCuentasAñadirSi.PointerExited += Animaciones.SaleRatonBoton2;

            ObjetosVentana.botonCuentasAñadirNo.Click += NoAñadirCuenta;
            ObjetosVentana.botonCuentasAñadirNo.PointerEntered += Animaciones.EntraRatonBoton2;
            ObjetosVentana.botonCuentasAñadirNo.PointerExited += Animaciones.SaleRatonBoton2;

            //-------------------------------------------

            ActualizarCuentas();
        }

        public static async void DetectarEnlaceCuenta(object sender, TextChangedEventArgs e)
        {           
            ActivarDesactivar(false);
            ObjetosVentana.prCuentasCargar.Visibility = Visibility.Visible;

            ObjetosVentana.spCuentasAvisoYaAñadida.Visibility = Visibility.Collapsed;

            await Task.Delay(100);

            TextBox tb = sender as TextBox;

            if (tb.Text.Length > 0) 
            {
                string id64 = string.Empty;

                if (tb.Text.Contains("https://steamcommunity.com/id/") == true)
                {
                    string usuario = tb.Text;

                    if (usuario.Contains("?") == true) 
                    { 
                        int int1 = usuario.IndexOf("?");
                        usuario = usuario.Remove(int1, usuario.Length - int1);
                    }

                    usuario = usuario.Replace("https://steamcommunity.com/id/", null);
                    usuario = usuario.Replace("http://steamcommunity.com/id/", null);
                    usuario = usuario.Replace("/", null);

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

                if (id64 != string.Empty)
                {
                    string html = await Decompiladores.CogerHtml("https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key=41F2D73A0B5024E9101F8D4E8D8AC21E&steamids=" + id64);
                
                    if (html != null)
                    {
                        SteamCuentaAPI cuenta = JsonConvert.DeserializeObject<SteamCuentaAPI>(html);

                        if (cuenta != null)
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
                                ObjetosVentana.spCuentasAvisoYaAñadida.Visibility = Visibility.Visible;
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

            StorageFile ficheroCuentas = await carpetaCuentas.GetFileAsync(nombreFicheroCuentas);
        
            if (ficheroCuentas != null)
            {
                string textoCuentas = await FileIO.ReadTextAsync(ficheroCuentas);

                if (textoCuentas != null)
                {
                    cuentas = JsonConvert.DeserializeObject<List<SteamCuenta>>(textoCuentas);
                }
            }

            return cuentas;
        }

        public static async void AñadirCuenta(object sender, RoutedEventArgs e)
        {
            List<SteamCuenta> cuentas = await LeerCuentas();

            SteamCuenta cuentaAñadir = ObjetosVentana.botonCuentasAñadirSi.Tag as SteamCuenta;

            cuentas.Add(cuentaAñadir);

            string textoCuentasAñadido = JsonConvert.SerializeObject(cuentas, Formatting.Indented);

            StorageFile ficheroCuentas = await carpetaCuentas.GetFileAsync(nombreFicheroCuentas);
            await FileIO.WriteTextAsync(ficheroCuentas, textoCuentasAñadido);

            ActualizarCuentas();
        }

        public static async void ActualizarCuentas()
        {
            ObjetosVentana.spCuentasAñadidas.Children.Clear();

            List<SteamCuenta> cuentas = await LeerCuentas();
           
            if (cuentas.Count > 0)
            {
                ObjetosVentana.gridCuentasAñadidas.Visibility = Visibility.Visible;

                foreach (SteamCuenta cuenta in cuentas)
                {
                    StackPanel sp = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };

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
                        Margin = new Thickness(0, 0, 20, 0)
                    };

                    spBoton1.Children.Add(imagenBoton1);

                    TextBlock tbBoton1 = new TextBlock
                    {
                        Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                        MaxWidth = 500,
                        Text = cuenta.Nombre,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    spBoton1.Children.Add(tbBoton1);

                    Button2 boton1 = new Button2
                    {
                        Content = spBoton1,
                        RequestedTheme = ElementTheme.Dark,
                        CornerRadius = new CornerRadius(5),
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(20)
                    };

                    sp.Children.Add(boton1);

                    //-----------------------------------------------


                    //-----------------------------------------------

                    ObjetosVentana.spCuentasAñadidas.Children.Add(sp);
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
