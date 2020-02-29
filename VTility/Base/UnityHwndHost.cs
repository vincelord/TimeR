using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Interop;

namespace VTility.Base
{
    internal class UnityHwndHost : HwndHost
    {
        internal const Int32 GWLP_USERDATA = -21;

        internal const UInt32 WM_CLOSE = 0x0010;

        private string arguments;

        private Process process = null;

        private string programName;

        private IntPtr unityHWND = IntPtr.Zero;

        public UnityHwndHost(string programName, string arguments = "")
        {
            this.programName = programName;
            this.arguments = arguments;
        }

        internal delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);

        //Gets window attributes
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        //Sets a window to be a child window of another window
        [DllImport("user32.dll")]
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //Sets window attributes
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        internal static extern bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")] // TODO: 32/64?
        internal static extern IntPtr GetWindowLongPtr(IntPtr hWnd, Int32 nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint processId);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            Debug.WriteLine("Going to launch Unity at: " + this.programName + " " + this.arguments);
            process = new Process();
            process.StartInfo.FileName = programName;
            process.StartInfo.Arguments = arguments + (arguments.Length == 0 ? "" : " ") + "-parentHWND " + hwndParent.Handle;
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.CreateNoWindow = true;

            process.Start();
            process.WaitForInputIdle();

            int repeat = 50;
            while (unityHWND == IntPtr.Zero && repeat-- > 0)
            {
                Thread.Sleep(100);
                EnumChildWindows(hwndParent.Handle, WindowEnum, IntPtr.Zero);
            }
            if (unityHWND == IntPtr.Zero)
                throw new Exception("Unable to find Unity window");
            Debug.WriteLine("Found Unity window: " + unityHWND);

            repeat += 150;
            while ((GetWindowLongPtr(unityHWND, GWLP_USERDATA).ToInt32() & 1) == 0 && --repeat > 0)
            {
                Thread.Sleep(100);
                Debug.WriteLine("Waiting for Unity to initialize... " + repeat);
            }
            if (repeat == 0)
            {
                Debug.WriteLine("Timed out while waiting for Unity to initialize");
            }
            else
            {
                Debug.WriteLine("Unity initialized!");
            }

            return new HandleRef(this, unityHWND);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Debug.WriteLine("Asking Unity to exit...");
            PostMessage(unityHWND, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            int i = 30;
            while (!process.HasExited)
            {
                if (--i < 0)
                {
                    Debug.WriteLine("Process not dead yet, killing...");
                    process.Kill();
                }
                Thread.Sleep(100);
            }
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        private static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private void TakeProcessViaContainer(string name, IntPtr formsHandle)
        {
            try
            {
                Thread.Sleep(500); // give the process some time to start

                process = new Process();
                process.StartInfo.FileName = name;

                process.StartInfo.Arguments = "-parentHWND " + formsHandle.ToInt32() + " " + Environment.CommandLine;
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                Thread.Sleep(500); // give the process some time to start

                process.WaitForInputIdle();

                //SetParent(process.MainWindowHandle, thisHandle);

                // Doesn't work for some reason ?!
                unityHWND = process.MainWindowHandle;

                EnumChildWindows(formsHandle, WindowEnum, IntPtr.Zero);

                //Texty.Text = "Unity HWND: 0x" + unityHWND.ToString("X8");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + ".\nCheck if Container.exe is placed next to UnityGame.exe.");
            }
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            if (unityHWND != IntPtr.Zero)
                throw new Exception("Found multiple Unity windows");
            unityHWND = hwnd;
            return 0;
        }

        //public static void ProcessReStyle(string procName)
        //{
        //    Process[] Procs = Process.GetProcesses();
        //    foreach (Process proc in Procs)
        //    {
        //        if (proc.ProcessName.StartsWith(procName))
        //        {
        //            var msg = MessageBox.Show("Process found! " + procName);
        //            IntPtr pFoundWindow = proc.MainWindowHandle;
        //            int style = GetWindowLong(pFoundWindow, GWL_STYLE);
        //            SetWindowLong(pFoundWindow, GWL_STYLE, (style & ~WS_CAPTION));
        //        }
        //    }
        //}
        //public static implicit operator Control(UnityHwndHost v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}