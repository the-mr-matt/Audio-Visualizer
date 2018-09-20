using Audio_Visualizer.CSCore;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using CSCore.DSP;
using System.Windows.Media;

namespace Audio_Visualizer.Systems
{
    public class DrawBitmap : SpectrumBase
    {
        #region ----CONSTRUCTOR----
        public DrawBitmap(int width, int height, double multiplier)
        {
            m_Bitmap = new Bitmap(width, height);
            m_Multiplier = multiplier;

            FftSize = FftSize.Fft1024;
            SpectrumProvider = Analyzer.SpectrumProvider;
            UseAverage = false;
            UseLogScale = true;
            ScalingStrategy = ScalingStrategy.Sqrt;
            SpectrumResolution = height * 2;
        }
        #endregion

        #region ----STATE----
        protected int m_Position;
        protected double m_Multiplier;
        protected bool m_IsInitialized;

        protected Bitmap m_Bitmap;
        #endregion

        #region ----CONFIG----
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        #endregion

        /// <summary>
        /// Initialize the bitmap
        /// </summary>
        public System.Windows.Media.Brush CreateBitmap()
        {
            using (Graphics g = Graphics.FromImage(m_Bitmap))
            {
                if (GetColors(g, 1))
                {
                    m_Position++;

                    if (m_Position > m_Bitmap.Width)
                    {
                        m_Position = 0;
                    }
                }

                ImageBrush brush = new ImageBrush(ConvertToBitmapSource(m_Bitmap))
                {
                    Stretch = Stretch.Fill
                };
                return brush;
            }
        }

        protected virtual bool GetColors(Graphics graphics, float lineThickness)
        {
            return false;
        }

        protected BitmapSource ConvertToBitmapSource(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }
            else
            {
                IntPtr hBitmap = bitmap.GetHbitmap();
                try
                {
                    BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap
                    (
                        hBitmap,
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions()
                    );

                    return bitmapSource;
                }
                finally
                {
                    DeleteObject(hBitmap);
                }
            }
        }
    }
}
