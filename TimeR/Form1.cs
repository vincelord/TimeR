using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TimeR
{
    /* 
        TODO:
        - topmost, bottommost as options
        - start with windows (what if deleted?)
        - snap to taskbar
        - snap to multiple monitors
        - fix bug regarding '-' in countdown input
        - checkbox option for window transparency
        - keep settings global (not only in folder - why anyway?)
        DONE:
        - allow selection of text even while running
        - checkbox option to start program with windows (autostart)
        - radio choose between 3 types of shutdown
        - convert any input to a valid datetime
        - options page
        - adapt UI from gadget images, etc.
        - buttons for start/pause, stop, close
    */

    public partial class form_main : Form
    {
        private const String APP_NAME = "TimeR";
        private const String STOP_VALUE = "00:00:00";

        private const int WM_ACTIVATEAPP = 0x1C;
        private const int WM_GETTEXT = 0xd;
        private const int WM_GETTEXTLENGTH = 0xe;
        private const int WM_ERASEBKGND = 0x14;
        private const int WM_WINDOWPOSCHANGING = 0x0046;


        private Timer timer;
        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;
        private Point MouseDownLocation;
        private Icon appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        private string LAST_VALIDATED_INPUT = STOP_VALUE;

        // The path to the key where Windows looks for startup applications
        private RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        /* windows api functions */
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_NOACTIVATE = 0x0010;
        private const UInt32 SWP_NOZORDER = 4;

        [DllImport("user32.dll", SetLastError = false)]
        private static extern IntPtr GetDesktopWindow();



        public form_main()
        {
            InitializeComponent();
            CreateSysTrayIcon();

            /* display clock */
            StartClock();

            /* load last settings */
            LoadSettings();

            /* additional event handlers */
            RegisterEventHandlers();
        }

        private void RegisterEventHandlers()
        {
            /* button hover gfx handler */
            button_start.MouseEnter += new EventHandler(button_start_MouseEnter);
            button_start.MouseLeave += new EventHandler(button_start_MouseLeave);
            button_stop.MouseEnter += new EventHandler(button_stop_MouseEnter);
            button_stop.MouseLeave += new EventHandler(button_stop_MouseLeave);

            /* drag window handler */
            LinkedList<Control> dragAndDropControls = new LinkedList<Control>();
            dragAndDropControls.AddLast(tabs);
            dragAndDropControls.AddLast(timerPage);
            dragAndDropControls.AddLast(optionsPage);
            dragAndDropControls.AddLast(shutdownOptions);
            foreach (var test in dragAndDropControls)
            {
                test.MouseDown += new MouseEventHandler(tabs_MouseDown);
                test.MouseMove += new MouseEventHandler(tabs_MouseMove);
            }

            /*  */
            Deactivate += new EventHandler(form_deactivate);
        }




        /* override hide window */
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Listen for operating system messages.
            switch (m.Msg)
            {
                // The WM_ACTIVATEAPP message occurs when the application
                // becomes the active application or becomes inactive.
                case WM_WINDOWPOSCHANGING:
                    //SetWindowPos(Handle, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_NOZORDER);
                    Activate();
                    break;
            }

            // pass wndproc
            base.WndProc(ref m);
        }

        /* In-Application Hotkeys */
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                MessageBox.Show("Can't touch this!");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CreateSysTrayIcon()
        {
            // Create a tray menu
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Exit", OnTrayExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "TimeR";
            trayIcon.Icon = new Icon(appIcon, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }
        
        private void ExitAndSave()
        {
            SaveSettings();
            Application.Exit();
        }

        private void SaveSettings()
        {
            // explanations in LoadSettings
            Properties.Settings.Default.opt_energy = opt_energy.Checked;
            Properties.Settings.Default.opt_reboot = opt_reboot.Checked;
            Properties.Settings.Default.opt_shutdown = opt_shutdown.Checked;
            Properties.Settings.Default.opt_force = opt_force.Checked;
            Properties.Settings.Default.pos_x = base.Left;
            Properties.Settings.Default.pos_y = base.Top;
            Properties.Settings.Default.time_left = countdownClock.Text;

            if(opt_autostart.Checked)
            {
                // Add the value in the registry so that the application runs at startup
                rkApp.SetValue(APP_NAME, Application.ExecutablePath);
            }
            else
            {
                // Remove the value from the registry so that the application doesn't start
                rkApp.DeleteValue(APP_NAME, false);
            }

            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            // load shutdown options
            opt_energy.Checked = Properties.Settings.Default.opt_energy;
            opt_reboot.Checked = Properties.Settings.Default.opt_reboot;
            opt_shutdown.Checked = Properties.Settings.Default.opt_shutdown;
            opt_force.Checked = Properties.Settings.Default.opt_force;

            // load last window position
            StartPosition = FormStartPosition.Manual;
            Left = Properties.Settings.Default.pos_x;
            Top = Properties.Settings.Default.pos_y;

            // load counter time as string
            countdownClock.Text = Properties.Settings.Default.time_left;

            // load autostart settings
            opt_autostart.Checked = IsInAutostartRegistry();
        }

        private bool IsInAutostartRegistry()
        {
            // Check to see the current state (running at startup or not)
            if (rkApp.GetValue(APP_NAME) == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                return false;
            }
            else
            {
                if(!rkApp.GetValue(APP_NAME).Equals(Application.ExecutablePath))
                {
                    rkApp.DeleteValue(APP_NAME, false);
                    return false;
                }

                // The value exists, the application is set to run at startup
                return true;
            }
        }

        private void TogglePlayPause()
        {
            if (timer == null)
            {
                // dont start on zero seconds
                if (countdownClock.Text == STOP_VALUE)
                    return;

                /* (re)start */
                timer = StartCountdown();
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_pause_hover));
            }
            else if (timer.Enabled)
            {
                /* pause */
                timer.Enabled = false;
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_play_hover));
            }
            else if (!timer.Enabled)
            {
                /* continue */
                timer.Enabled = true;
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_pause_hover));
            }
        }



        /* timer TODO */
        private Timer StartClock()
        {
            Timer t = new Timer();
            t.Interval = 1000;
            t.Tick += new EventHandler(clocktimeHandler);
            t.Enabled = true;
            return t;
        }

        private Timer StartCountdown()
        {
            Timer t = new Timer();
            t.Interval = 1000;
            t.Tick += new EventHandler(countdownHandler);
            t.Enabled = true;
            return t;
        }

        private void ResetCountdown()
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_play));
            }
            countdownClock.Text = STOP_VALUE;
        }

        private Boolean IsRunning()
        {
            if (timer != null && timer.Enabled)
                return true;
            return false;
        }

        /*** EVENT HANDLER ***/

        void clocktimeHandler(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            timeClock.Text = dt.ToString("HH:mm:ss");
        }

        void countdownHandler(object sender, EventArgs e)
        {
            String inputValue = TryGetAsTime(countdownClock.Text);

            if (inputValue == STOP_VALUE)
            {
                SaveSettings();
                ExecuteCountdownAction();
                ResetCountdown();
                return;
            }

            try {
                var selectionStartBefore = countdownClock.SelectionStart;
                var selectionLengthBefore = countdownClock.SelectionLength;
                DateTime dtInput = DateTime.ParseExact(inputValue, "HH:mm:ss",
                    System.Globalization.CultureInfo.InvariantCulture);

                DateTime next = dtInput.Subtract(TimeSpan.FromSeconds(1));
                countdownClock.Text = next.ToString("HH:mm:ss");
                countdownClock.SelectionStart = selectionStartBefore;
                countdownClock.SelectionLength = selectionLengthBefore;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);

                ResetCountdown();
            }
        }

        private void ExecuteCountdownAction()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            /* process info */
            startInfo.FileName = "shutdown.exe";

            /* collect shutdown arguments */
            var optForce = opt_force.Checked;
            if (optForce)
            {
                startInfo.Arguments += "/f ";
            }

            var checkedOption = shutdownOptions.Controls.OfType<RadioButton>().Where(c => c.Checked).First();
            if (checkedOption.Equals(opt_energy))
            {
                startInfo.Arguments += "/h /t 0";
            }
            else if (checkedOption.Equals(opt_shutdown))
            {
                startInfo.Arguments += "/s /t 0";
            }
            else if (checkedOption.Equals(opt_reboot))
            {
                startInfo.Arguments += "/r /t 0";
            }

            
            /* process configuration */
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            //startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //startInfo.RedirectStandardOutput = true;
            //startInfo.RedirectStandardError = true;

            //MessageBox.Show("Der gecheckte Wert ist " + startInfo.Arguments);
            process.StartInfo = startInfo;
            process.Start();

            //string outstring = process.StandardOutput.ReadToEnd();
            //string errstring = process.StandardError.ReadToEnd();

            //process.WaitForExit();

            //if (outstring.Length > 0) Console.WriteLine("STDOUT:\n" + outstring);
            //if (outstring.Length > 0) Console.WriteLine("STDERR:\n" + errstring);
        }

        private void OnTrayExit(object sender, EventArgs e)
        {
            ExitAndSave();
        }




        /* BUTTONS */
        private void button_close_Click(object sender, EventArgs e)
        {
            ExitAndSave();
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            TogglePlayPause();
        }

        private void tab_index_changed(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            ResetCountdown();
        }

        void button_start_MouseEnter(object sender, EventArgs e)
        {
            if (IsRunning())
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_pause_hover));
            else
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_play_hover));
        }

        void button_start_MouseLeave(object sender, EventArgs e)
        {
            if (IsRunning())
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_pause));
            else
                this.button_start.BackgroundImage = ((Image)(Properties.Resources.icon_play));
        }

        void button_stop_MouseEnter(object sender, EventArgs e)
        {
            this.button_stop.BackgroundImage = ((Image)(Properties.Resources.icon_stop_hover));
        }

        void button_stop_MouseLeave(object sender, EventArgs e)
        {
            this.button_stop.BackgroundImage = ((Image)(Properties.Resources.icon_stop));
        }


        private void form_deactivate(object sender, EventArgs e)
        {
            this.Activate();
        }



        /* TABS */
        private void tabs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }

        private void tabs_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var SNAPDIST = 10;

                // keep form in screen bounds
                var newX = e.X + base.Left - MouseDownLocation.X;
                var newY = e.Y + base.Top - MouseDownLocation.Y;
                //Console.WriteLine(newX + "/" + newY+" "+ GetScreenBounds().Width+"/"+ GetScreenBounds().Height);

                if (newX <= SNAPDIST)
                    newX = 0;
                if (newY <= SNAPDIST)
                    newY = 0;

                if (newX >= AppUtil.GetScreenBounds().Width - Bounds.Width - SNAPDIST)
                    newX = AppUtil.GetScreenBounds().Width - Bounds.Width;
                if (newY >= AppUtil.GetScreenBounds().Height - Bounds.Height - SNAPDIST)
                    newY = AppUtil.GetScreenBounds().Height - Bounds.Height;
                
                base.Left = newX;
                base.Top = newY;
            }
        }


        private void countdown_leave(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Text = TryGetAsTime(textBox.Text);
        }

        private string TryGetAsTime(string text)
        {
            var regParsedTime = new Regex(@"^([0-9]{0,2})[^0-9]?([0-9]{0,2})[^0-9]?([0-9]{0,2})");
            var regOnlyNumbers = new Regex(@"^[0-9]{0,6}");

            var regResults = regOnlyNumbers.Matches(text);
            if (regResults.Count == 1)
            {
                while (text.Length < 6)
                    text = "0" + text;
            }

            regResults = regParsedTime.Matches(text);
            if (regResults.Count != 1)
            {
                return LAST_VALIDATED_INPUT;
            }
            else
            {
                List<String> mlist = new List<String>();
                mlist.Add(regResults[0].Groups[1].ToString());
                mlist.Add(regResults[0].Groups[2].ToString());
                mlist.Add(regResults[0].Groups[3].ToString());
                
                TimeSpan ts = new TimeSpan(int.Parse(mlist[0]), int.Parse(mlist[1]), int.Parse(mlist[2]));

                LAST_VALIDATED_INPUT = ts.ToString(@"hh\:mm\:ss");
                return LAST_VALIDATED_INPUT;
            }
        }

        private void countdown_textchange(object sender, EventArgs e)
        {
            var tb = sender as TextBox;
            Console.WriteLine(tb.Text);
        }



        /* drag n drop */
        //private const int WM_NCHITTEST = 0x84;
        //private const int HT_CLIENT = 0x1;
        //private const int HT_CAPTION = 0x2;
        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    if (m.Msg == WM_NCHITTEST)
        //        m.Result = (IntPtr)(HT_CAPTION);
        //}


    }
}
