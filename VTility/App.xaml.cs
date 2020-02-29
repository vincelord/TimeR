using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Windows;
using System.Windows.Input;
using VTility.Logic;
using VTility.Windows;

namespace VTility
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon notifyIcon;

        public static App Instance;

        public App()
        {
            if (Instance != null)
                return;
            Instance = this;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            //I am creating a delegate (pointer) to HandleSomethingHappened
            //and adding it to SomethingHappened's list of "Event Handlers".
            //myObj.SomethingHappened += new MyEventHandler(HandleOnExit);

            // load all timers from registry into application
            UtilTimer.LoadSettings();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner

            HandleOnExit();

            base.OnExit(e);
        }

        //This delegate can be used to point to methods which return void
        public delegate void ExitEventHandler();

        //This event can cause any method which conforms to MyEventHandler to be called.
        public event ExitEventHandler OnExitHandler;

        //Here is some code I want to be executed when the handler fires.
        private void HandleOnExit()
        {
            //Do some stuff
            OnExitHandler?.Invoke();
        }

        private void DoubleAnimationUsingKeyFrames_Completed(object sender, System.EventArgs e)
        {
            Console.WriteLine("Popup closing animation completed.");
            WPopupTimer.Current.ClosePopup();
            //var c = sender as WPopupTimer;
            //c.Close();
        }
    }
}