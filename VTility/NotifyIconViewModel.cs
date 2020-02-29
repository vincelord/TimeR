using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using VTility.Logic;
using VTility.Windows;

namespace VTility
{
    /// <summary>
    /// Provides bindable properties and commands for the NotifyIcon. In this sample, the
    /// view model is assigned to the NotifyIcon in XAML. Alternatively, the startup routing
    /// in App.xaml.cs could have created this view model, and assigned it to the NotifyIcon.
    /// </summary>
    public class NotifyIconViewModel : INotifyPropertyChanged
    {
        //private bool _playing = false;
        //public bool IsPlaying { get { return _playing; } set { _playing = value; OnPropertyChanged("IsPlaying"); } }
        public string LabelPlay => "Play";

        public string LabelPause => "Pause";

        public ICommand TimerNewCommand => new DelegateCommand
        {
            CanExecuteFunc = () => !WPopupTimer.IsOpened,
            CommandAction = () =>
            {
                UtilTimer.ShowNewTimerDialog();
            }
        };

        public ICommand TimerAbortCommand => new DelegateCommand
        {
            CanExecuteFunc = () => !WPopupTimer.IsOpened && UtilTimer.HasTimersRunning,
            CommandAction = () =>
            {
                UtilTimer.ShowAbortDialog();
            }
        };

        //public ICommand TimerResetCommand => new DelegateCommand
        //{
        //    CanExecuteFunc = () => UtilTimer.Instance != null && !UtilTimer.Instance.HasTimeLeft,
        //    CommandAction = () =>
        //    {
        //        UtilTimer.Instance.ResetCountdown();
        //    }
        //};

        /// <summary>
        /// Shows a window, if none is already open.
        /// </summary>
        public ICommand ShowWindowCommand => new DelegateCommand
        {
            CanExecuteFunc = () => Application.Current.MainWindow == null || !Application.Current.MainWindow.IsLoaded,
            CommandAction = () =>
            {
                Application.Current.MainWindow = new WMain();
                Application.Current.MainWindow.Show();
            }
        };

        /// <summary>
        /// Hides the main window. This command is only enabled if a window is open.
        /// </summary>
        public ICommand HideWindowCommand => new DelegateCommand
        {
            CanExecuteFunc = () => Application.Current.MainWindow != null && Application.Current.MainWindow.Focusable,
            CommandAction = () => Application.Current.MainWindow.Close()
        };

        public ICommand ResetWindowCommand => new DelegateCommand
        {
            CanExecuteFunc = () => Application.Current.MainWindow != null && Application.Current.MainWindow.Focusable,
            CommandAction = () =>
            {
                ((BaseWindow)Application.Current.MainWindow).WindowReset();
            }
        };

        /// <summary>
        /// Shuts down the application.
        /// </summary>
        public ICommand ExitApplicationCommand => new DelegateCommand { CommandAction = () => Application.Current.Shutdown() };

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String info)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(info));
            }
        }
    }

    /// <summary>
    /// Simplistic delegate command for the demo.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        public Action CommandAction { get; set; }
        public Func<bool> CanExecuteFunc { get; set; }

        public void Execute(object parameter)
        {
            CommandAction();
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteFunc == null || CanExecuteFunc();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}