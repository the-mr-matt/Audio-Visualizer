using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace Audio_Visualizer.UI
{
    class WindowHandling
    {
        #region Variables
        private static bool currentlyResizing;
        const int increment = 5;
        #endregion

        #region WindowCommands
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

        /// <summary>
        /// Allows the window to be dragged from specific elements - the main window is hidden so this needs to be manually handled
        /// </summary>
        public static void DragWindow(Window window)
        {
            window.DragMove();
        }
        #endregion

        #region WindowResize
        /// <summary>
        /// Initialize the window resizing
        /// </summary>
        public static void StartResize(object sender)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                currentlyResizing = true;
                senderRect.CaptureMouse();
            }
        }

        /// <summary>
        /// End the window resizing
        /// </summary>
        public static void EndResize(object sender)
        {
            Rectangle senderRect = sender as Rectangle;
            if (senderRect != null)
            {
                currentlyResizing = false;
                senderRect.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// Resizes the window based on the rectangle that was dragged
        /// </summary>
        public static void ResizeWindow(object sender, MouseEventArgs e)
        {
            if (currentlyResizing)
            {
                Rectangle senderRect = sender as Rectangle;
                Window mainWindow = senderRect.Tag as Window;
                if (senderRect != null)
                {
                    double width = e.GetPosition(mainWindow).X;
                    double height = e.GetPosition(mainWindow).Y;

                    senderRect.CaptureMouse();

                    if (senderRect.Name.ToLower().Contains("right"))
                    {
                        width += increment;
                        if (width > 0) { mainWindow.Width = width; }
                    }

                    if (senderRect.Name.ToLower().Contains("left"))
                    {
                        width -= increment;
                        mainWindow.Left += width;
                        width = mainWindow.Width - width;
                        
                        if (width > 0) { mainWindow.Width = width; }
                    }

                    if (senderRect.Name.ToLower().Contains("bottom"))
                    {
                        height += increment;
                        if (height > 0) { mainWindow.Height = height; }
                    }

                    if (senderRect.Name.ToLower().Contains("top"))
                    {
                        height -= increment;
                        mainWindow.Top += height;
                        height = mainWindow.Height - height;
                        if (height > 0) { mainWindow.Height = height; }
                    }
                }
            }
        }

        /// <summary>
        /// Shows the resizing rectangles
        /// </summary>
        public static void EnableResize(Grid grid)
        {
            grid.IsEnabled = true;
        }

        /// <summary>
        /// Hides the resizing rectangles
        /// </summary>
        public static void DisableResize(Grid grid)
        {

            grid.IsEnabled = false;
        }
        #endregion
    }
}
