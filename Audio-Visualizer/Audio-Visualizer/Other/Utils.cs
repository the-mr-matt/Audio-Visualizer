using System.Windows;

namespace Audio_Visualizer.Other
{
    public class Utils
    {
        public static Thickness ZeroMargin
        {
            get { return new Thickness() { Top = 0, Bottom = 0, Left = 0, Right = 0 }; ; }
        }

        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }
    }
}