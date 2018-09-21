using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

    internal static class IconUtilities
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static ImageSource ToImageSource(this Icon icon)
        {
            Bitmap bitmap = icon.ToBitmap();
            IntPtr hBitmap = bitmap.GetHbitmap();

            ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!DeleteObject(hBitmap))
            {
                throw new Win32Exception();
            }

            return wpfBitmap;
        }
    }
}