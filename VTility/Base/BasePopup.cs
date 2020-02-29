using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace VTility
{
    public class BasePopup : BaseWindow
    {
        //public static readonly DependencyProperty AnimationCompletedProperty = DependencyProperty.Register("AnimationCompleted", typeof(ICommand), typeof(WPopupTimer));
        public static readonly DependencyProperty CommandCancelProperty = DependencyProperty.Register("CommandCancel", typeof(ICommand), typeof(BasePopup));

        public static readonly DependencyProperty CommandConfirmProperty = DependencyProperty.Register("CommandConfirm", typeof(ICommand), typeof(BasePopup));

        // Create a custom routed event by first registering a RoutedEventID
        // This event is for the storyboard and uses the bubbling routing strategy
        public static readonly RoutedEvent PopupClose = EventManager.RegisterRoutedEvent(
            "RoutedPopupClose", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BasePopup));

        public static IList<BasePopup> All;

        internal static BasePopup _current;

        public BasePopup() : base()
        {
            BasePopup.Current = this;
            All.Add(this);

            //InitializeComponent();

            // bind cancel
            CommandCancel = new RoutedCommand("CommandCancel", typeof(BasePopup));
            var binding = new CommandBinding();
            binding.Command = CommandCancel;
            binding.Executed += PopupHide;
            binding.CanExecute += CanExecuteTrue;
            CommandBindings.Add(binding);

            // bind confirm button
            CommandConfirm = new RoutedCommand("CommandConfirm", typeof(BasePopup));
            var binding2 = new CommandBinding();
            binding2.Command = CommandConfirm;
            binding2.Executed += ConfirmAction;
            binding2.CanExecute += CanExecuteTrue;
            CommandBindings.Add(binding2);

            // bind anim
            //AnimationCompleted = new RoutedCommand("AnimationCompleted", typeof(WPopupTimer));
            //var binding2 = new CommandBinding();
            //binding2.Command = AnimationCompleted;
            //binding2.Executed += AnimComplete;
            //binding2.CanExecute += CanExecuteTrue;
            //CommandBindings.Add(binding2);

            //var a=new System.Windows.Media.Animation.DoubleAnimationUsingKeyFrames();
            //a.Completed += lolz;

            DoPopupAlignment();
        }

        static BasePopup()
        {
            All = new List<BasePopup>();
        }

        // Provide CLR accessors for the event
        public event RoutedEventHandler RoutedPopupClose
        {
            add { AddHandler(BasePopup.PopupClose, value); }
            remove { RemoveHandler(BasePopup.PopupClose, value); }
        }

        public static BasePopup Current
        {
            get => _current;
            set => _current = value;
        }

        public static bool IsOpened => Last != null;
        public static BasePopup Last => All.Count > 0 ? All[All.Count - 1] : null;

        public ICommand CommandCancel
        {
            get => (ICommand)GetValue(CommandCancelProperty);
            set => SetValue(CommandCancelProperty, value);
        }

        //public ICommand AnimationCompleted
        //{
        //    get => (ICommand)GetValue(AnimationCompletedProperty);
        //    set => SetValue(AnimationCompletedProperty, value);
        //}
        public ICommand CommandConfirm
        {
            get => (ICommand)GetValue(CommandConfirmProperty);
            set => SetValue(CommandConfirmProperty, value);
        }

        public void ClosePopup()
        {
            All.Remove(this);
            this.Close();
        }

        public void DoPopupAlignment()
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

        public void PopupCloseAnimated()
        {
            PopupCloseRaise();
        }

        internal void CanExecuteTrue(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        // This method raises the PopupCloseEvent
        private void PopupCloseRaise()
        {
            Console.WriteLine("Raising PopupCloseEvent");
            RoutedEventArgs newEventArgs = new RoutedEventArgs(BasePopup.PopupClose, this);
            this.RaiseEvent(newEventArgs);
        }

        internal void PopupHide(object sender, ExecutedRoutedEventArgs e)
        {
            Console.WriteLine("Starting to hide popup " + this.Name + "...");
            this.PopupCloseRaise();
        }

        internal virtual void ConfirmAction(object sender, ExecutedRoutedEventArgs e)
        {
            Console.WriteLine("Executing confirm action for " + this.Name + "...");
        }
    }
}