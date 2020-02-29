using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using VTility.Windows;

namespace VTility.Logic
{
    [Serializable]
    public enum TimedActionType { Notification = 1, Shutdown = 2, Custom = 3 }

    [Serializable]
    public abstract partial class TimedAction
    {
        public string name;
        public TimedActionType type;
        public bool warn;

        public TimedAction(TimedActionType type, string name, bool warn)
        {
            this.name = name;
            this.type = type;
            this.warn = warn;
        }

        public abstract void Execute();
    }

    [Serializable]
    public class TimedNotificationAction : TimedAction
    {
        public string description;

        public TimedNotificationAction(string name, string description, bool warn) : base(TimedActionType.Notification, name, warn)
        {
            this.description = description;
        }

        public override void Execute()
        {
            var pop = new WPopupTimer();

            //pop.WholeContent.Children.Clear();

            var tb = new TextBlock();
            tb.Text = "TimedAction [" + name + "] expired!";
            //pop.WholeContent.Children.Add(tb);

            pop.ShowDialog();
        }
    }

    [Serializable]
    public class TimedShutdownAction : TimedAction
    {
        private bool optForce;

        private ShutdownType shutdownType;

        public TimedShutdownAction(ShutdownType shutdownType, string name, bool warn, bool optForce) : base(TimedActionType.Shutdown, name, warn)
        {
            this.shutdownType = shutdownType;
            this.optForce = optForce;
        }

        public enum ShutdownType { Hibernate = 1, Shutdown, Reboot = 3 }

        public override void Execute()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();

            /* process info */
            startInfo.FileName = "shutdown.exe";

            /* set force shutdown */
            //var optForce = regEntry.RegistryLoad(PageOptions.OPTION_FORCE);
            //bool optForce, ShutdownType shutdownType

            if (true.Equals(optForce))
            {
                startInfo.Arguments += "/f ";
            }

            /* set shutdown type */
            if (shutdownType.Equals(ShutdownType.Hibernate))
            {
                startInfo.Arguments += "/h /t 0";
            }
            else if (shutdownType.Equals(ShutdownType.Shutdown))
            {
                startInfo.Arguments += "/s /t 0";
            }
            else if (shutdownType.Equals(ShutdownType.Reboot))
            {
                startInfo.Arguments += "/r /t 0";
            }

