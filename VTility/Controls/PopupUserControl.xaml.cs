using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VTility.Controls
{
    /// <summary>
    /// Interaction logic for PopupUserControl.xaml
    /// </summary>
    public partial class PopupUserControl : UserControl
    {
        // Create a custom routed event by first registering a RoutedEventID
        // This event uses the bubbling routing strategy
        public static readonly RoutedEvent PopupCloseEvent = EventManager.RegisterRoutedEvent(
            "PopupClose", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PopupUserControl));

        public PopupUserControl() : base()
        {
            InitializeComponent();

            buttonCancel.Click += ClickCancelHandler;
        }

        // Provide CLR accessors for the event
        public event RoutedEventHandler PopupClose
        {
            add { AddHandler(PopupUserControl.PopupCloseEvent, value); }
            remove { RemoveHandler(PopupUserControl.PopupCloseEvent, value); }
        }

        public Window frame { get; set; }

        public object SurfaceContent
        {
            get { return surfaceContent.Content; }
            set { surfaceContent.Content = value; }
        }

        public void DoPopupClose()
        {
            RaisePopupCloseEvent();
        }

        public void DoPopupShow()
        {
            // fade in animation
            Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
            {
                var source = PresentationSource.FromVisual(this);
                var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var transform = source.CompositionTarget.TransformFromDevice;
                var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

                frame.Left = corner.X - this.ActualWidth - 100;
                frame.Top = corner.Y - this.ActualHeight;
            }));
        }

        // General Event handlers
        internal void DoubleAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            Console.WriteLine("anim complete");
            frame.Close();
        }

        private void ClickCancelHandler(object sender, RoutedEventArgs e)
        {
            DoPopupClose();
        }

        // This method raises the PopupCloseEvent
        private void RaisePopupCloseEvent()
        {
            Console.WriteLine("Raising PopupCloseEvent");
            RoutedEventArgs newEventArgs = new RoutedEventArgs(PopupUserControl.PopupCloseEvent, this);
            popupFrame.RaiseEvent(newEventArgs);
        }
    }
}