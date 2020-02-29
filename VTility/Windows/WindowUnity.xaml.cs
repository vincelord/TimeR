using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace VTility.Windows
{
    public partial class WindowUnity : BaseWindow
    {
        // DLL methods &% constants
        public static readonly int GWL_STYLE = -16;

        public static readonly uint WS_VISIBLE = 0x10000000;
        public static readonly int WM_ACTIVATE = 0x0006;
        public static readonly int WA_ACTIVE = 1;

        //[DllImport("user32.dll")]
        //static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, uint lParam);

        [DllImport("USER32.DLL")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //Sets window attributes
        [DllImport("USER32.DLL")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        //Gets window attributes
        [DllImport("USER32.DLL")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "MoveWindow", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        private Process process = null;
        private IntPtr handleWindowProcess = IntPtr.Zero;
        private IntPtr handleUnityProcess = IntPtr.Zero;

        public WindowUnity()
        {
            InitializeComponent();

            // -------------------------------------------------------------------------------- alternative #1
            //var thisHandle = new WindowInteropHelper(this).Handle;
            //var formsHandle = UnityFormsHost.Handle;
            //TakeProcessViaContainer(filePath, formsHandle);
            //Thread.Sleep(100); // give the process some time to start

            // -------------------------------------------------------------------------------- alternative #2
            //try
            //{
            //    process = new Process();
            //    process.StartInfo.FileName = gamePath;
            //    process.StartInfo.Arguments = "-parentHWND " + panel1.Handle.ToInt32() + " " + Environment.CommandLine;
            //    process.StartInfo.UseShellExecute = true;
            //    process.StartInfo.CreateNoWindow = true;
            //    process.Start();
            //    process.WaitForInputIdle();
            //    // Doesn't work for some reason ?!
            //    //unityHWND = process.MainWindowHandle;
            //    //EnumChildWindows(panel1.Handle, WindowEnum, IntPtr.Zero);
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to Child.exe.");
            //}

            // -------------------------------------------------------------------------------- alternative #3
            //panel2.Children.Add(new UnityHwndHost(gamePath, ""));

            // -------------------------------------------------------------------------------- alternative #4
            //tryIt();

            // -------------------------------------------------------------------------------- alternative #5
            // create process and embed
            //process = new Process();
            //process.StartInfo.FileName = gamePath;
            //var theHWND = panel1.Handle.ToInt32();
            //var args = "-parentHWND " + theHWND + " delayed";

            //Thread.Sleep(3000); // give the process some time to start

            // UnityFormsHost.Child = new UnityHwndHost(gamePath, args);
            // Embed the Windows Standalone application into another application. When you use this, you need to pass the parent application’s window handle(‘HWND’) to the Windows Standalone application.
            // When you pass - parentHWND 'HWND' delayed, the Unity application is hidden while it is running.You must also call SetParent from the Microsoft Developer library for Unity in the application.Microsoft’s SetParent embeds the Unity window.When it creates Unity processes, the Unity window respects the position and size provided as part of the Microsoft’s STARTUPINFO structure.
            // To resize the Unity window, check its GWLP_USERDATA in Microsoft’s GetWindowLongPtr function.Its lowest bit is set to 1 when the graphics initialize and it’s safe to resize.Its second lowest bit is set to 1 after the Unity splash screen finishes displaying.
            // For more information, see this example: EmbeddedWindow.zip
        }

        private void tryIt()
        {
            // open dialog for path to game
            var dialogGameExe = new OpenFileDialog();
            dialogGameExe.DefaultExt = "exe";
            var dialogResult = dialogGameExe.ShowDialog();
            var gamePathExe = dialogGameExe.FileName;

            if (gamePathExe.Trim().Length == 0)
                return;

            var pathGameLastSlash = gamePathExe.LastIndexOf("\\");
            var gameName = gamePathExe.Substring(pathGameLastSlash + 1);

            try
            {
                var procInfo = new ProcessStartInfo(gamePathExe);

                var helper = new WindowInteropHelper(Window.GetWindow(this.uFormsHost));
                //var helper = new WindowInteropHelper(Window.GetWindow(wMain));

                handleWindowProcess = helper.Handle;
                //handleWindowProcess = uFormsHost.Handle;

                procInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(gamePathExe);

                var args = "-parentHWND " + handleWindowProcess + " " + Environment.CommandLine; /// .. delayed
                procInfo.Arguments = args;

                //procInfo.UseShellExecute = true;
                //procInfo.CreateNoWindow = true;

                process = Process.Start(procInfo);

                this.Unloaded += new RoutedEventHandler((s, e) => { process.Dispose(); });

                // Get the main handle
                handleUnityProcess = process.MainWindowHandle;

                //// Put it into this form
                //SetParent(handleUnityProcess, handleWindowProcess);

                //// Remove border and whatnot
                //SetWindowLong(handleUnityProcess, GWL_STYLE, WS_VISIBLE);

                //// Move the window to overlay it on this window
                //MoveWindow(handleUnityProcess, 0, 0, (int)this.ActualWidth, (int)this.ActualHeight, true);

                // Wait for process to be created and enter idle condition
                process.WaitForInputIdle();

                //Thread.Sleep(1500); // give the process some time to start

                // display HWND in label
                uLabelHWND.Content = "Unity HWND: 0x" + handleUnityProcess.ToString("X8");
                uLabelWindow.Content = "Window HWND: 0x" + handleWindowProcess.ToString("X8");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ActivateUnityWindow()
        {
            SendMessage(handleUnityProcess, WM_ACTIVATE, WA_ACTIVE, 0);
        }

        //private void DeactivateUnityWindow()
        //{
        //    SendMessage(handleUnityProcess, WM_ACTIVATE, WA_INACTIVE, 0);
        //}

        //private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        //{
        //    unityHWND = hwnd;
        //    ActivateUnityWindow();
        //    return 0;
        //}

        private void onResize(object sender, EventArgs e)
        {
            MoveWindow(handleUnityProcess, 0, 0, (int)this.Width, (int)this.Height, true);
            ActivateUnityWindow();
        }

        private void embedWindowManually(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            tryIt();
        }
    }
}