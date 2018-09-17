using CSCore.DSP;
using System;
using System.Collections.Generic;

namespace Audio_Visualizer.CSCore
{
    public class BasicSpectrumProvider : FftProvider, ISpectrumProvider
    {
        private readonly int m_SampleRate;
        private readonly List<object> m_Contexts = new List<object>();

        public BasicSpectrumProvider(int channels, int sampleRate, FftSize fftSize) : base (channels, fftSize)
        {
            if(sampleRate <= 0)
            {
                throw new ArgumentOutOfRangeException("sampleRate");
            }

            m_SampleRate = sampleRate;
        }

        public int GetFftBandIndex(float frequency)
        {
            int fftSize = (int)FftSize;
            double f = m_SampleRate / 2.0;
            return (int)((frequency / f) * (fftSize / 2));
        }

        public bool GetFftData(float[] fftResultBuffer, object context)
        {
            if(m_Contexts.Contains(context))
            {
                return false;
            }

            m_Contexts.Add(context);
            GetFftData(fftResultBuffer);
            return true;
        }

        public override void Add(float[] samples, int count)
        {
            base.Add(samples, count);
            if(count > 0)
            {
                m_Contexts.Clear();
            }
        }

        public override void Add(float left, float right)
        {
            base.Add(left, right);
            m_Contexts.Clear();
        }
    }
}
