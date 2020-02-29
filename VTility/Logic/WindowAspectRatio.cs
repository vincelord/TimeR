namespace VTility.Logic
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Interop;

    internal class WindowAspectRatio
    {
        private double _ratio;
        private Rect _sizeOrig;
        private Window _window;
        private Rect _clipping;

        private WindowAspectRatio(Window window, Rect clipping)
        {
            _sizeOrig = new Rect();
            _sizeOrig.Width = window.Width;
            _sizeOrig.Height = window.Height;
            _window = window;

            _clipping = clipping;

            _ratio = window.Width / window.Height;

            ((HwndSource)HwndSource.FromVisual(window)).AddHook(DragHook);
        }

        public static void Register(Window window)
        {
            new WindowAspectRatio(window, new Rect(0, 0, 0, 0));
        }

        public static void Register(Window window, Rect clipping)
        {
            new WindowAspectRatio(window, clipping);
        }

        internal enum WM
        {
            WINDOWPOSCHANGING = 0x0046,
        }

        [Flags()]
        public enum SWP
        {
            NoMove = 0x2,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int flags;
        }

        private IntPtr DragHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handeled)
        {
            if ((WM)msg == WM.WINDOWPOSCHANGING)
            {
                WINDOWPOS position = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));

                if ((position.flags & (int)SWP.NoMove) != 0 || HwndSource.FromHwnd(hwnd).RootVisual == null)
                    return IntPtr.Zero;

                // limit height
                if (position.cy >= Screen.PrimaryScreen.Bounds.Height)
                    return IntPtr.Zero;

                // beware aspect ratio
                //position.cx = (int)(position.cy * GetClippedRatio());
                position.cy = position.cx + (int)_clipping.Top;

                Marshal.StructureToPtr(position, lParam, true);
                handeled = true;
            }

            return IntPtr.Zero;
        }

        private double GetClippedRatio()
        {
            return (_sizeOrig.Width) / (_sizeOrig.Height);
        }
    }
}