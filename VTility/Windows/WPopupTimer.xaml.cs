using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using VTility.Logic;
using static VTility.Logic.TimedShutdownAction;

namespace VTility.Windows
{
    /// <summary>
    /// Interaction logic for TestReuse.xaml
    /// </summary>
    public partial class WPopupTimer : BasePopup
    {
        //public static readonly DependencyProperty AnimationCompletedProperty = DependencyProperty.Register("AnimationCompleted", typeof(ICommand), typeof(WPopupTimer));
        //public static readonly DependencyProperty CommandCancelProperty = DependencyProperty.Register("CommandCancel", typeof(ICommand), typeof(WPopupTimer));

        //public static readonly DependencyProperty CommandConfirmProperty = DependencyProperty.Register("CommandConfirm", typeof(ICommand), typeof(WPopupTimer));

        //// Create a custom routed event by first registering a RoutedEventID
        //// This event is for the storyboard and uses the bubbling routing strategy
        //public static readonly RoutedEvent PopupClose = EventManager.RegisterRoutedEvent(
        //    "RoutedPopupClose", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WPopupTimer));

        //public static IList<WPopupTimer> All;

        //private static WPopupTimer _current;

        //static WPopupTimer()
        //{
        //    All = new List<WPopupTimer>();
        //}

        public WPopupTimer() : base()
        {
            //WPopupTimer.Current = this;
            //All.Add(this);

            InitializeComponent();

            //// bind cancel
            //CommandCancel = new RoutedCommand("CommandCancel", typeof(WPopupTimer));
            //var binding = new CommandBinding();
            //binding.Command = CommandCancel;
            //binding.Executed += PopupHide;
            //binding.CanExecute += CanExecuteTrue;
            //CommandBindings.Add(binding);

            //// bind anim
            ////AnimationCompleted = new RoutedCommand("AnimationCompleted", typeof(WPopupTimer));
            ////var binding2 = new CommandBinding();
            ////binding2.Command = AnimationCompleted;
            ////binding2.Executed += AnimComplete;
            ////binding2.CanExecute += CanExecuteTrue;
            ////CommandBindings.Add(binding2);

            ////var a=new System.Windows.Media.Animation.DoubleAnimationUsingKeyFrames();
            ////a.Completed += lolz;

            //DoPopupAlignment();
        }

        override internal void ConfirmAction(object sender, ExecutedRoutedEventArgs e)
        {
            this.StartAction();
        }

        //// Provide CLR accessors for the event
        //public event RoutedEventHandler RoutedPopupClose
        //{
        //    add { AddHandler(WPopupTimer.PopupClose, value); }
        //    remove { RemoveHandler(WPopupTimer.PopupClose, value); }
        //}

        //public static WPopupTimer Current
        //{
        //    get => _current;
        //    set => _current = value;
        //}

        //public static WPopupTimer Last => All.Count > 0 ? All[All.Count - 1] : null;
        //public static bool IsOpened => Last != null;

        //public ICommand CommandCancel
        //{
        //    get => (ICommand)GetValue(CommandCancelProperty);
        //    set => SetValue(CommandCancelProperty, value);
        //}

        ////public ICommand AnimationCompleted
        ////{
        ////    get => (ICommand)GetValue(AnimationCompletedProperty);
        ////    set => SetValue(AnimationCompletedProperty, value);
        ////}
        //public ICommand CommandConfirm
        //{
        //    get => (ICommand)GetValue(CommandConfirmProperty);
        //    set => SetValue(CommandConfirmProperty, value);
        //}

        //public void ClosePopup()
        //{
        //    All.Remove(this);
        //    this.Close();
        //}

        //public void DoPopupAlignment()
        //{
        //    // fade in animation
        //    Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() =>
        //    {
        //        var source = PresentationSource.FromVisual(this);
        //        var workingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
        //        var transform = source.CompositionTarget.TransformFromDevice;
        //        var corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));

        //        this.Left = corner.X - this.ActualWidth - 100;
        //        this.Top = corner.Y - this.ActualHeight;
        //    }));
        //}

        //public void PopupCloseDo()
        //{
        //    PopupCloseRaise();
        //}

        //private void CanExecuteTrue(object sender, CanExecuteRoutedEventArgs e)
        //{
        //    e.CanExecute = true;
        //}

        //// This method raises the PopupCloseEvent
        //private void PopupCloseRaise()
        //{
        //    Console.WriteLine("Raising PopupCloseEvent");
        //    RoutedEventArgs newEventArgs = new RoutedEventArgs(WPopupTimer.PopupClose, this);
        //    this.RaiseEvent(newEventArgs);
        //}

        //private void PopupHide(object sender, ExecutedRoutedEventArgs e)
        //{
        //    Console.WriteLine("Starting to hide popup " + this.Name + "...");
        //    this.PopupCloseRaise();
        //}

        private void StartAction()
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
                    this.PopupCloseAnimated();
                    return;

                case TimedActionType.Shutdown:
                    ShutdownType sdType = ShutdownType.Shutdown;  // TODO get real value
                    bool force = true; // TODO get real value

                    action = new TimedShutdownAction(sdType, force, nama, warn);
                    action.image = TimerImages.Shutdown;
                    break;

                case TimedActionType.Notification:
                    var thetext = "bla";
                    action = new TimedNotificationAction(thetext, nama, warn);
                    action.image = TimerImages.Notification;
                    break;

                case TimedActionType.Custom:
                    // TODO
                    //action = new TimedNotificationAction(nama, thetext, warn);
                    //action.image = TimerImages.Custom;
                    break;
            }

            if (action != default)
            {
                // save last value of old timer in registry
                UtilTimer.Current.Save();

                // create new timer with action associated
                var timer = new UtilTimer(action, time);
                //UtilTimer.Current = timer;

                TimerTicker.ReloadTickerEntries();
            }
            else
            {
                MessageBox.Show("Could not create action, sorry my dude...");
            }

            this.PopupCloseAnimated();
        }
    }
}