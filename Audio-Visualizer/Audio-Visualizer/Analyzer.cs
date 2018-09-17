using Audio_Visualizer.CSCore;
using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Audio_Visualizer
{
    public class Analyzer
    {
        #region ----PROPERTIES----
        public static int NumberOfAnalysisBars { get { return m_NumberOfAnalysisBars; } }
        #endregion

        #region ----CONFIG----
        private const int m_NumberOfAnalysisBars = 14;
        private const double m_Multiplier = 4;
        private static Grid[] m_Bars;

        private static WasapiCapture m_SoundIn;
        private static ISoundOut m_SoundOut;
        private static IWaveSource m_Source;
        private static SpectrumAnalyzer m_SpectrumAnalyzer;
        private static MMDevice m_Device;
        #endregion

        /// <summary>
        /// Init analyzer bars
        /// </summary>
        public static void CreateAnalyserBars()
        {
            //remove existing definitions
            MainWindow.Instance.AnalyzerBars.Columns = m_NumberOfAnalysisBars;

            m_Bars = new Grid[m_NumberOfAnalysisBars];

            for (int i = 0; i < m_NumberOfAnalysisBars; i++)
            {
                //create a grid - parent object of the whole bar
                Grid bar = new Grid();

                //make the bar a child of the analyser bars parent
                MainWindow.Instance.AnalyzerBars.Children.Add(bar);

                //set the column of the bar
                bar.SetValue(Grid.ColumnProperty, i);

                //grid size and margins of bar
                bar.Width = 25;
                bar.HorizontalAlignment = HorizontalAlignment.Center;
                bar.VerticalAlignment = VerticalAlignment.Stretch;
                var gridMargin = bar.Margin;
                gridMargin.Top = gridMargin.Bottom = gridMargin.Left = gridMargin.Right = 0;
                bar.Margin = gridMargin;

                //background gradient rectangle
                Rectangle background = new Rectangle();

                //make background a child of the bar
                bar.Children.Add(background);
                
                //set margin
                var bgMargin = background.Margin;
                bgMargin.Top = bgMargin.Bottom = bgMargin.Left = bgMargin.Right = 0;
                background.Margin = bgMargin;

                //background color
                Color color = (Color)ColorConverter.ConvertFromString("#FF363636");
                background.Fill = new SolidColorBrush(color);

                //dropshadow
                DropShadowEffect dropShadow = new DropShadowEffect();
                dropShadow.BlurRadius = 20;
                dropShadow.ShadowDepth = 0;
                dropShadow.Opacity = 0.3;
                background.Effect = dropShadow;

                //line
                Rectangle line = new Rectangle();

                //make line a child of the bar
                bar.Children.Add(line);

                //size and margins of line
                line.VerticalAlignment = VerticalAlignment.Top;
                line.HorizontalAlignment = HorizontalAlignment.Stretch;
                var lineMargin = line.Margin;
                lineMargin.Left = lineMargin.Right = lineMargin.Top = lineMargin.Bottom = 0;
                line.Margin = lineMargin;
                line.Height = 2;

                //corner radius
                line.RadiusX = line.RadiusY = 1.0;

                //line color
                Color lineColor = (Color)ColorConverter.ConvertFromString("#FF00A2FF");
                line.Fill = new SolidColorBrush(lineColor);

                //add to array
                m_Bars[i] = bar;
            }
        }

        /// <summary>
        /// Convert the normalized bar values into height values for each bar
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void SetAnalzerBar(int index, double value)
        {
            //put value into height range
            double maxHeight = MainWindow.Instance.AnalyzerBars.ActualHeight;
            value *= maxHeight;

            //clamp the value
            if (value > maxHeight) { value = maxHeight; }

            //set margin
            var margin = m_Bars[index].Margin;
            margin.Top = maxHeight - value;

            //reassign margin back to bar
            m_Bars[index].Margin = margin;
        }

        /// <summary>
        /// Begin the audio input
        /// </summary>
        public static void InitAudioSource()
        {
            Stop();

            //open default audio device
            m_SoundIn = new WasapiLoopbackCapture();

            m_Device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Console);

            m_SoundIn.Device = m_Device;
            m_SoundIn.Initialize();

            var soundInSource = new SoundInSource(m_SoundIn);
            ISampleSource source = soundInSource.ToSampleSource();

            SetupSampleSource(source);

            byte[] buffer = new byte[m_Source.WaveFormat.BytesPerSecond / 2];
            soundInSource.DataAvailable += (s, aEvent) =>
            {
                int read;
                while ((read = m_Source.Read(buffer, 0, buffer.Length)) > 0) ;
            };

            m_SoundIn.Start();

            MainWindow.StartTimer();
        }

        /// <summary>
        /// Setup the spectrum analyzer
        /// </summary>
        public static void SetupSampleSource(ISampleSource aSampleSource)
        {
            const FftSize fftSize = FftSize.Fft2048;
            var spectrumProvider = new BasicSpectrumProvider(aSampleSource.WaveFormat.Channels, aSampleSource.WaveFormat.SampleRate, fftSize);

            m_SpectrumAnalyzer = new SpectrumAnalyzer(fftSize)
            {
                SpectrumProvider = spectrumProvider,
                UseAverage = true,
                BarCount = NumberOfAnalysisBars,
                UseLogScale = true,
                ScalingStrategy = ScalingStrategy.Sqrt
            };

            var notificationSource = new SingleBlockNotificationStream(aSampleSource);
            notificationSource.SingleBlockRead += (s, a) => spectrumProvider.Add(a.Left, a.Right);

            m_Source = notificationSource.ToWaveSource(16);
        }

        /// <summary>
        /// Retrieve all normalized bar values from the FFT
        /// </summary>
        public static void GetBarValues()
        {
            //set the height and gradient of the bars - spectrum analyzer
            double[] data = m_SpectrumAnalyzer.GetPointData(m_Multiplier);

            for (int i = 0; i < NumberOfAnalysisBars; i++)
            {
                SetAnalzerBar(i, data[i]);
            }
        }

        /// <summary>
        /// Stop all
        /// </summary>
        private static void Stop()
        {
            MainWindow.StopTimer();

            if (m_SoundIn != null)
            {
                m_SoundIn.Stop();
                m_SoundIn.Dispose();
                m_SoundIn = null;
            }

            if (m_SoundOut != null)
            {
                m_SoundOut.Dispose();
                m_SoundOut.Stop();
                m_SoundOut = null;
            }

            if (m_Source != null)
            {
                m_Source.Dispose();
                m_Source = null;
            }
        }
    }
}
