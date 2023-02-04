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
            Pestañas.Cargar();
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
            ObjetosVentana.gridJuegos = gridJuegos;
            ObjetosVentana.gridLogros = gridLogros;
            ObjetosVentana.gridOpciones = gridOpciones;

            //-------------------------------------------------------------------

            ObjetosVentana.gridCuentasAñadidas = gridCuentasAnadidas;
            ObjetosVentana.spCuentasAñadidas = spCuentasAnadidas;
            ObjetosVentana.spCuentasAñadir = spCuentasAnadir;
            ObjetosVentana.prCuentasCargar = prCuentasCargar;
            ObjetosVentana.tbCuentasAñadir = tbCuentasAnadir;
            ObjetosVentana.tTipCuentasAviso = tTipCuentasAviso;
            ObjetosVentana.spCuentasAvisoPermisos = spCuentasAvisoPermisos;
            ObjetosVentana.botonCuentasAvisoPermisos = botonCuentasAvisoPermisos;
            ObjetosVentana.spCuentasPreguntar = spCuentasPreguntar;
            ObjetosVentana.imagenCuentasPreguntarAñadir = imagenCuentasPreguntarAnadir;
            ObjetosVentana.tbCuentasPreguntarAñadir = tbCuentasPreguntarAnadir;
            ObjetosVentana.botonCuentasAñadirSi = botonCuentasAnadirSi;
            ObjetosVentana.botonCuentasAñadirNo = botonCuentasAnadirNo;

            //-------------------------------------------------------------------

            ObjetosVentana.svJuegos = svJuegos;
            ObjetosVentana.prJuegos = prJuegos;
            ObjetosVentana.gvJuegos = gvJuegos;

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
            public static Grid gridJuegos { get; set; }
            public static Grid gridLogros { get; set; }
            public static Grid gridOpciones { get; set; }

            //-------------------------------------------------------------------

            public static Grid gridCuentasAñadidas { get; set; }
            public static StackPanel spCuentasAñadidas { get; set; }
            public static StackPanel spCuentasAñadir { get; set; }
            public static ProgressRing prCuentasCargar { get; set; }
            public static TextBox tbCuentasAñadir { get; set; }
            public static TeachingTip tTipCuentasAviso { get; set; }
            public static StackPanel spCuentasAvisoPermisos { get; set; }
            public static Button botonCuentasAvisoPermisos { get; set; }
            public static StackPanel spCuentasPreguntar { get; set; }
            public static ImageEx imagenCuentasPreguntarAñadir { get; set; }
            public static TextBlock tbCuentasPreguntarAñadir { get; set; }
            public static Button botonCuentasAñadirSi { get; set; }
            public static Button botonCuentasAñadirNo { get; set; }

            //-------------------------------------------------------------------

            public static ScrollViewer svJuegos { get; set; }
            public static ProgressRing prJuegos { get; set; }
            public static AdaptiveGridView gvJuegos { get; set; }

            //-------------------------------------------------------------------

            public static ScrollViewer svOpciones { get; set; }
            public static ComboBox cbOpcionesIdioma { get; set; }
            public static ComboBox cbOpcionesPantalla { get; set; }
            public static Button botonOpcionesLimpiar { get; set; }
        }

        private void nvPrincipal_Loaded(object sender, RoutedEventArgs e)
        {
            ResourceLoader recursos = new ResourceLoader();
   
            Pestañas.CreadorItems(null, recursos.GetString("Accounts"), 1);

            //StackPanel sp = (StackPanel)ObjetosVentana.nvPrincipal.MenuItems[1];
            //Pestañas.Visibilidad(gridSkins, true, sp, true);
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
                        Pestañas.Visibilidad(gridOpciones);
                        BarraTitulo.CambiarTitulo(recursos.GetString("Options"));
                        ScrollViewers.EnseñarSubir(svOpciones);
                    }
                }
            }

            if (args.InvokedItem != null)
            {
                if (args.InvokedItem.GetType() == typeof(StackPanel2))
                {
                    StackPanel2 sp = (StackPanel2)args.InvokedItem;

                    if (sp.Tag != null)
                    {
                        int numero = (int)sp.Tag;

                        if (numero == 1)
                        {
                            Pestañas.Visibilidad(gridCuentas);
                            BarraTitulo.CambiarTitulo(null);

                            if (ObjetosVentana.nvPrincipal.MenuItems.Count > 2)
                            {
                                int i = ObjetosVentana.nvPrincipal.MenuItems.Count;
                                int j = 0;
                                while (i > j)
                                {
                                    i -= 1;

                                    if (i > 1)
                                    {
                                        ObjetosVentana.nvPrincipal.MenuItems.RemoveAt(i);
                                    }          
                                }
                            }
                        }
                        else if (numero == 3)
                        {
                            Pestañas.Visibilidad(gridJuegos);
                            BarraTitulo.CambiarTitulo(null);


                        }
                    }
                }
            }
        }
    }
}