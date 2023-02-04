using CommunityToolkit.WinUI.UI.Controls;
using FontAwesome6.Fonts;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.ApplicationModel.Resources;
using Windows.UI;
using static Steam_Achievements_WinUI.MainWindow;

namespace Interfaz
{
    public static class Pestañas
    {
        public static void Cargar()
        {
            ResourceLoader recursos = new ResourceLoader();

            ObjetosVentana.nvPrincipal.MenuItems.RemoveAt(0);
            ObjetosVentana.nvPrincipal.MenuItems.Insert(0, ObjetosVentana.nvItemMenu);

            ObjetosVentana.nvItemMenu.PointerEntered += EntraRatonNvItemMenu;
            ObjetosVentana.nvItemMenu.PointerEntered += Animaciones.EntraRatonNvItem2;
            ObjetosVentana.nvItemMenu.PointerExited += Animaciones.SaleRatonNvItem2;

            TextBlock tbOpcionesTt = new TextBlock
            {
                Text = recursos.GetString("Options")
            };

            ToolTipService.SetToolTip(ObjetosVentana.nvItemOpciones, tbOpcionesTt);
            ToolTipService.SetPlacement(ObjetosVentana.nvItemOpciones, PlacementMode.Bottom);

            ObjetosVentana.nvItemOpciones.PointerEntered += Animaciones.EntraRatonNvItem2;
            ObjetosVentana.nvItemOpciones.PointerExited += Animaciones.SaleRatonNvItem2;
        }

        public static void Visibilidad(Grid grid)
        {
            ObjetosVentana.nvItemSubirArriba.Visibility = Visibility.Collapsed;

            ObjetosVentana.gridCuentas.Visibility = Visibility.Collapsed;
            ObjetosVentana.gridJuegos.Visibility = Visibility.Collapsed;
            ObjetosVentana.gridLogros.Visibility = Visibility.Collapsed;
            ObjetosVentana.gridOpciones.Visibility = Visibility.Collapsed;

            grid.Visibility = Visibility.Visible;
        }

        public static void CreadorItems(string imagenEnlace, string nombre, int posicion)
        {
            StackPanel2 sp = new StackPanel2
            {
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(5),
                Orientation = Orientation.Horizontal,
                Height = 30,
                Tag = posicion
            };

            sp.PointerEntered += Animaciones.EntraRatonStackPanel2;
            sp.PointerExited += Animaciones.SaleRatonStackPanel2;

            if (imagenEnlace != null)
            {
                ImageEx imagen = new ImageEx
                {
                    Source = imagenEnlace,
                    IsCacheEnabled = true,
                    EnableLazyLoading = true,
                    MaxHeight = 30,
                    MaxWidth = 30,
                    CornerRadius = new CornerRadius(2),
                    Margin = new Thickness(0, 0, 15, 0)
                };

                sp.Children.Add(imagen);
            }

            TextBlock tb = new TextBlock
            {
                Text = nombre,
                Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                VerticalAlignment = VerticalAlignment.Center
            };

            sp.Children.Add(tb);

            if (nombre != null)
            {
                TextBlock tbTt = new TextBlock
                {
                    Text = nombre
                };

                ToolTipService.SetToolTip(sp, tbTt);
                ToolTipService.SetPlacement(sp, PlacementMode.Bottom);
            }

            ObjetosVentana.nvPrincipal.MenuItems.Insert(posicion, sp);
        }

        public static void CrearSeparador(int posicion)
        {
            StackPanel2 sp = new StackPanel2
            {
                CornerRadius = new CornerRadius(3),
                Padding = new Thickness(0),
                Orientation = Orientation.Horizontal,
                Height = 30,
                Tag = posicion,
                IsHitTestVisible = false
            };

            FontAwesome icono = new FontAwesome
            {
                Foreground = new SolidColorBrush((Color)Application.Current.Resources["ColorFuente"]),
                Icon = FontAwesome6.EFontAwesomeIcon.Solid_ChevronRight
            };

            sp.Children.Add(icono);

            ObjetosVentana.nvPrincipal.MenuItems.Insert(posicion, sp);
        }

        public static void EntraRatonNvItemMenu(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout(sender as NavigationViewItem);
        }
    }
}
