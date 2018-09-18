using Audio_Visualizer.Other;
using Audio_Visualizer.UI;
using CSCore.CoreAudioAPI;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Audio_Visualizer.Systems
{
    public class Mixer
    {
        #region ----STATE----
        private static Label[] m_ChannelNames;
        private static Grid[] m_Levels;
        private static Grid[] m_LevelParents;
        private static Thickness m_InitialMargin;

        private static AudioMeterInformation m_PeakMeter;
        #endregion

        #region ----CONFIG----
        private const double m_Multiplier = 2.0;
        #endregion

        /// <summary>
        /// Creates the mixer channels - levels, slider, channel name, scale
        /// </summary>
        public static void CreateMixerChannels()
        {
            //create arrays
            m_ChannelNames = new Label[10];
            m_Levels = new Grid[10];
            m_LevelParents = new Grid[10];

            //create mixer sliders
            for (int i = 0; i < 10; i++)
            {
                //main grid
                Grid grid = new Grid() { Margin = Utils.ZeroMargin };

                //make grid child of mixer grid
                MainWindow.Instance.MixerGrid.Children.Add(grid);

                //label
                Label label = new Label()
                {
                    Content = "",
                    Foreground = new SolidColorBrush(ColorPalette.Accent),
                    FontFamily = new FontFamily("Bahnschrift Bold"),
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
                Rectangle bg = new Rectangle()
                {
                    Fill = new SolidColorBrush(ColorPalette.Gray),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Margin = Utils.ZeroMargin
                };

                Rectangle line = new Rectangle()
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
                    Rectangle tick = new Rectangle()
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

            //set first channel
            SetChannelName(0, "Master");
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

        public static void InitPeakMeter()
        {
            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();
            m_PeakMeter = AudioMeterInformation.FromDevice(enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console));
        }

        public static void ProcessLevels()
        {
            //master levels
            double value = m_PeakMeter.GetPeakValue() * m_Multiplier;
            
            SetLevel(0, value);
        }
    }
}
