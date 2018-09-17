using Audio_Visualizer.UI;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Audio_Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region WindowCommands
        private void close_window_Click(object sender, RoutedEventArgs e)
        {
            WindowHandling.CloseWindow(this);
        }
        private void maximize_window_Click(object sender, RoutedEventArgs e)
        {
            //set the window state and disable window resizing when the window is maximized
            if(WindowState == WindowState.Maximized)
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
        private void minimize_window_Click(object sender, RoutedEventArgs e)
        {
            WindowHandling.MinimizeWindow(this);
        }
        private void window_drag(object sender, MouseButtonEventArgs e)
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
        private void select_mixer(object sender, RoutedEventArgs e)
        {
            SetIconPosition(Mixer);
        }
        private void select_eq(object sender, RoutedEventArgs e) 
        {
            SetIconPosition(EQ);
        }
        private void select_chroma(object sender, RoutedEventArgs e)
        {
            SetIconPosition(Chroma);
        }
        private void select_comms(object sender, RoutedEventArgs e)
        {
            SetIconPosition(Comms);
        }
        private void select_settings(object sender, RoutedEventArgs e)
        {
            SetIconPosition(Settings);
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
    }
}
