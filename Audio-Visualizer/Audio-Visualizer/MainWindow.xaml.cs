using Audio_Visualizer.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void close_window_Click(object sender, RoutedEventArgs e)
        {
            WindowHandling.CloseWindow(this);
        }

        private void maximize_window_Click(object sender, RoutedEventArgs e)
        {
            if(this.WindowState == WindowState.Maximized)
            {
                WindowHandling.RestoreWindow(this);
            }
            else
            {
                WindowHandling.MaximizeWindow(this);
            }
        }

        private void minimize_window_Click(object sender, RoutedEventArgs e)
        {
            WindowHandling.MinimizeWindow(this);
        }
    }
}
