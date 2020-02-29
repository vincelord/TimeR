using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using VTility.Logic;

namespace VTility.Pages
{
    /// <summary>
    /// Interaktionslogik für PageExperiment.xaml
    /// </summary>
    public partial class PageExperiment : BasePage
    {
        // for fileIO
        private string _tmpPath = Path.GetTempPath();

        // data container
        private String _radioVisType;

        private String _radioAspect;

        // for animation
        private Timer _timer = new Timer();

        // color modifiers
        private int _mod1 = 0;

        private int _mod2 = 0;
        private int _mod3 = 0;
        private int _mod4 = 0;

        private int _bpp = System.Windows.Forms.Screen.PrimaryScreen.BitsPerPixel;

        private int _frameCounter = 0;

        // for image calculations (pixels, stride, etc.)
        private WriteableBitmap _writablebitmap;

        private int _width;
        private int _height;

        private Int32Rect _rect;
        private int _bytesPerPixel;
        private int _stride;
        private int _arraySize;
        private int _dpi;
        private byte[] _array;
        private System.Windows.Media.PixelFormat _format;
        private object[] _lastOptions;

        public PageExperiment()
        {
            InitializeComponent();

            PrepareCanvas();
        }

        public override void LoadSettings()
        {
            //throw new NotImplementedException();
        }

        public override void SaveSettings()
        {
            //throw new NotImplementedException();
        }

        public void RecalcCanvas(System.Drawing.Imaging.PixelFormat format)
        {
            // TODO get format by url ending / mime type?
            _format = UtilDrawing.ConvertPixelFormat(format);

            // Calculate the number of bytes per pixel.
            _bytesPerPixel = (_format.BitsPerPixel + 7) / 8;

            // Stride is bytes per pixel times the number of pixels.
            // Stride is the byte width of a single rectangle row.
            _stride = _width * _bytesPerPixel;
        }

        public void PrepareCanvas()
        {
            var currentOptions = ReadOptions();

            bool newBmp = false;
            if (!currentOptions.Equals(_lastOptions))
            {
                newBmp = true;
                _lastOptions = currentOptions;
            }

            // Create the image and writeable bitmap will be used to write and update.
            //VisualizationImage = new Image();
            if (newBmp || _writablebitmap == null)
            {
                _writablebitmap = new WriteableBitmap(_width, _height, _dpi, _dpi, _format, null);

                // Define the rectangle of the writeable image we will modify.
                // The size is that of the writeable bitmap.
                _rect = new Int32Rect(0, 0, _writablebitmap.PixelWidth, _writablebitmap.PixelHeight);

                // Calculate the number of bytes per pixel.
                _bytesPerPixel = (_format.BitsPerPixel + 7) / 8;

                // Stride is bytes per pixel times the number of pixels.
                // Stride is the byte width of a single rectangle row.
                _stride = _writablebitmap.PixelWidth * _bytesPerPixel;

                // Create a byte array for a the entire size of bitmap.
                _arraySize = _stride * _writablebitmap.PixelHeight;
                _array = new byte[_arraySize];

                //Set the Image source.
                ImageContainer2.Source = _writablebitmap;
            }
        }

        private object[] ReadOptions()
        {
            // TODO parse input options
            _width = UtilApplication.ParseAsInt(TextBoxWidth.Text, 1);
            _height = UtilApplication.ParseAsInt(TextBoxHeight.Text, 1);
            _dpi = 96;
            _format = System.Windows.Media.PixelFormats.Bgra32;

            return new object[] { _width, _height, _dpi, _format };
        }

        public void WriteRandomPicture()
        {
            PrepareCanvas();

            //Console.WriteLine("Generating random image... ");

            //Update the color array with new random colors
            Random value = new Random();
            value.NextBytes(_array);

            //Update writeable bitmap with the colorArray to the image.
            _writablebitmap.WritePixels(_rect, _array, _stride, 0);
        }

