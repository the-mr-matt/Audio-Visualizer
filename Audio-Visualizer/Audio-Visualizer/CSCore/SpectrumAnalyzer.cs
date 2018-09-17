using System;
using CSCore.DSP;

namespace Audio_Visualizer.CSCore
{
    public class SpectrumAnalyzer : SpectrumBase
    {
        #region ----CONSTRUCTOR----
        public SpectrumAnalyzer(FftSize fftSize)
        {
            FftSize = fftSize;
        }
        #endregion

        #region ----PROPERTIES----
        public int BarCount
        {
            get { return m_BarCount; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                m_BarCount = value;
                SpectrumResolution = value;

                UpdateFrequencyMapping();
                RaisePropertyChanged("BarCount");
                RaisePropertyChanged("BarWidth");
            }
        }
        #endregion

        #region ----CONFIG----
        private int m_BarCount;
        #endregion

        public double[] GetPointData(double multiplier)
        {
            float[] fftBuffer = new float[(int)FftSize];

            SpectrumProvider.GetFftData(fftBuffer, this);

            SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(multiplier, fftBuffer);
            
            double[] values = new double[spectrumPoints.Length];
            
            for (int i = 0; i < spectrumPoints.Length; i++)
            {
                SpectrumPointData p = spectrumPoints[i];
                values[i] = p.Value;
            }

            return values;
        }
    }
}
