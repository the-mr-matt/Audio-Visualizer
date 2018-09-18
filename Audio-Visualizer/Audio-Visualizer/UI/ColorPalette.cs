using System.Windows.Media;

namespace Audio_Visualizer.UI
{
    public class ColorPalette
    {
        public static Color Accent
        {
            get { return (Color)ColorConverter.ConvertFromString("#FF00A2FF"); }
        }
        
        public static Color Gray
        {
            get { return (Color)ColorConverter.ConvertFromString("#FF363636"); }
        }
    }
}
