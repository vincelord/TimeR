using System;
using System.Windows;
using VTility.Logic;
using System.Linq;

namespace VTility.Pages
{
    public partial class PageTimer : BasePage
    {
        public PageTimer()
        {
            InitializeComponent();
        }

        public UtilTimer SelectedTimer
        {
            get => UtilTimer.Current;
            set
            {
                UtilTimer.Current = value;
            }
        }
        public override void LoadSettings()
        { 
            //TextBoxCountdown.Text = LoadPageSetting("lastCountdownValue") as string;
            //TextBoxCountdown.Text = UtilTimer.Current.countdownValue.ToString();

            // Loads and displays all timers from registry
            //if (selectedTimer.countdownAction != null)
            //    TextBoxCountdown.Text = LoadPageSetting(selectedTimer.countdownAction.ToString()) as string;

            // TODO Change icon
            //IconTimedActionType.Source = FindResource("")
        }

        public override void SaveSettings()
        {
            UtilTimer.SaveLast(SelectedTimer.Name);
            foreach(var cd in UtilTimer.AllCountdowns)
                cd.Save();
        }

        private void button_play_click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Playing/Pausing");
            SelectedTimer.PlayPause();
        }

        private void button_stop_click(object sender, RoutedEventArgs e)
        {
            if (SelectedTimer.HasTimeLeft)
            {
                Console.WriteLine("Stopping/Resetting");
                SelectedTimer.ResetCountdown();
            }
        }
        private void OnSelectionChange(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var timerName = ComboSelectedTimer.SelectedItem.ToString();
            var res = UtilTimer.AllCountdowns.Where((arg) => arg.Name.Equals(timerName));
            foreach(var a in res)
            {
                Console.WriteLine("Setting new timer to current: " + a.Name);
                UtilTimer.Current = a;
            }
        }
    }
}