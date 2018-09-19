using System;
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

        public static System.Drawing.Color Lerp(System.Windows.Media.Color a, System.Windows.Media.Color b, double t)
        {
            //clamp t
            t = Math.Min(Math.Max(t, 0), 1);

            //lerp values
            int A = (int)Lerp(a.A, b.A, t);
            int R = (int)Lerp(a.R, b.R, t);
            int G = (int)Lerp(a.G, b.G, t);
            int B = (int)Lerp(a.B, b.B, t);

            return System.Drawing.Color.FromArgb(A, R, G, B);
        }
    }
}