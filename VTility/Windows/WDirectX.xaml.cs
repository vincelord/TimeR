//using Countdowner;
//using System;
//using System.Diagnostics;
//using System.Drawing;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Runtime.InteropServices;
//using System.Windows;
//using System.Windows.Forms;
//using System.Windows.Input;
//using System.Windows.Interop;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Threading;
//using SharpDX;

//namespace Countdowner
//{
//    /// <summary>
//    /// Interaction logic for WDraw.xaml
//    /// </summary>
//    public partial class WDirectX : BaseWindow
//    {
//        public WDirectX()
//        {
//            InitializeComponent();
//        }

//        public bool InitializeGraphics()
//        {
//            try
//            {
//                // Now  setup our D3D stuff
//                PresentParameters presentParams = new PresentParameters();
//                presentParams.Windowed = true;
//                presentParams.SwapEffect = SwapEffect.Discard;
//                var device = new Device(0, DeviceType.Hardware, this,
//                        CreateFlags.SoftwareVertexProcessing, presentParams);
//                return true;
//            }
//            catch (DirectXException)
//            {
//                return false;
//            }
//        }

//    }

//}