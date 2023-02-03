using CommunityToolkit.WinUI.UI.Controls;
using Interfaz;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;

namespace Steam_Achievements_WinUI
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();

            CargarObjetosVentana();

            BarraTitulo.Generar(this);
            BarraTitulo.CambiarTitulo(null);
            Pesta�as.Cargar();
            ScrollViewers.Cargar();
            Interfaz.Menu.Cargar();
            Opciones.CargarDatos();
            Steam.Cuentas.Cargar();
        }

        public void CargarObjetosVentana()
        {
            ObjetosVentana.ventana = ventana;
            ObjetosVentana.gridTitulo = gridTitulo;
            ObjetosVentana.tbTitulo = tbTitulo;
            ObjetosVentana.nvPrincipal = nvPrincipal;
            ObjetosVentana.nvItemMenu = nvItemMenu;
            ObjetosVentana.menuItemMenu = menuItemMenu;
            ObjetosVentana.nvItemOpciones = nvItemOpciones;
            ObjetosVentana.nvItemSubirArriba = nvItemSubirArriba;

            //-------------------------------------------------------------------

            ObjetosVentana.gridCuentas = gridCuentas;
            ObjetosVentana.gridOpciones = gridOpciones;

            //-------------------------------------------------------------------

            ObjetosVentana.gridCuentasA�adidas = gridCuentasAnadidas;
            ObjetosVentana.spCuentasA�adidas = spCuentasAnadidas;
            ObjetosVentana.spCuentasA�adir = spCuentasAnadir;
            ObjetosVentana.prCuentasCargar = prCuentasCargar;
            ObjetosVentana.tbCuentasA�adir = tbCuentasAnadir;
            ObjetosVentana.spCuentasAvisoYaA�adida = spCuentasAvisoYaAnadida;
            ObjetosVentana.spCuentasPreguntar = spCuentasPreguntar;
            ObjetosVentana.imagenCuentasPreguntarA�adir = imagenCuentasPreguntarAnadir;
            ObjetosVentana.tbCuentasPreguntarA�adir = tbCuentasPreguntarAnadir;
            ObjetosVentana.botonCuentasA�adirSi = botonCuentasAnadirSi;
            ObjetosVentana.botonCuentasA�adirNo = botonCuentasAnadirNo;

            //-------------------------------------------------------------------

            ObjetosVentana.svOpciones = svOpciones;
            ObjetosVentana.cbOpcionesIdioma = cbOpcionesIdioma;
            ObjetosVentana.cbOpcionesPantalla = cbOpcionesPantalla;
            ObjetosVentana.botonOpcionesLimpiar = botonOpcionesLimpiar;
        }

        public static class ObjetosVentana
        {
            public static Window ventana { get; set; }
            public static Grid gridTitulo { get; set; }
            public static TextBlock tbTitulo { get; set; }
            public static NavigationView nvPrincipal { get; set; }
            public static NavigationViewItem nvItemMenu { get; set; }
            public static MenuFlyout menuItemMenu { get; set; }
            public static NavigationViewItem nvItemOpciones { get; set; }
            public static NavigationViewItem nvItemSubirArriba { get; set; }

            //-------------------------------------------------------------------

            public static Grid gridCuentas { get; set; }
            public static Grid gridOpciones { get; set; }

            //-------------------------------------------------------------------

            public static Grid gridCuentasA�adidas { get; set; }
            public static StackPanel spCuentasA�adidas { get; set; }
            public static StackPanel spCuentasA�adir { get; set; }
            public static ProgressRing prCuentasCargar { get; set; }
            public static TextBox tbCuentasA�adir { get; set; }
            public static StackPanel spCuentasAvisoYaA�adida { get; set; }
            public static StackPanel spCuentasPreguntar { get; set; }
            public static ImageEx imagenCuentasPreguntarA�adir { get; set; }
            public static TextBlock tbCuentasPreguntarA�adir { get; set; }
            public static Button botonCuentasA�adirSi { get; set; }
            public static Button botonCuentasA�adirNo { get; set; }

            //-------------------------------------------------------------------

            public static ScrollViewer svOpciones { get; set; }
            public static ComboBox cbOpcionesIdioma { get; set; }
            public static ComboBox cbOpcionesPantalla { get; set; }
            public static Button botonOpcionesLimpiar { get; set; }
        }

        private void nvPrincipal_Loaded(object sender, RoutedEventArgs e)
        {
            ResourceLoader recursos = new ResourceLoader();
   
            //Pesta�as.CreadorItems(FontAwesome6.EFontAwesomeIcon.Solid_Palette, recursos.GetString("Skins"));

            //StackPanel sp = (StackPanel)ObjetosVentana.nvPrincipal.MenuItems[1];
            //Pesta�as.Visibilidad(gridSkins, true, sp, true);
        }

        private void nvPrincipal_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            ResourceLoader recursos = new ResourceLoader();

            if (args.InvokedItemContainer != null)
            {
                if (args.InvokedItemContainer.GetType() == typeof(NavigationViewItem2))
                {
                    NavigationViewItem2 item = args.InvokedItemContainer as NavigationViewItem2;

                    if (item.Name == "nvItemMenu")
                    {

                    }
                    else if (item.Name == "nvItemOpciones")
                    {
                        Pesta�as.Visibilidad(gridOpciones, true, null, false);
                        BarraTitulo.CambiarTitulo(recursos.GetString("Options"));
                        ScrollViewers.Ense�arSubir(svOpciones);
                    }
                }
            }

            if (args.InvokedItem != null)
            {
                if (args.InvokedItem.GetType() == typeof(StackPanel2))
                {
                    StackPanel2 sp = (StackPanel2)args.InvokedItem;

                    if (sp.Children[1] != null)
                    {
                        if (sp.Children[1].GetType() == typeof(TextBlock))
                        {
                            TextBlock tb = sp.Children[1] as TextBlock;

                            if (tb.Text == recursos.GetString("Skins"))
                            {
                                //Pesta�as.Visibilidad(gridSkins, true, sp, true);
                                BarraTitulo.CambiarTitulo(null);
                                //ScrollViewers.Ense�arSubir(svSkins);
                            }
                        }
                    }
                }
            }
        }
    }
}