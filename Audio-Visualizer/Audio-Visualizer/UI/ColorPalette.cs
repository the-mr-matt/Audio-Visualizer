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

        public static Color PanelBG
        {
            get { return (Color)ColorConverter.ConvertFromString("#FF252525"); }
        }

        public static System.Drawing.Color PanelBG_Drawing
        {
            get { return System.Drawing.ColorTranslator.FromHtml("#FF252525"); }
        }
    }
}
