using System;
using System.Windows;
using System.Windows.Controls;
using VTility.Logic;

namespace VTility
{
    public abstract class BasePage : Page, ISavable
    {
        private UtilRegistry _RegEntry;
        private string _TypeKey = "";

        public BasePage()
        {
            _TypeKey = this.GetType().Name;
            _RegEntry = new UtilRegistry(_TypeKey);

            Loaded += BasePage_Loaded;
            Unloaded += BasePage_Unloaded;

            //if(!_RegEntry.RegistryHasSettingsForTypeKey())
            //    CreatePageKey();
        }

        public object LoadPageSetting(string setting)
        {
            return _RegEntry.RegistryLoadValue(setting);
        }

        ///<summary>Loads Registry Settings when the page has loaded.</summary>
        public abstract void LoadSettings();

        public void SavePageSetting(string setting, object value)
        {
            _RegEntry.RegistrySave(setting, value);
        }

        ///<summary>Saves Registry Settings when the page has loaded.</summary>
        public abstract void SaveSettings();

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            App.Instance.OnExitHandler += OnExit;
            LoadSettings();
        }

        private void BasePage_Unloaded(object sender, RoutedEventArgs e)
        {
            App.Instance.OnExitHandler -= OnExit;
            Console.WriteLine("unloading and saving page settings...");
            SaveSettings();
        }

        private void OnExit()
        {
            Console.WriteLine("exiting and saving page settings...");
            SaveSettings();
        }

        //private void CreatePageKey()
        //{
        //    // Create key for page
        //    Console.WriteLine("Settings for page [" + (this._TypeKey ?? this.ToString()) + "] not found! Initializing them now...");
        //    _RegEntry.RegistryCreateSubKey(this._TypeKey);
        //}

        /* Handling page settings on load and unload */
        //{
        //    if (_RegistryEntry.RegistryHasSettingsForTypeKey())
        //    {
        //        // Load page settings
        //        Console.WriteLine("Settings for [" + (this.Name ?? this.Title ?? this._TypeKey) + "] found! Override this function and parse them.");
        //    }
        //    throw new Exception("Would like to load, but dont know which settings. Override this method to do so!");
        //}
        //{
        //    Console.WriteLine("Would like to save, but dont know which settings. Override this method to do so!");
        //    throw new Exception("Would like to save, but dont know which settings. Override this method to do so!");
        //}
    }
}