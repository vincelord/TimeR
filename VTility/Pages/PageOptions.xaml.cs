using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using VTility.Logic;

namespace VTility.Pages
{
    /// <summary>
    /// Interaktionslogik für PageOptions.xaml
    /// </summary>
    public partial class PageOptions : BasePage
    {
        public const string OPTION_SHUTDOWN = "opt_shutdown";
        public const string OPTION_FORCE = "opt_force";
        public const string HOTKEY_KEY = "opt_hotkey";
        public const string HOTKEY_USE = "false";

        public PageOptions()
        {
            InitializeComponent();
        }

        public override void SaveSettings()
        {
            SavePageSetting(OPTION_SHUTDOWN, opt_shutdown.SelectedIndex);
            SavePageSetting(OPTION_FORCE, opt_force.IsChecked);
            SavePageSetting(HOTKEY_KEY, hotkeyBox.Text);
            SavePageSetting(HOTKEY_USE, opt_useHotkey.IsChecked);
        }

        public override void LoadSettings()
        {
            try
            {
                // get shutdown type
                if (LoadPageSetting(OPTION_SHUTDOWN) != null)
                {
                    opt_shutdown.SelectedIndex = Convert.ToInt32(LoadPageSetting(OPTION_SHUTDOWN));
                }

                // get shutdown force
                if (LoadPageSetting(OPTION_FORCE) != null)
                {
                    opt_force.IsChecked = Convert.ToBoolean(LoadPageSetting(OPTION_FORCE));
                }

                // get autostart
                if (UtilApplication.RegistryHasAutostart())
                    opt_autostart.IsChecked = true;
                else
                    opt_autostart.IsChecked = false;

                // get hotkey usage
                if (LoadPageSetting(HOTKEY_USE) != null)
                {
                    opt_useHotkey.IsChecked = Convert.ToBoolean(LoadPageSetting(HOTKEY_USE));
                }
                // get hotkey key
                if (LoadPageSetting(HOTKEY_KEY) != null)
                {
                    hotkeyBox.Text = Convert.ToString(LoadPageSetting(HOTKEY_KEY));
                }
            }
            catch (Exception e1)
            {
                Console.WriteLine("error... " + e1);
            }
        }

        private void HotkeyBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return;
            }

            // Build the shortcut key name.
            StringBuilder shortcutText = new StringBuilder();
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                shortcutText.Append("Ctrl+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0)
            {
                shortcutText.Append("Shift+");
            }
            if ((Keyboard.Modifiers & ModifierKeys.Alt) != 0)
            {
                shortcutText.Append("Alt+");
            }
            shortcutText.Append(key.ToString());

            // Update the text box.
            hotkeyBox.Text = shortcutText.ToString();
        }

        // Settings
        private void opt_autostart_checked(object sender, RoutedEventArgs e)
        {
            UtilApplication.RegistrySetAutostart(true);
        }

        private void opt_autostart_unchecked(object sender, RoutedEventArgs e)
        {
            UtilApplication.RegistrySetAutostart(false);
        }

        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            UtilApplication.RemoveFromRegistry();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        private void ButtonRegistry_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("regedit.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //Process.Start("regjump.exe", );
        }
    }
}