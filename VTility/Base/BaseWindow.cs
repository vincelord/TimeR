using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using VTility.Logic;

namespace VTility
{
    public class BaseWindow : Window
    {
        private Rect originalSize;
        private string WindowKey;
        private string WindowName;

        // for use in registry
        public BaseWindow()
        {
            WindowName = this.GetType().Name;
            WindowKey = UtilApplication.APP_REGROOT + "\\" + WindowName;

            this.Loaded += WindowBase_Loaded;
            this.Unloaded += WindowBase_Unloaded;
            this.StateChanged += WindowBase_OnStateChanged;
        }

        /* Event Handler */

        public void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount < 2)
                this.DragMove();
        }

        public Point GetCursorPositionPercentage()
        {
            System.Drawing.Point absPos = System.Windows.Forms.Cursor.Position; // absolute cursor point

            var relPos = this.PointFromScreen(new Point(absPos.X, absPos.Y)); // window relative cursor point
            var smartX = (int)(relPos.X / this.Width * 100);

            if (smartX < 0)
                smartX = 0;
            else if (smartX > 100)
                smartX = 100;

            var smartY = (int)(relPos.Y / this.Height * 100);

            if (smartY < 0)
                smartY = 0;
            else if (smartY > 100)
                smartY = 100;

            return new Point(smartX, smartY);
        }

        public void LoadWindowSettings()
        {
            if (RegistryHasSettingsForThisWindow())
            {
                if (Convert.ToBoolean(RegistryLoad("Hidden")) == false)
                    WindowShow();
                else
                    WindowHide();

                // load last window position/size
                if (Convert.ToBoolean(RegistryLoad("Maximized")) == true)
                {
                    WindowState = WindowState.Maximized;
                }
                else
                {
                    // TODO check if on screen
                    double newTop = Convert.ToDouble(RegistryLoad("Top"));
                    if (newTop > MaxHeight)
                        newTop = 0;
                    Top = newTop;

                    double newLeft = Convert.ToDouble(RegistryLoad("Left"));
                    if (newLeft > MaxWidth)
                        newLeft = 0;
                    Left = newLeft;

                    double newHeight = Convert.ToDouble(RegistryLoad("Height"));
                    if (newHeight > MaxHeight)
                        newHeight = MaxHeight;
                    Height = newHeight;

                    double newWidth = Convert.ToDouble(RegistryLoad("Width"));
                    if (newWidth > MaxWidth)
                        newWidth = MaxWidth;

                    Width = newWidth;
                }
            }
            else
            {
                RegistryCreateSubKey(WindowName);
            }
        }

        public void RegistryCreateSubKey(string keyname)
        {
            RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey("Software\\" + UtilApplication.APP_NAME, true);

            if (ProgSettings == null)
            {
                ProgSettings = Registry.CurrentUser.OpenSubKey("Software", true);
                ProgSettings.CreateSubKey(UtilApplication.APP_NAME);
                ProgSettings.Close();
                ProgSettings = Registry.CurrentUser.OpenSubKey("Software\\" + UtilApplication.APP_NAME, true);
            }

            if (!ProgSettings.GetSubKeyNames().Contains(keyname))
                ProgSettings.CreateSubKey(keyname);
            ProgSettings.Close();
        }

        public void RegistryDelete(string key)
        {
            RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(WindowKey, true);
            ProgSettings.DeleteValue(key, false);
            ProgSettings.Close();
        }

        public object RegistryLoad(string key)
        {
            RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(WindowKey, true);
            object setting = ProgSettings.GetValue(key, false);
            ProgSettings.Close();

            return setting;
        }

        public void RegistrySave(string key, object value)
        {
            RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(WindowKey, true);
            if (ProgSettings == null)
            {
                ProgSettings.Close();
                RegistryCreateSubKey(WindowName);
            }

            ProgSettings = Registry.CurrentUser.OpenSubKey(WindowKey, true);
            ProgSettings.SetValue(key, value);
            ProgSettings.Close();
        }

        public void SaveWindowSettings()
        {
            // save window settings
            if (WindowState == WindowState.Maximized)
            {
                // Use the RestoreBounds as the current values will be 0, 0 and the size of the screen
                RegistrySave("Top", RestoreBounds.Top);
                RegistrySave("Left", RestoreBounds.Left);
                RegistrySave("Height", RestoreBounds.Height);
                RegistrySave("Width", RestoreBounds.Width);
                RegistrySave("Maximized", true);
            }
            else
            {
                RegistrySave("Top", this.Top);
                RegistrySave("Left", this.Left);
                RegistrySave("Height", this.Height);
                RegistrySave("Width", this.Width);
                RegistrySave("Maximized", false);
            }
        }

        public void ToTop()
        {
            this.Topmost = true;
        }

        public void WindowHide()
        {
            this.Close();
        }

        public void WindowMaximize()
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        public void WindowReset()
        {
            this.WindowState = WindowState.Normal;
            this.Width = originalSize.Width;
            this.Height = originalSize.Height;
            this.WindowToBottomRight(30);
        }

        public void WindowShow()
        {
            this.Activate();
        }

        public void WindowToBottomRight(int offset)
        {
            //Screen thisScreen = UtilScreen.FindScreenOf(this);
            int boundWidth = Screen.PrimaryScreen.Bounds.Width;
            int boundHeight = Screen.PrimaryScreen.Bounds.Height;
            int xWithout = boundWidth - (int)Width;
            int yWithout = boundHeight - (int)Height;
            this.Left = xWithout - offset;
            this.Top = yWithout - offset;
        }

        public void WindowToCenter()
        {
            //Screen thisScreen = UtilScreen.FindScreenOf(this);
            int boundWidth = Screen.PrimaryScreen.Bounds.Width;
            int boundHeight = Screen.PrimaryScreen.Bounds.Height;
            int x = boundWidth - (int)Width;
            int y = boundHeight - (int)Height;
            this.Left = x / 2;
            this.Top = y / 2;
        }

        public void WindowToTopRight(int offset)
        {
            //Screen thisScreen = UtilScreen.FindScreenOf(this);
            int boundWidth = Screen.PrimaryScreen.Bounds.Width;
            int boundHeight = Screen.PrimaryScreen.Bounds.Height;
            int x = boundWidth - (int)Width;
            int y = boundHeight - (int)Height;
            this.Left = x - offset;
            this.Top = offset;
        }

        protected void WindowBase_OnStateChanged(object sender, EventArgs e)
        {
            var smallest = UtilScreen.FindScreenSmallest();
            Screen smallestScreen = smallest.Item1;
            Dimensions smallestDim = smallest.Item2;

            var newMax = smallestDim.Height() - UtilScreen.GetTaskBarHeightOf(smallestScreen);
            MaxHeight = newMax;
            Console.WriteLine("Setting MaxHeight to " + newMax);
        }

        private bool RegistryHasSettingsForThisWindow()
        {
            RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(WindowKey, false);
            bool hasKey = ProgSettings != null && (ProgSettings.ValueCount > 0 || ProgSettings.SubKeyCount > 0);
            if (hasKey)
            {
                ProgSettings.Close();
            }
            return hasKey;
        }

        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            originalSize = new Rect(RestoreBounds.Top, RestoreBounds.Left, RestoreBounds.Width, RestoreBounds.Height);

            LoadWindowSettings();
        }

        private void WindowBase_Unloaded(object sender, RoutedEventArgs e)
        {
            SaveWindowSettings();
        }

        /*** REGISTRY ***/
        /* UI / Drag'n'Drop ***/
        /* Window States */
        /* Window Positioning */
    }
}