using CommunityToolkit.WinUI.UI.Controls;
using Interfaz;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using Expander = Microsoft.UI.Xaml.Controls.Expander;

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
            Steam.Juegos.Cargar();
            Steam.Logros.Cargar();
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

            ObjetosVentana.spJuegosBuscador = spJuegosBuscador;
            ObjetosVentana.tbJuegosBuscador = tbJuegosBuscador;
            ObjetosVentana.svJuegos = svJuegos;
            ObjetosVentana.gvJuegos = gvJuegos;
            ObjetosVentana.gvJuegosBuscador = gvJuegosBuscador;
            ObjetosVentana.tTipJuegos = tTipJuegos;

            //-------------------------------------------------------------------

            ObjetosVentana.svLogros = svLogros;
            ObjetosVentana.spLogrosCabecera = spLogrosCabecera;
            ObjetosVentana.imagenLogrosCabecera = imagenLogrosCabecera;
            ObjetosVentana.gridLogrosTrialMensaje = gridLogrosTrialMensaje;
            ObjetosVentana.botonLogrosTrialComprarApp = botonLogrosTrialComprarApp;
            ObjetosVentana.botonLogrosSteam = botonLogrosSteam;
            ObjetosVentana.botonLogrosDetallesSteam = botonLogrosDetallesSteam;
            ObjetosVentana.botonLogrosHubSteam = botonLogrosHubSteam;
            ObjetosVentana.botonLogrosGuiasSteam = botonLogrosGuiasSteam;
            ObjetosVentana.expanderLogrosCompletados = expanderLogrosCompletados;
            ObjetosVentana.spLogrosCompletados = spLogrosCompletados;
            ObjetosVentana.tbLogrosCompletadosInfo = tbLogrosCompletadosInfo;
            ObjetosVentana.expanderLogrosPendientes = expanderLogrosPendientes;
            ObjetosVentana.spLogrosPendientes = spLogrosPendientes;
            ObjetosVentana.tbLogrosPendientesInfo = tbLogrosPendientesInfo;
            ObjetosVentana.tTipLogros = tTipLogros;

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

            public static StackPanel spJuegosBuscador { get; set; }
            public static TextBox tbJuegosBuscador { get; set; }
            public static ScrollViewer svJuegos { get; set; }
            public static AdaptiveGridView gvJuegos { get; set; }
            public static AdaptiveGridView gvJuegosBuscador { get; set; }
            public static TeachingTip tTipJuegos { get; set; }

            //-------------------------------------------------------------------

            public static ScrollViewer svLogros { get; set; }
            public static StackPanel spLogrosCabecera { get; set; }
            public static Image imagenLogrosCabecera { get; set; }
            public static Grid gridLogrosTrialMensaje { get; set; }
            public static Button botonLogrosTrialComprarApp { get; set; }
            public static Button botonLogrosSteam { get; set; }
            public static Button botonLogrosDetallesSteam { get; set; }
            public static Button botonLogrosHubSteam { get; set; }
            public static Button botonLogrosGuiasSteam { get; set; }
            public static Expander expanderLogrosCompletados { get; set; }
            public static StackPanel spLogrosCompletados { get; set; }
            public static TextBlock tbLogrosCompletadosInfo { get; set; }
            public static Expander expanderLogrosPendientes { get; set; }
            public static StackPanel spLogrosPendientes { get; set; }
            public static TextBlock tbLogrosPendientesInfo { get; set; }
            public static TeachingTip tTipLogros { get; set; }

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

                            ObjetosVentana.spJuegosBuscador.Visibility = Visibility.Collapsed;
                        }
                        else if (numero == 3)
                        {
                            Pestañas.Visibilidad(gridJuegos);
                            BarraTitulo.CambiarTitulo(null);

                            if (ObjetosVentana.nvPrincipal.MenuItems.Count > 2)
                            {
                                int i = ObjetosVentana.nvPrincipal.MenuItems.Count;
                                int j = 0;
                                while (i > j)
                                {
                                    i -= 1;

                                    if (i > 3)
                                    {
                                        ObjetosVentana.nvPrincipal.MenuItems.RemoveAt(i);
                                    }
                                }
                            }

                            ObjetosVentana.spJuegosBuscador.Visibility = Visibility.Visible;
                        }
                        else if (numero == 5)
                        {
                            Pestañas.Visibilidad(gridLogros);
                            BarraTitulo.CambiarTitulo(null);

                            ObjetosVentana.spJuegosBuscador.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
        }
    }
}