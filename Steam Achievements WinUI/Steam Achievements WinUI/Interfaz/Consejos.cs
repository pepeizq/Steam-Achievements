using Microsoft.UI.Xaml.Controls;

namespace Interfaz
{
    public static class Consejos
    {
        public static void CerrarConsejo(TeachingTip sender, object e)
        {
            TeachingTip consejo = sender as TeachingTip;
            consejo.IsOpen = false;
           
            Grid grid = consejo.Tag as Grid;
            
            int i = 0;
            foreach (object hijo in grid.Children)
            {
                if (hijo.GetType() == typeof(TeachingTip))
                {
                    break;
                }

                i += 1;
            }

            if (i > 0)
            {
                grid.Children.RemoveAt(i);
            }
        }
    }
}