            /* process configuration */
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            process.StartInfo = startInfo;
            process.Start();
        }
    }

    [Serializable]
    public class UtilTimer : Multiton<UtilTimer>
    {
        public static readonly string REGTYPENAME = typeof(UtilTimer).Name;
        public static readonly string REGTYPEPATH = UtilRegistry.REGROOT + "\\" + REGTYPENAME;
        public static IList<UtilTimer> AllCountdowns;
        public static Regex regOnlyNumbers = new Regex(@"^[0-9]{0,6}");
        public static Regex regParsedTime = new Regex(@"^([0-9]{0,2})[^0-9]?([0-9]{0,2})[^0-9]?([0-9]{0,2})");
        public TimedAction countdownAction;
        public TimeSpan countdownValue = new TimeSpan(0, 0, 0);
        private const int INTERVAL_CD = 1000; // equals 1 sec, do not modify!
        private string _name;

        [NonSerialized]
        private UtilRegistry _regEntry;

        private DateTime _startingTime;

        [NonSerialized]
        private DispatcherTimer countdownDispatcher;

        public UtilTimer() : this(new TimedNotificationAction("default", "", false), "00:15:00")
        {
        }

        static UtilTimer()
        {
            AllCountdowns = new List<UtilTimer>();
        }

        public UtilTimer(TimedAction actionOrResource, string timestring)
        {
            Initialise(actionOrResource, timestring);
        }

        public static bool HasTimersRunning => AllCountdowns.Count > 0;
        public bool HasTimeLeft => IsTimerExpired();
        public string Identifier => _regEntry.Identifier;
        public bool IsPlaying => countdownDispatcher != null && countdownDispatcher.IsEnabled;

        public string Name
        {
            get => _name ?? "unnamed";
            set => _name = value;
        }

        public string Path => _regEntry.ResourcePath;
        public double Seconds => countdownValue.TotalSeconds;

        public static TimeSpan TryGetAsTime(string text)
        {
            var regResults = regOnlyNumbers.Matches(text);
            if (regResults.Count == 1)
            {
                while (text.Length < 6)
                    text = "0" + text;
            }

            regResults = regParsedTime.Matches(text);
            if (regResults.Count != 1)
            {
                return TimeSpan.FromSeconds(0);
            }
            else
            {
                List<String> mlist = new List<String>();
                mlist.Add(regResults[0].Groups[1].ToString());
                mlist.Add(regResults[0].Groups[2].ToString());
                mlist.Add(regResults[0].Groups[3].ToString());

                TimeSpan ts = new TimeSpan(int.Parse(mlist[0]), int.Parse(mlist[1]), int.Parse(mlist[2]));

                return ts;
            }
        }

        public void Initialise(TimedAction countdownAction, string timestring = null)
        {
            Name = countdownAction.name; // TODO change to real name
            _regEntry = new UtilRegistry(REGTYPENAME + "\\" + Name);
            Console.WriteLine("preparing regentry for [" + Name + "] at " + _regEntry.ResourcePath);

            TimeSpan parsed;

            // set countdownvalue
            if (timestring == null && _regEntry != null)
                timestring = _regEntry.RegistryLoadValue("timerValue") as string;            // optionally load last stored timespan from registry
            if (timestring != null && TimeSpan.TryParse(timestring, out parsed))
                countdownValue = parsed;
            else
            {
                System.Windows.Forms.MessageBox.Show("Parsing error occured for " + timestring + "!");
                //return;
            }

            // set action
            this.countdownAction = countdownAction;

            AllCountdowns.Add(this);

            Console.WriteLine("Initialised new utiltimer for [" + countdownAction + "]: " + timestring);
        }

        public bool IsTimerExpired()
        {
            return (countdownValue.Equals(TimeSpan.FromSeconds(0)) || countdownValue.TotalSeconds <= 0);
        }

        public DispatcherTimer StartCountdownTimer()
        {
            DispatcherTimer tt = new DispatcherTimer(DispatcherPriority.SystemIdle);
            tt.Tick += new EventHandler(CountdownHandler);
            tt.Interval = TimeSpan.FromMilliseconds(INTERVAL_CD);
            tt.Start();
            _startingTime = DateTime.Now;

            Console.WriteLine("started: " + _startingTime);
            return tt;
        }

        public override string ToString()
        {
            return Name;
        }

        internal static List<UtilTimer> LoadAllFromRegistry()
        {
            Console.WriteLine("Loading all UtilTimers from registry...");
            var res = new List<UtilTimer>();
            string[] keynames = UtilRegistry.RegistryLoadSubkeyNames(REGTYPENAME);
            for (int i = 0; i < keynames.Length; i++)
            {
                var keyname = keynames[i];
                try
                {
                    var ut = UtilTimer.LoadData(keyname);
                    if (ut != null)
                    {
                        ut.Initialise();
                        res.Add(ut);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("There was a problem: " + e);
                    AskDeleteSetting(keyname);
                }
            }

            TimerTicker.ReloadTickerEntries(res);

            return res;
        }

        internal void Delete()
        {
            if (_regEntry != null)
            {
                UtilRegistry.RegistryDeleteKey(Identifier);
                Console.WriteLine("Delete for [" + Path + "] was successful!");
            }
            else
                Console.WriteLine("Delete for [" + Path + "] failed!");
        }

        internal void Save()
        {
            if (_regEntry != null)
            {
                var objString = UtilApplication.ObjectToString(this);
                _regEntry.RegistrySave("objectData", objString);
                Console.WriteLine("Save for [" + Path + "] was successful!");
            }
            else
                Console.WriteLine("Save for [" + Path + "] failed! _regEntry was null...");
        }

        private void Initialise() => Initialise(countdownAction, countdownValue.ToString());

        //internal static bool HasCountDownFor(string action)
        //{
        //    foreach (var t in allCountdowns)
        //    {
        //        if (t.Key.Equals(action))
        //            return true;
        //    }
        //    return false;
        //}

        #region EventHandler

        public void PlayPause()
        {
            if (countdownDispatcher == null)
            { // start new timer
                if (IsTimerExpired())
                {
                    Console.WriteLine("Value is zero. Not doing anything!");
                    return;
                }

                /* (re)start */
                Console.WriteLine("Starting countdown!");
                countdownDispatcher = StartCountdownTimer();
            }
            else if (countdownDispatcher.IsEnabled)
            { // pause
                Console.WriteLine("Pausing and storing value");
                countdownDispatcher.IsEnabled = false;
            }
            else if (!countdownDispatcher.IsEnabled)
            { // continue
                Console.WriteLine("Continuing");
                countdownDispatcher.IsEnabled = true;
            }
        }

        public void ResetCountdown()
        {
            if (countdownDispatcher != null)
            {
                StopCountdown();
                countdownDispatcher = null;
            }
            Console.WriteLine("countdownDispatcher reset.");
        }

        public void StopCountdown()
        {
            if (countdownDispatcher != null)
                countdownDispatcher.Stop();
            Console.WriteLine("countdownDispatcher stopped.");
        }

        internal static UtilTimer LoadData(string timerName)
        {
            var keyPath = UtilTimer.REGTYPEPATH + "\\" + timerName;
            var keyName = "objectData";
            try
            {
                string data = UtilRegistry.RegistryLoadValue(keyName, keyPath) as string;
                return UtilApplication.StringToObject(data) as UtilTimer;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // timer probably damaged
                UtilTimer.AskDeleteSetting(timerName);
            }
            return null;
        }

        internal static string LoadLast() => LoadSetting("lastTimerName");

        internal static string LoadSetting(string settingName)
        {
            try
            {
                string data = UtilRegistry.RegistryLoadValue(settingName, UtilTimer.REGTYPEPATH) as string;
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                // timer probably damaged
                AskDeleteSetting(settingName);
            }
            return null;
        }

        internal static void LoadSettings()
        {
            UtilTimer.LoadAllFromRegistry();

            // load last timer name from registry
            string lastTimerName = UtilTimer.LoadLast() as string;
            if (lastTimerName != null && AllCountdowns.Count > 0)
            {
                // check cache for object before creating a new one
                UtilTimer lastTimer = AllCountdowns.Where((args) => args.Name.Equals(lastTimerName)).First();

                // set last used timer to active scope
                if (lastTimer == null)
                    lastTimer = UtilTimer.LoadData(lastTimerName);
                if (lastTimer != null)
                    UtilTimer.Current = lastTimer;
            }
        }

        internal static void SaveLast(string timerName)
        {
            UtilRegistry.RegistrySave(UtilTimer.REGTYPENAME, "lastTimerName", timerName);
        }

        internal static void ShowAbortDialog()
        {
            var pop = new WPopupAbort();
            //pop.popupFrame.WholeContent.Children.Clear();

            //var dd = new System.Windows.Controls.ComboBox();
            //dd.ItemsSource = UtilTimer.AllCountdowns;
            //pop.popupFrame.WholeContent.Children.Add(dd);

            pop.ShowDialog();
        }

        internal static void ShowNewTimerDialog()
        {
            var a = new WPopupTimer();
            a.ShowDialog();
        }

        private static void AskDeleteSetting(string settingKeyName)
        {
            var res = MessageBox.Show("Setting [" + settingKeyName + "] for UtilTimer is damaged. Should it be deleted from registry?", "bla", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (res.Equals(DialogResult.Yes))
                UtilRegistry.RegistryDeleteKey(settingKeyName, UtilTimer.REGTYPEPATH);
        }

        private void CountdownHandler(object sender, EventArgs e)
        {
            if (IsTimerExpired())
            {
                ExecuteCountdownAction();
                ResetCountdown();
                return;
            }
            countdownValue = countdownValue.Subtract(TimeSpan.FromMilliseconds(INTERVAL_CD));
            Console.WriteLine(countdownValue);
        }

        private void ExecuteCountdownAction()
        {
            if (countdownAction != null)
            {
                countdownAction.Execute();
            }
        }

        #endregion EventHandler
    }
}