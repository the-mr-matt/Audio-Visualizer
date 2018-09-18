using Audio_Visualizer.UI;
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
        #endregion

        /// <summary>
        /// Creates the mixer channels - levels, slider, channel name, scale
        /// </summary>
        public static void CreateMixerChannels()
        {
            var zeroMargin = new Thickness() { Top = 0, Bottom = 0, Left = 0, Right = 0 };

            //create arrays
            m_ChannelNames = new Label[10];

            //create mixer sliders
            for (int i = 0; i < 10; i++)
            {
                //main grid
                Grid grid = new Grid() { Margin = zeroMargin };

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
                    Margin = zeroMargin
                };

                m_ChannelNames[i] = label;

                //make label a child of grid
                grid.Children.Add(label);

                //slider
                Slider slider = new Slider()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness() { Left = -35, Top = 30, Right = 0, Bottom = 10 },
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

                //levels grid
                //first channel has L and R
                if (i == 0)
                {
                    Grid left = new Grid()
                    {
                        Margin = new Thickness() { Left = 25, Top = 36, Right = 0, Bottom = 16 },
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Width = 15
                    };

                    Grid right = new Grid()
                    {
                        Margin = new Thickness() { Left = 45, Top = 36, Right = 0, Bottom = 16 },
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Width = 15
                    };

                    //make levels children of grid
                    grid.Children.Add(left);
                    grid.Children.Add(right);

                    //levels shape
                    Rectangle lBg = new Rectangle()
                    {
                        Fill = new SolidColorBrush(ColorPalette.Gray),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = zeroMargin
                    };

                    Rectangle lLine = new Rectangle()
                    {
                        Fill = new SolidColorBrush(ColorPalette.Accent),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = zeroMargin,
                        Height = 2
                    };

                    Rectangle rBg = new Rectangle()
                    {
                        Fill = new SolidColorBrush(ColorPalette.Gray),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Margin = zeroMargin
                    };

                    Rectangle rLine = new Rectangle()
                    {
                        Fill = new SolidColorBrush(ColorPalette.Accent),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Top,
                        Margin = zeroMargin,
                        Height = 2
                    };

                    //make shape child of levels grid
                    left.Children.Add(lBg);
                    left.Children.Add(lLine);
                    right.Children.Add(rBg);
                    right.Children.Add(rLine);
                }
                else
                {
                    Grid levels = new Grid()
                    {
                        Margin = new Thickness() { Left = 35, Top = 36, Right = 0, Bottom = 16 },
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        Width = 15
                    };

                    //make levels child of grid
                    grid.Children.Add(levels);

                    Levels(zeroMargin, levels);
                }

                //scale grid
                UniformGrid scaleGrid = new UniformGrid()
                {
                    Columns = 1,
                    Rows = 10,
                    Margin = new Thickness() { Left = 60, Top = 40, Right = 0, Bottom = 20 },
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
                        Margin = zeroMargin,
                        Height = 2
                    };

                    //make ticks child of scale grid
                    scaleGrid.Children.Add(tick);
                }
            }

            //set first channel
            SetChannelName(0, "Master");
        }

        private static void Levels(Thickness zeroMargin, Grid levels)
        {
            //levels shape
            Rectangle bg = new Rectangle()
            {
                Fill = new SolidColorBrush(ColorPalette.Gray),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = zeroMargin
            };

            Rectangle line = new Rectangle()
            {
                Fill = new SolidColorBrush(ColorPalette.Accent),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = zeroMargin,
                Height = 2
            };

            //make shape child of levels grid
            levels.Children.Add(bg);
            levels.Children.Add(line);
        }

        /// <summary>
        /// Replace the default channel name
        /// </summary>
        public static void SetChannelName(int index, string content)
        {
            //TODO: application icon

            m_ChannelNames[index].Content = content;
        }
    }
}
