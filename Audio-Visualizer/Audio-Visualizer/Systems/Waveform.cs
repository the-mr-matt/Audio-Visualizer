using Audio_Visualizer.UI;
using System.Drawing;


namespace Audio_Visualizer.Systems
{
    public class Waveform : DrawBitmap
    {
        #region ----CONSTRUCTOR----
        public Waveform(int width, int height, double multiplier) : base(width, height, multiplier) { }
        #endregion

        protected override bool GetColors(Graphics graphics, float lineThickness)
        {
            //get the fft result from the spectrumprovider
            using (var pen = new Pen(Audio_Visualizer.UI.ColorPalette.Accent_Drawing, lineThickness))
            {
                float halfHeight = m_Bitmap.Size.Height / 2f;
                float xCoord = m_Position;

                pen.Color = ColorPalette.PanelBG_Drawing;

                var p1 = new PointF(xCoord, halfHeight * 2f);
                var p2 = new PointF(xCoord, 0);

                graphics.DrawLine(pen, p1, p2);
                
                pen.Color = ColorPalette.Accent_Drawing;

                float value = halfHeight + Mixer.MasterPeak;

                p1.Y = halfHeight + Mixer.MasterPeak * halfHeight * (float)m_Multiplier;
                p2.Y = halfHeight - Mixer.MasterPeak * halfHeight * (float)m_Multiplier;

                graphics.DrawLine(pen, p1, p2);
            }

            return true;
        }
    }
}
