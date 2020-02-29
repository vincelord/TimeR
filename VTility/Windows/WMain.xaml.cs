using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using VTility.Logic;

namespace VTility.Windows
{
    /// <summary>
    /// Interaktionslogik für WMain.xaml
    /// </summary>
    public partial class WMain : BaseWindow
    {
        public static WMain Instance;

        public WMain()
        {
            InitializeComponent();

            if (WMain.Instance == null)
                WMain.Instance = this;
        }

        /* Basic Buttons */

        private void button_close_click(object sender, RoutedEventArgs e)
        {
            //RegistrySave("countdown", TextBoxCountdown.Text);

            SaveWindowSettings();
            Application.Current.Shutdown();
        }

        private void button_options_click(object sender, RoutedEventArgs e)
        {
            frameMain.Navigate(new Uri("./Pages/PageOptions.xaml", UriKind.Relative));
        }

        private void ButtonHide_Click(object sender, RoutedEventArgs e)
        {
            WindowHide();
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowMaximize();
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            WindowReset();
        }

        /* Tabs */

        private void ButtonTabTimer_Click(object sender, RoutedEventArgs e)
        {
            frameMain.Navigate(new Uri("./Pages/PageTimer.xaml", UriKind.Relative));
        }

        private void ButtonTabDraw_Click(object sender, RoutedEventArgs e)
        {
            frameMain.Navigate(new Uri("./Pages/PageExperiment.xaml", UriKind.Relative));
        }

        private void ButtonTabNotes_Click(object sender, RoutedEventArgs e)
        {
            frameMain.Navigate(new Uri("./Pages/PageNotes.xaml", UriKind.Relative));
        }

        /* Windows */

        private void ButtonUnity_Click(object sender, RoutedEventArgs e)
        {
            WindowUnity wu = new WindowUnity();
            wu.Show();
        }

        /* Aspect Ratio */

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            WindowAspectRatio.Register((Window)sender, new Rect(0, TitleBar.ActualHeight, 0, 0));
        }

        /* Title bar */

        private void TitleBarDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                WindowMaximize();
        }

        /* Drag */

        private void window_drag(object sender, MouseButtonEventArgs e)
        {
            DragWindow(sender, e);
        }

        /* Hotkey stuff */

        private void OnHotKeyPressed()
        {
            if (!IsVisible)
                WindowShow();
            else
                WindowHide();
        }

        [DllImport("User32.dll")]
        private static extern bool RegisterHotKey(
            [In] IntPtr hWnd,
            [In] int id,
            [In] uint fsModifiers,
            [In] uint vk);

        [DllImport("User32.dll")]
        private static extern bool UnregisterHotKey(
            [In] IntPtr hWnd,
            [In] int id);

        private HwndSource _source;
        private const int HOTKEY_ID = 9000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var helper = new WindowInteropHelper(this);
            _source = HwndSource.FromHwnd(helper.Handle);
            _source.AddHook(HwndHook);
            RegisterHotKey();
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            _source = null;
            UnregisterHotKey();
            base.OnClosed(e);
        }

        private void RegisterHotKey()
        {
            Console.WriteLine("Trying to register hotkey.");
            var helper = new WindowInteropHelper(this);
            const uint VK_F10 = 0x79;
            const uint MOD_CTRL = 0x0002;
            if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_CTRL, VK_F10))
            {
                // handle error
                var mb = MessageBox.Show("Could not register hotkey.");
            }
        }

        private void UnregisterHotKey()
        {
            var helper = new WindowInteropHelper(this);
            UnregisterHotKey(helper.Handle, HOTKEY_ID);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            OnHotKeyPressed();
                            handled = true;
                            break;
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}