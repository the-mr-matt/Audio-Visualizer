using Audio_Visualizer.Other;
using Audio_Visualizer.UI;
using CSCore.CoreAudioAPI;
using System;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Audio_Visualizer.Systems
{
    public class Mixer
    {
        #region ----PROPERTIES----
        public static float MasterPeak
        {
            get { return m_MasterPeak; }
            private set { m_MasterPeak = value; }
        }
        #endregion

        #region ----STATE----
        private static Label[] m_ChannelNames;
        private static Grid[] m_Levels;
        private static Grid[] m_LevelParents;
        private static Slider[] m_Sliders;
        private static System.Windows.Shapes.Rectangle[] m_Icons;
        private static Thickness m_InitialMargin;

        private static AudioMeterInformation[] m_PeakMeters;
        private static AudioEndpointVolume m_MasterVolume;
        private static SimpleAudioVolume[] m_ChannelVolume;

        private static float m_MasterPeak;
        private static int m_NumberOfOccupiedChannels;
        private static bool m_HasStarted;
        #endregion

        #region ----CONFIG----
        private const double m_Multiplier = 2.0;
        private static string[] m_ForbiddenProcesses = new string[]
        {
            "System",
            "Idle",
            "Bootcamp",
            "Audio-Visualizer"
        };
        #endregion

        /// <summary>
        /// Creates the mixer channels - levels, slider, channel name, scale
        /// </summary>
        public static void CreateMixerChannels(MMDevice device)
        {
            //create arrays
            m_ChannelNames = new Label[10];
            m_Levels = new Grid[10];
            m_LevelParents = new Grid[10];
            m_Sliders = new Slider[10];
            m_Icons = new System.Windows.Shapes.Rectangle[9];
            m_PeakMeters = new AudioMeterInformation[10];

            //create mixer sliders
            for (int i = 0; i < 10; i++)
            {
                //main grid
                Grid grid = new Grid() { Margin = Utils.ZeroMargin };

                //make grid child of mixer grid
                MainWindow.Instance.MixerGrid.Children.Add(grid);

                //skip the master channel
                if (i != 0)
                {
                    m_Icons[i-1] = new System.Windows.Shapes.Rectangle()
                    {
                        Width = 30,
                        Height = 30,
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = Utils.ZeroMargin
                    };

                    grid.Children.Add(m_Icons[i-1]);
                }

                //label
                Label label = new Label()
                {
                    Content = "",
                    Foreground = new SolidColorBrush(ColorPalette.Accent),
                    FontFamily = new System.Windows.Media.FontFamily("Bahnschrift Bold"),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = Utils.ZeroMargin
                };

                //add to array
                m_ChannelNames[i] = label;

                //make label a child of grid
                grid.Children.Add(label);

                //add to array
                m_LevelParents[i] = grid;

                //slider
                Slider slider = new Slider()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    //modify spacing on master channel
                    Margin = new Thickness() { Left = -32, Top = 36, Right = 0, Bottom = 16 },
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = null,
                    Width = 22,
                    Orientation = Orientation.Vertical,
                    Style = (Style)Application.Current.Resources["MixerSlider"],
                    Maximum = 100,
                    Value = 70
                };

                //add to array
                m_Sliders[i] = slider;

                //setup channel volume
                if (i == 0)
                {
                    //master channel
                    m_MasterVolume = AudioEndpointVolume.FromDevice(device);
                    m_Sliders[0].Value = m_MasterVolume.GetMasterVolumeLevelScalar() * 100f;

                    //master peak meter
                    m_PeakMeters[0] = AudioMeterInformation.FromDevice(device);
                }

                //value changed event
                int index = i;
                slider.ValueChanged += (sender, e) => Slider_ValueChanged(sender, e, i == 0, index);

                //make slider child of grid
                grid.Children.Add(slider);

                //levels
                //make a parent object
                m_InitialMargin = new Thickness() { Left = 18, Top = 36, Right = 0, Bottom = 16 };

                Grid level = new Grid()
                {
                    Margin = m_InitialMargin,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = 15
                };

                //make levels child of grid
                grid.Children.Add(level);

                //add to array
                m_Levels[i] = level;

                //levels shape
                System.Windows.Shapes.Rectangle bg = new System.Windows.Shapes.Rectangle()
                {
                    Fill = new SolidColorBrush(ColorPalette.Gray),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = Utils.ZeroMargin
                };

                System.Windows.Shapes.Rectangle line = new System.Windows.Shapes.Rectangle()
                {
                    Fill = new SolidColorBrush(ColorPalette.Accent),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = Utils.ZeroMargin,
                    Height = 2
                };

                //make shape child of levels grid
                level.Children.Add(bg);
                level.Children.Add(line);

                //scale grid
                UniformGrid scaleGrid = new UniformGrid()
                {
                    Columns = 1,
                    Rows = 10,
                    //modify spacing on the master channel
                    Margin = new Thickness() { Left = 45, Top = 36, Right = 0, Bottom = 20 },
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Width = 5
                };

                //make scaleGrid child of grid
                grid.Children.Add(scaleGrid);

                //scale ticks
                for (int x = 0; x < 10; x++)
                {
                    System.Windows.Shapes.Rectangle tick = new System.Windows.Shapes.Rectangle()
                    {
                        Fill = new SolidColorBrush(ColorPalette.Gray),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = Utils.ZeroMargin,
                        Height = 2
                    };

                    //make ticks child of scale grid
                    scaleGrid.Children.Add(tick);
                }
            }


            //init channel volumes
            GetChannelVolumes();

            //set first channel
            SetChannelName(0, "Master");

            m_HasStarted = true;
        }

        /// <summary>
        /// Add the programs outputting audio to the simple volume array
        /// </summary>
        private static void GetChannelVolumes()
        {
            //create array of channels
            m_ChannelVolume = new SimpleAudioVolume[9];

            //get processes outputting audio
            AudioSessionManager2 sessionManager = null;
            var thread = new Thread
            (
                () => { sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render); }
            );

            thread.Start();
            thread.Join();

            AudioSessionEnumerator sessionEnumerator = sessionManager.GetSessionEnumerator();

            //loop through processes - clamp to number of channels
            //skip the master channel
            m_NumberOfOccupiedChannels = 0;
            for (int i = 1; i < Math.Min(sessionEnumerator.Count, 10); i++)
            {
                //get the process and name
                var process = sessionEnumerator[i].QueryInterface<AudioSessionControl2>().Process;
                string name = process == null ? "Unnamed" : process.ProcessName;

                //check if this process is valid for audio monitoring
                bool flag = false;
                for (int x = 0; x < m_ForbiddenProcesses.Length; x++)
                {
                    if (name == m_ForbiddenProcesses[x])
                    {
                        flag = true;
                    }
                }

                if (flag) { continue; }

                //get process icon
                Icon icon = Icon.ExtractAssociatedIcon(process.MainModule.FileName);

                //set icon
                m_Icons[m_NumberOfOccupiedChannels].Fill = new ImageBrush(icon.ToImageSource());

                //get the simple volume
                SimpleAudioVolume simpleVolume = sessionEnumerator[i].QueryInterface<SimpleAudioVolume>();
                m_ChannelVolume[m_NumberOfOccupiedChannels] = simpleVolume;

                //get the peak meter
                m_PeakMeters[m_NumberOfOccupiedChannels] = sessionEnumerator[i].QueryInterface<AudioMeterInformation>();

                m_NumberOfOccupiedChannels++;
            }
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }

        /// <summary>
        /// Sets the value of unused channels to zero 
        /// </summary>
        public static void SetUnusedChannels()
        {
            for (int i = m_NumberOfOccupiedChannels + 1; i < 10; i++)
            {
                SetLevel(i, 0.0);
            }
        }

        /// <summary>
        /// Called when a slider has changed value, sets the volume on the channel with the given index
        /// </summary>
        private static void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e, bool isMaster, int index)
        {
            SetVolume(isMaster, index);
        }

        /// <summary>
        /// Replace the default channel name
        /// </summary>
        public static void SetChannelName(int index, string content)
        {
            //TODO: application icon

            m_ChannelNames[index].Content = content;
        }

        /// <summary>
        /// Set the level for the given channel
        /// </summary>
        private static void SetLevel(int index, double normalizedValue)
        {
            //get the current height of the overall grid
            double maxGridHeight = m_LevelParents[index].ActualHeight;

            //get the current margin
            var margin = m_Levels[index].Margin;

            //calculate the maximum possible height of the level at the current window size
            double maxLevelHeight = (maxGridHeight - m_InitialMargin.Bottom);

            //calculate the new margin
            double value = Utils.Lerp(m_InitialMargin.Top, maxLevelHeight, (1.0 - normalizedValue));

            //assign margin
            margin.Top = value;
            m_Levels[index].Margin = margin;
        }
        
        /// <summary>
        /// Get the volume output from the channels
        /// </summary>
        public static void ProcessLevels()
        {
            for (int i = 0; i < 10; i++)
            {
                //set and clamp levels
                float level = (float)m_PeakMeters[0].GetPeakValue() * (float)m_Multiplier;
                SetLevel(i, Math.Min(level, 1.0));
            }
        }

        /// <summary>
        /// Set the channel volume at the given index
        /// </summary>
        public static void SetVolume(bool isMaster, int index)
        {
            if (m_HasStarted)
            {
                float value = (float)m_Sliders[index].Value / 100f;

                if (isMaster)
                {
                    m_MasterVolume.SetMasterVolumeLevelScalarNative(value, Guid.Empty);
                }
                else
                {
                    m_ChannelVolume[index-1].SetMasterVolumeNative(value, Guid.Empty);
                }
            }
        }
    }
}