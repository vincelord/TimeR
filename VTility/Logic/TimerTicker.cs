using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows.Data;

namespace VTility.Logic
{
    public class TimerImages
    {
        public const string Custom = "/VTility;component/Resources/timer-custom.png";
        public const string Notification = "/VTility;component/Resources/timer-notification.png";
        public const string Shutdown = "/VTility;component/Resources/timer-shutdown.png";
    }

    public class TimerTicker : Multiton<TimerTicker>, INotifyPropertyChanged
    {
        private CollectionView allTickerEntries;

        private string tickerEntry;
        private string tickerTypeImageSource;
        private Timer timer;

        public TimerTicker()
        {
            // start ticker and execute its action periodically
            timer = new Timer();
            timer.Interval = 1000; // 1 second updates
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            ReloadTickerEntries();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public CollectionView AllTickerEntries
        {
            get => allTickerEntries;
            set
            {
                if (allTickerEntries == value) return;
                allTickerEntries = value;
                OnPropertyChanged("AllTickerEntries");
            }
        }

        public string CountdownValue
        {
            get => UtilTimer.Current.countdownValue.ToString() ?? "00:00:00";
            set
            {
                Console.WriteLine("Setting value of [" + UtilTimer.Current + "] to [" + value + "]");

                TimeSpan tmp;
                if (TimeSpan.TryParse(value, out tmp))
                {
                    UtilTimer.Current.countdownValue = tmp;
                }
                else
                {
                    UtilTimer.Current.StopCountdown();
                }
            }
        }

        public string Name => UtilTimer.Current.Name ?? "Timer_Example";

        public DateTime Now => DateTime.Now;

        public string TickerEntry
        {
            get => tickerEntry;
            set
            {
                if (tickerEntry == value) return;
                tickerEntry = value;

                // set current instance to last selected
                var lastTimers = UtilTimer.AllCountdowns.Where((args) => args.Name.Equals(tickerEntry));
                var lastTimer = lastTimers.Count() > 0 ? lastTimers.First() : null;
                if (lastTimer != null)
                    UtilTimer.Current = lastTimer;

                OnPropertyChanged("TickerEntry");
                //TimerTicker.Current.TickerTypeImageSource = UtilTimer.Current.countdownAction.image;
                //OnPropertyChanged("TickerTypeImageSource");
            }
        }

        public string TickerTypeImageSource
        {
            get => tickerTypeImageSource;
            set
            {
                if (tickerTypeImageSource == value) return;
                tickerTypeImageSource = value;
                OnPropertyChanged("TickerTypeImageSource");
            }
        }

        public static void ReloadTickerEntries(IList<UtilTimer> res = null)
        {
            if (UtilTimer.Current == null)
                return;
            Console.WriteLine("reloading all ticker entries");

            // sync to utiltimer
            if (res == null)
                res = UtilTimer.AllCountdowns;
            if (res == null)
                res = new List<UtilTimer>();
            UtilTimer.AllCountdowns = res;

            // add all items to list and bind combobox to it
            TimerTicker.Current.AllTickerEntries = new CollectionView(res);

            // set first element
            TimerTicker.Current.TickerEntry = UtilTimer.Current.Name;

            // set action type image
            TimerTicker.Current.TickerTypeImageSource = UtilTimer.Current.countdownAction.image;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged("Now");
            OnPropertyChanged("CountdownValue");
        }
    }
}