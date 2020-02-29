using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows.Data;

namespace VTility.Logic
{
    public class TimerTicker : Multiton<TimerTicker>, INotifyPropertyChanged
    {
        private Timer timer;

        //public TimerTicker() : this(new UtilTimer()) { }
        //public IEnumerable AllTickerEntries =>
        private CollectionView _allTickerEntries;

        public CollectionView AllTickerEntries
        {
            get => _allTickerEntries;
            set
            {
                if (_allTickerEntries == value) return;
                _allTickerEntries = value;
                OnPropertyChanged("AllTickerEntries");
            }
        }

        private string _tickerEntry;

        public string TickerEntry
        {
            get { return _tickerEntry; }
            set
            {
                if (_tickerEntry == value) return;
                _tickerEntry = value;

                // set current instance to last selected
                UtilTimer lastTimer = UtilTimer.AllCountdowns.Where((args) => args.Name.Equals(_tickerEntry)).First();
                if (lastTimer != null)
                    UtilTimer.Current = lastTimer;

                OnPropertyChanged("TickerEntry");
            }
        }

        public DateTime Now => DateTime.Now;
        public string Name => UtilTimer.Current.Name ?? "Timer_Example";

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

        public TimerTicker()
        {
            // start ticker and execute its action periodically
            timer = new Timer();
            timer.Interval = 1000; // 1 second updates
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            ReloadTickerEntries();
        }

        public static void ReloadTickerEntries(IList<UtilTimer> res = null)
        {
            Console.WriteLine("reloading all ticker entries");

            if (res == null)
                res = UtilTimer.AllCountdowns;
            if (res == null)
                res = new List<UtilTimer>();

            UtilTimer.AllCountdowns = res;

            // add all items to list and bind combobox to it
            TimerTicker.Current.AllTickerEntries = new CollectionView(res);

            // set first element
            TimerTicker.Current.TickerEntry = UtilTimer.Current.Name;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Now"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountdownValue"));
        }

        internal static void SetUtilTimers()
        {
        }
    }
}