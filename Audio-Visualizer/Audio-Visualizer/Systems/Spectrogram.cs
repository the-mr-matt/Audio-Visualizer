using System.Drawing;
using Audio_Visualizer.Other;
using Audio_Visualizer.UI;

namespace Audio_Visualizer.Systems
{
    public class Spectrogram : DrawBitmap
    {
        #region ----CONSTRUCTOR----
        public Spectrogram(int width, int height, double multiplier) : base(width, height, multiplier) { }
        #endregion
        
        /// <summary>
        /// Calculate the colors
        /// </summary>
        protected override bool GetColors(Graphics graphics, float lineThickness)
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
                        float pointHeight = (float)m_Bitmap.Size.Height / (float)spectrumPoints.Length;

                        //get the color based on the fft band value
                        double value = p.Value * m_Multiplier;

                        pen.Color = Utils.Lerp(ColorPalette.PanelBG, ColorPalette.Accent, value);

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
    }
}