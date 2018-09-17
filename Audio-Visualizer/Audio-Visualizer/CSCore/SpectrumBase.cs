using CSCore;
using CSCore.DSP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio_Visualizer.CSCore
{
    public class SpectrumBase : INotifyPropertyChanged
    {
        #region ----PROPERTIES----
        public int MaxFrequency
        {
            get { return m_MaxFrequency; }
            set
            {
                if (value <= MinFrequency)
                {
                    throw new ArgumentOutOfRangeException("value", "Value must not be less than or equal to the minimum value");
                }

                m_MaxFrequency = value;
                UpdateFrequencyMapping();
                RaisePropertyChanged("MaxFrequency");
            }
        }

        public int MinFrequency
        {
            get { return m_MinFrequency; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Value must be greater than or equal to zero");
                }

                m_MinFrequency = value;
                UpdateFrequencyMapping();
                RaisePropertyChanged("MinFrequency");
            }
        }

        [BrowsableAttribute(false)]
        public ISpectrumProvider SpectrumProvider
        {
            get { return m_SpectrumProvider; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                m_SpectrumProvider = value;
                RaisePropertyChanged("SpectrumProvider");
            }
        }

        public bool UseLogScale
        {
            get { return m_UseLogScale; }
            set
            {
                m_UseLogScale = value;
                UpdateFrequencyMapping();
                RaisePropertyChanged("UseLogScale");
            }
        }

        public ScalingStrategy ScalingStrategy
        {
            get { return m_ScalingStrategy; }
            set
            {
                m_ScalingStrategy = value;
                RaisePropertyChanged("ScalingStrategy");
            }
        }

        public bool UseAverage
        {
            get { return m_UseAverage; }
            set
            {
                m_UseAverage = true;
                RaisePropertyChanged("UseAverage");
            }
        }

        [BrowsableAttribute(false)]
        public FftSize FftSize
        {
            get { return (FftSize)m_FFTSize; }
            set
            {
                if ((int)Math.Log((int)value, 2) % 1 != 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }

                m_FFTSize = (int)value;
                m_MaxFFTIndex = m_FFTSize / 2 - 1;
                RaisePropertyChanged("FFTSize");
            }
        }
        #endregion

        #region ----CONFIG----
        protected const int ScaleFactorLinear = 1;
        protected const int ScaleFactorSqr = 1;
        protected const double MinDBValue = -90;
        protected const double MaxDBValue = 10;
        protected const double DBScale = (MaxDBValue - MinDBValue);
        protected int SpectrumResolution;

        private int m_FFTSize;
        private bool m_UseLogScale;
        private bool m_UseAverage;
        private int m_MaxFFTIndex;
        private int m_MaxFrequency = 12000;   //Hz
        private int m_MaxFrequencyIndex;
        private int m_MinFrequency = 20;
        private int m_MinFrequencyIndex;
        private ScalingStrategy m_ScalingStrategy;
        private int[] m_SpectrumIndexMax;
        private int[] m_SpectrumLogScaleIndexMax;
        private ISpectrumProvider m_SpectrumProvider;
        #endregion

        #region ----EVENTS----
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        protected virtual void UpdateFrequencyMapping()
        {
            m_MaxFrequencyIndex = Math.Min(m_SpectrumProvider.GetFftBandIndex(MaxFrequency) + 1, m_MaxFFTIndex);
            m_MinFrequencyIndex = Math.Min(m_SpectrumProvider.GetFftBandIndex(MinFrequency), m_MaxFFTIndex);

            int actualResolution = SpectrumResolution;

            int indexCount = m_MaxFrequencyIndex - m_MinFrequencyIndex;
            double linearIndexBucketSize = Math.Round(indexCount / (double)actualResolution, 3);

            m_SpectrumIndexMax = m_SpectrumIndexMax.CheckBuffer(actualResolution, true);
            m_SpectrumLogScaleIndexMax = m_SpectrumLogScaleIndexMax.CheckBuffer(actualResolution, true);

            double maxLog = Math.Log(actualResolution, actualResolution);
            for (int i = 1; i < actualResolution; i++)
            {
                int logIndex = (int)((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) + m_MinFrequencyIndex;

                m_SpectrumIndexMax[i - 1] = m_MinFrequencyIndex + (int)(i * linearIndexBucketSize);
                m_SpectrumLogScaleIndexMax[i - 1] = logIndex;
            }

            if (actualResolution > 0)
            {
                m_SpectrumIndexMax[m_SpectrumIndexMax.Length - 1] = m_SpectrumLogScaleIndexMax[m_SpectrumLogScaleIndexMax.Length - 1] = m_MaxFrequencyIndex;
            }
        }

        protected virtual SpectrumPointData[] CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
        {
            var dataPoints = new List<SpectrumPointData>();

            double value0 = 0;
            double value = 0;
            double lastValue = 0;
            double actualMaxValue = maxValue;
            int spectrumPointIndex = 0;

            for (int i = m_MinFrequencyIndex; i <= m_MaxFrequencyIndex; i++)
            {
                switch (ScalingStrategy)
                {
                    case ScalingStrategy.Decibel:
                        value0 = (((20 * Math.Log10(fftBuffer[i])) - MinDBValue) / DBScale) * actualMaxValue;
                        break;
                    case ScalingStrategy.Linear:
                        value0 = (fftBuffer[i] * ScaleFactorLinear) * actualMaxValue;
                        break;
                    case ScalingStrategy.Sqrt:
                        value0 = ((Math.Sqrt(fftBuffer[i])) * ScaleFactorSqr) * actualMaxValue;
                        break;
                }

                bool recalc = true;
                value = Math.Max(0, Math.Max(value0, value));
                while (spectrumPointIndex <= m_SpectrumIndexMax.Length - 1 && i == (UseLogScale ? m_SpectrumLogScaleIndexMax[spectrumPointIndex] : m_SpectrumIndexMax[spectrumPointIndex]))
                {
                    if (!recalc) { value = lastValue; }
                    if (value > maxValue) { value = maxValue; }
                    if (m_UseAverage && spectrumPointIndex > 0) { value = (lastValue + value) / 2.0; }

                    dataPoints.Add(new SpectrumPointData { SpectrumPointIndex = spectrumPointIndex, Value = value });

                    lastValue = value;
                    value = 0.0f;
                    spectrumPointIndex++;
                    recalc = false;
                }
            }

            return dataPoints.ToArray();
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null && !String.IsNullOrEmpty(propertyName))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [DebuggerDisplay("{Value}")]
        protected struct SpectrumPointData
        {
            public int SpectrumPointIndex;
            public double Value;
        }
    }
}