        public void WriteGradient()
        {
            PrepareCanvas();

            // Create an array of pixels to contain pixel color values
            uint[] pixels = new uint[_width * _height];

            for (int x = 0; x < _width; ++x)
            {
                for (int y = 0; y < _height; ++y)
                {
                    int i = _width * y + x;

                    var count = (int)_frameCounter % 256;

                    // map bgra > rgba
                    var red = 256 / 100 * _mod1 * y / _height;
                    var green = 256 / 100 * _mod2;
                    var blue = 256 / 100 * _mod3 * x / _width;
                    var alpha = 256 / 100 * _mod4 * x * y / _width / _height;

                    pixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }

            try
            {
                _writablebitmap.WritePixels(_rect, pixels, _stride, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("error..." + e);
            }
        }

        public void WriteOurs()
        {
            PrepareCanvas();

            // args
            int anzahlFlaechen = 16;

            // init
            double pixelWert = 256 / anzahlFlaechen;

            // Create an array of pixels to contain pixel color values
            uint[] pixels = new uint[_width * _height];
            for (double pixelX = 0; pixelX < _width; ++pixelX)
            {
                for (double pixelY = 0; pixelY < _height; ++pixelY)
                {
                    var count = (int)_frameCounter % 360;

                    double xPercent = pixelX / _width;
                    double yPercent = pixelY / _height;

                    //double xRel = x / _width * _height;
                    //double yRel = y / _height * _width;

                    var dieseFarbe = 1;

                    //if (x < _height / _width)
                    //    dieseFarbe = 2;

                    var formulaY = GetFormulaX(xPercent);
                    var formulaX = -(GetFormulaX(-xPercent + 1)) + 0.5;

                    if (yPercent < formulaY)
                        dieseFarbe++;
                    if (yPercent < formulaX)
                        dieseFarbe++;

                    var alpha = dieseFarbe * 256 / anzahlFlaechen;

                    // map bgra > rgba
                    var red = 256 / 100 * _mod1;
                    var green = 256 / 100 * _mod2;
                    var blue = 256 / 100 * _mod3;
                    int i = (int)(_width * pixelY + pixelX);
                    pixels[i] = (uint)((alpha << 24) + (red << 16) + (green << 8) + blue);
                }
            }

            try
            {
                _writablebitmap.WritePixels(_rect, pixels, _stride, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine("error... " + e);
            }
        }

        private double GetFormulaX(double xPercent)
        {
            return (4 * _mod1 / 5 * Math.Pow(xPercent - 0.5, 3) - 3 * _mod2 / 5 * Math.Pow(xPercent - 0.5, 2)) + 0.5;
        }

        private void WriteScreenShot()
        {
            var format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
            RecalcCanvas(format);

            this.Dispatcher.BeginInvoke((Action)delegate ()
            {
                Bitmap screen = UtilDrawing.TakeScreen(format).TransformImage2(_width, _height);

                // saves file
                //var fileName = tmpPath + "filename2.jpeg";
                //var fileName2 = tmpPath + "filename3.jpeg";
                //newImg.Save(fileName, ImageFormat.Png);

                // opens explorer and selects file
                //string argument = "/select, \"" + fileName + "\"";
                //Process.Start("explorer.exe", argument);

                // write bitmap to writablebitmap
                //var bpp = Utilities.GetBytesPerPixel(format);

                //int stride = Utilities.GetStride(_bpp, _width);
                byte[] pixels = new byte[UtilDrawing.GetSize(_stride, screen.Height)];

                BitmapSource baseLayer = UtilDrawing.CreateBitmapSourceFromBitmap(screen, System.Drawing.Imaging.ImageFormat.Bmp);
                baseLayer.CopyPixels(pixels, _stride, 0);

                _writablebitmap.WritePixels(_rect, pixels, _stride, 0);
            }, DispatcherPriority.Render);
        }

        public void WriteOnlinePicture()
        {
            PrepareCanvas();

            //Uri uri = new Uri(TextBoxURL.Text);
            //BitmapImage bitmapImage = new BitmapImage();
            //bitmapImage.CreateOptions = BitmapCreateOptions.None;
            ////bitmapImage.ImageOpened += ImageOpened;
            //bitmapImage.UriSource = uri;
            //ImageTest.Source = new WriteableBitmap(bitmapImage);

            try
            {
                WebClient client = new WebClient();
                Bitmap bitmap;

                using (Stream BitmapStream = client.OpenRead(TextBoxURL.Text))
                {
                    Image img = Image.FromStream(BitmapStream);
                    img = img.CopyWithTransparency();
                    bitmap = new Bitmap(img);
                }

                //Stream stream = client.OpenRead(TextBoxURL.Text);
                //var bitmap = new Bitmap(stream);
                //stream.Flush();
                //stream.Close();

                //var fileName = tmpPath + "filename.jpeg";
                //bitmap.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                //Console.WriteLine("Image written as " + fileName);

                var baseLayer = UtilDrawing.CreateBitmapSourceFromBitmap(bitmap, System.Drawing.Imaging.ImageFormat.Bmp);

                //var format = System.Drawing.Imaging.PixelFormat.Format32bppArgb;
                var format = System.Drawing.Imaging.PixelFormat.Format32bppRgb;
                //var format = baseLayer.Format;
                var bpp = UtilDrawing.GetBytesPerPixel(format);
                var stride = UtilDrawing.GetStride(bpp, bitmap.Width);
                var pixels = new byte[UtilDrawing.GetSize(stride, bitmap.Height)];

                baseLayer.CopyPixels(pixels, stride, 0);

                _writablebitmap.WritePixels(new Int32Rect(0, 0, baseLayer.PixelWidth, baseLayer.PixelHeight), pixels, stride, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void t1_Tick(object sender, EventArgs e)
        {
            // image container
            switch (_radioAspect)
            {
                case UtilDrawingConstants.FILL:
                    ImageContainer2.Stretch = Stretch.Fill;
                    break;

                case UtilDrawingConstants.KEEP:
                    ImageContainer2.Stretch = Stretch.Uniform;
                    break;

                default: break;
            }

            // get generation algorith
            switch (_radioVisType)
            {
                case UtilDrawingConstants.RANDOM:
                    WriteRandomPicture();
                    break;

                case UtilDrawingConstants.SCREENSHOT:
                    WriteScreenShot();
                    break;

                case UtilDrawingConstants.SPIRALE:
                    WriteOurs();
                    break;

                case UtilDrawingConstants.GRADIENT:
                    WriteGradient();
                    break;

                default: break;
            }
            _frameCounter++;
        }

        // Animation Stuff...
        private void StartAnimation(int fps)
        {
            Console.WriteLine("Animation started with " + fps + " fps.");
            if (_timer == null) _timer = new Timer(); // Timer anlegen
            _timer.Interval = GetFPSInterval(fps); // Intervall festlegen, hier 100 ms
            _timer.Tick += new EventHandler(t1_Tick); // Eventhandler ezeugen der beim Timerablauf aufgerufen wird
            _timer.Start(); // Timer starten
        }

        private int GetFPSInterval(int fps)
        {
            int theFps = fps > 0 ? fps : 1;
            return 1000 / theFps;
        }

        private void StopAnimation()
        {
            _timer.Enabled = false;
            _timer.Dispose();
            _timer = new Timer();
        }

        private void slider_value_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _timer.Interval = GetFPSInterval((int)SliderFPS.Value);
        }

        private void button_start_click(object sender, RoutedEventArgs e)
        {
            StartAnimation((int)SliderFPS.Value);
        }

        private void button_stop_click(object sender, RoutedEventArgs e)
        {
            StopAnimation();
        }

        private void button_picture_online(object sender, RoutedEventArgs e)
        {
            WriteOnlinePicture();
        }

        private void button_picture_sample(object sender, RoutedEventArgs e)
        {
            WriteGradient();
        }

        private void RadioRandom_Checked(object sender, RoutedEventArgs e)
        {
            _radioVisType = UtilDrawingConstants.RANDOM;
        }

        private void RadioScreen_Checked(object sender, RoutedEventArgs e)
        {
            _radioVisType = UtilDrawingConstants.SCREENSHOT;
        }

        private void RadioGradient_Checked(object sender, RoutedEventArgs e)
        {
            _radioVisType = UtilDrawingConstants.GRADIENT;
        }

        private void RadioSpirale_Checked(object sender, RoutedEventArgs e)
        {
            _radioVisType = UtilDrawingConstants.SPIRALE;
        }

        private void slider_alpha_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mod4 = (int)SliderAlpha.Value;
        }

        private void slider_red_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mod1 = (int)SliderRed.Value;
        }

        private void slider_green_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mod2 = (int)SliderGreen.Value;
        }

        private void slider_blue_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _mod3 = (int)SliderBlue.Value;
        }

        private void options_expanded(object sender, RoutedEventArgs e)
        {
            ControlsGridContainer.Margin = new Thickness(0, 0, 0, 0);
        }

        private void options_collapsed(object sender, RoutedEventArgs e)
        {
            ControlsGridContainer.Margin = new Thickness(0, ControlsGridContainer.ActualHeight, 0, 0);
        }

        private void RadioKeep_Checked(object sender, RoutedEventArgs e)
        {
            _radioAspect = UtilDrawingConstants.KEEP;
        }

        private void RadioFill_Checked(object sender, RoutedEventArgs e)
        {
            _radioAspect = UtilDrawingConstants.FILL;
        }
    }
}