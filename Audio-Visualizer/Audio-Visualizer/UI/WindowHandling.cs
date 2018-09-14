using System.Windows;

namespace Audio_Visualizer.UI
{
    class WindowHandling
    {
        /// <summary>
        /// Closes the application on click, mimics the X button on a normal window
        /// </summary>
        public static void CloseWindow(Window window)
        {
            SystemCommands.CloseWindow(window);
        }

        /// <summary>
        /// Maximizes the window, mimics the center button at the top of a normal window
        /// </summary>
        public static void MaximizeWindow(Window window)
        {
            SystemCommands.MaximizeWindow(window);
        }

        /// <summary>
        /// Minimizes the window, mimics the left button on a normal window
        /// </summary>
        public static void MinimizeWindow(Window window)
        {
            SystemCommands.MinimizeWindow(window);
        }

        /// <summary>
        /// Restores the window
        /// </summary>
        public static void RestoreWindow(Window window)
        {
            SystemCommands.RestoreWindow(window);
        }
    }
}
