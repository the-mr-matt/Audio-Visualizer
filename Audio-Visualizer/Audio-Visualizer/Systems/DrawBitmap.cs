using System;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using System.Windows;
using Audio_Visualizer.CSCore;
using Audio_Visualizer.Other;
using Audio_Visualizer.UI;
using CSCore.DSP;

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
            SpectrumResolution = 300;
        }
        #endregion

        #region ----STATE----
        private int m_Position;
        private double m_Multiplier;
        private bool m_IsInitialized;

        private Bitmap m_Bitmap;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        #endregion

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

                ImageBrush brush = new ImageBrush(ConvertToBitmapSource(m_Bitmap));
                brush.Stretch = Stretch.Fill;
                return brush;
            }
        }

        private bool GetColors(Graphics graphics, float lineThickness)
        {
            if (!m_IsInitialized)
            {
                UpdateFrequencyMapping();
                m_IsInitialized = true;
            }

            var fftBuffer = new float[(int)Analyzer.FFTSize];

            //get the fft result from the spectrumprovider
            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                //prepare the fft result for rendering
                SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(1.0, fftBuffer);
                using (var pen = new System.Drawing.Pen(ColorPalette.PanelBG_Drawing, lineThickness))
                {
                    float currentYOffset = m_Bitmap.Size.Height;

                    //render the fft result
                    for (int i = 0; i < spectrumPoints.Length; i++)
                    {
                        SpectrumPointData p = spectrumPoints[i];

                        float xCoord = m_Position;
                        float pointHeight = m_Bitmap.Size.Height / spectrumPoints.Length;

                        //get the color based on the fft band value
                        pen.Color = Utils.Lerp(System.Windows.Media.Color.FromArgb(255, 255, 0, 0), ColorPalette.Accent, p.Value * m_Multiplier);

                        var p1 = new PointF(xCoord, currentYOffset);
                        var p2 = new PointF(xCoord, currentYOffset - pointHeight);

                        graphics.DrawLine(pen, p1, p2);

                        currentYOffset -= pointHeight;
                    }
                }
                return true;
            }
            return false;
        }

        private BitmapSource ConvertToBitmapSource(Bitmap bitmap)
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