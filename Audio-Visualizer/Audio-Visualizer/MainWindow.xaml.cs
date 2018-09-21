using Audio_Visualizer.UI;
using Audio_Visualizer.Systems;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CSCore.CoreAudioAPI;

namespace Audio_Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;

        #region ----CONSTRUCTOR----
        public MainWindow()
        {
            //singleton
            Instance = this;

            InitializeComponent();

            //get device
            MMDevice device = MMDeviceEnumerator.DefaultAudioEndpoint(DataFlow.Render, Role.Console);
            
            //init analyzer
            Analyzer.CreateAnalyserBars();
            Analyzer.InitAudioSource(device);

            //init mixer
            Mixer.CreateMixerChannels(device);
        }
        #endregion

        #region ----CONFIG----
        private static TimeSpan m_TimerInterval = new TimeSpan(0, 0, 0, 0, 20);
        private const double m_SpectrogramMultiplier = 6.0;
        private const double m_WaveformMultiplier = 0.5;
        #endregion

        #region ----STATE----
        private static bool m_HasInit;
        private static DispatcherTimer m_Timer = new DispatcherTimer();
        private static Spectrogram m_Spectrogram;
        private static Waveform m_Waveform;
        #endregion

        #region WindowCommands
        private void CloseWindowClick(object sender, RoutedEventArgs e)
        {
            WindowHandling.CloseWindow(this);
        }
        private void MaximizeWindowClick(object sender, RoutedEventArgs e)
        {
            //set the window state and disable window resizing when the window is maximized
            if (WindowState == WindowState.Maximized)
            {
                WindowHandling.RestoreWindow(this);
                //WindowHandling.EnableResize(WindowResize);
            }
            else
            {
                WindowHandling.MaximizeWindow(this);
                //WindowHandling.DisableResize(WindowResize);
            }
        }
        private void MinimizeWindowClick(object sender, RoutedEventArgs e)
        {
            WindowHandling.MinimizeWindow(this);
        }
        private void WindowDrag(object sender, MouseButtonEventArgs e)
        {
            WindowHandling.DragWindow(this);
        }
        #endregion

        #region WindowResize
        private void ResizeInit(object sender, MouseButtonEventArgs e)
        {
            WindowHandling.StartResize(sender);
        }
        private void ResizeEnd(object sender, MouseButtonEventArgs e)
        {
            WindowHandling.EndResize(sender);
        }
        private void ResizingWindow(object sender, MouseEventArgs e)
        {
            WindowHandling.ResizeWindow(sender, e);
        }
        #endregion

        #region MenuNavigation
        private void SelectMixer(object sender, RoutedEventArgs e)
        {
            SetIconPosition(MixerButton);
        }
        private void SelectEQ(object sender, RoutedEventArgs e)
        {
            SetIconPosition(EQButton);
        }
        private void SelectChroma(object sender, RoutedEventArgs e)
        {
            SetIconPosition(ChromaButton);
        }
        private void SelectComms(object sender, RoutedEventArgs e)
        {
            SetIconPosition(CommsButton);
        }
        private void SelectSettings(object sender, RoutedEventArgs e)
        {
            SetIconPosition(SettingsButton);
        }

        /// <summary>
        /// Positions the selected panel icon next to the button that was pressed
        /// </summary>
        private void SetIconPosition(Button button)
        {
            //get the margin of the button and half it to make the margin relative to the center of the button
            //subtract half the height of the icon to make the margin relative to the center of the icon

            var margin = SelectedPanelIcon.Margin;
            margin.Top = button.Margin.Top + button.Height / 2f - SelectedPanelIcon.Height / 2f;
            SelectedPanelIcon.Margin = margin;
        }
        #endregion

        /// <summary>
        /// Start the dispatch timer
        /// </summary>
        public static void StartTimer()
        {
            m_Timer.Tick += TimerTick;
            m_Timer.Interval = m_TimerInterval;
            m_Timer.Start();
        }

        /// <summary>
        /// Stop the dispatch timer
        /// </summary>
        public static void StopTimer()
        {
            m_Timer.Stop();
            m_Timer.Tick -= TimerTick;
        }

        //main logic
        private static void TimerTick(object sender, EventArgs e)
        {
            //initialize after elements have loaded
            if (!m_HasInit)
            {
                //init spectrogram
                int spectrogramWidth = (int)Instance.SpectrogramPanel.ActualWidth * 2;
                int spectrogramHeight = (int)Instance.SpectrogramGrid.ActualHeight * 2;
                m_Spectrogram = new Spectrogram(spectrogramWidth, spectrogramHeight, m_SpectrogramMultiplier);

                int waveformWidth = (int)Instance.WaveformPanel.ActualWidth * 2;
                int waveformHeight = (int)Instance.WaveformPanel.ActualHeight * 2;
                m_Waveform = new Waveform(waveformWidth, waveformHeight, m_WaveformMultiplier);
                
                Mixer.SetUnusedChannels();

                m_HasInit = true;
            }

            Analyzer.ProcessBarValues();
            Mixer.ProcessLevels();
            
            Instance.SpectrogramBitmap.Fill = m_Spectrogram.CreateBitmap();
            Instance.WaveformBitmap.Fill = m_Waveform.CreateBitmap();
        }
    }
}
