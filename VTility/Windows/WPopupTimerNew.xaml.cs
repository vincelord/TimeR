using System;
using System.Windows;
using System.Windows.Threading;
using VTility.Logic;
using static VTility.Logic.TimedShutdownAction;

namespace VTility.Windows
{
    /// <summary>
    /// Interaktionslogik für Popup.xaml
    /// </summary>
    public partial class WPopupTimerNew : BaseWindow
    {
        public WPopupTimerNew()
        {
            //InitializeComponent();

            //Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            //{
            //    var source = PresentationSource.FromVisual(this);
            //    var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            //    var transform = source.CompositionTarget.TransformFromDevice;
            //    var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

            //    this.Left = corner.X - this.ActualWidth - 100;
            //    this.Top = corner.Y - this.ActualHeight;
            //}));
        }

        public void DoPopupClose()
        {
            RaisePopupCloseEvent();
        }

        // Create a custom routed event by first registering a RoutedEventID
        // This event uses the bubbling routing strategy
        public static readonly RoutedEvent PopupCloseEvent = EventManager.RegisterRoutedEvent(
            "PopupClose", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WPopupTimerNew));

        // Provide CLR accessors for the event
        public event RoutedEventHandler PopupClose
        {
            add { AddHandler(WPopupTimerNew.PopupCloseEvent, value); }
            remove { RemoveHandler(WPopupTimerNew.PopupCloseEvent, value); }
        }

        // This method raises the PopupCloseEvent

        private void RaisePopupCloseEvent()
        {
            Console.WriteLine("Raising PopupCloseEvent");
            RoutedEventArgs newEventArgs = new RoutedEventArgs(WPopupTimerNew.PopupCloseEvent, this);
            this.RaiseEvent(newEventArgs);
        }

        // Animation Event handlers
        private void DoubleAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            this.Close();
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

                this.Left = corner.X - this.ActualWidth - 100;
                this.Top = corner.Y - this.ActualHeight;
            }));
        }

        // Window Event handlers
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DoPopupClose();
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            // parse action from dropdown value
            TimedActionType timedActionType;
            if (!TimedActionType.TryParse(TimerAction.Text, true, out timedActionType))
            {
                MessageBox.Show("Error parsing TimedActionType!");
                return;
            }

            string nama = TimerName.Text;
            Console.WriteLine("Action for new timer [" + nama + "] was " + timedActionType.ToString());

            // switch on action
            TimedAction action = default;
            string time = TimerTime.Text;
            bool warn = WarnCheck.IsChecked.Value;

            switch (timedActionType)
            {
                default:
                    MessageBox.Show("Doing nothing then..");
                    this.DoPopupClose();
                    return;

                case TimedActionType.Shutdown:
                    ShutdownType sdType = ShutdownType.Shutdown;  // TODO get real value
                    bool force = true; // TODO get real value

                    action = new TimedShutdownAction(sdType, nama, warn, force);
                    break;
            }

            if (action != default)
            {
                // save last value of old timer in registry
                UtilTimer.Current.Save();

                // create new timer with action associated
                var timer = new UtilTimer(action, time);
            }
            else
            {
                MessageBox.Show("Could not create action, sorry my dude...");
            }

            this.DoPopupClose();
        }
    }
}