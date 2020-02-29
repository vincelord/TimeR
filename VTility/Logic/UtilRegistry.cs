using Microsoft.Win32;
using System;

namespace VTility.Logic
{
    [Serializable]
    public class UtilRegistry
    {
        public const string REGROOT = UtilApplication.APP_REGROOT; // ex: Software\\Vtility

        private string _resourceIdentifier;

        public UtilRegistry(string resourceIdentifier)
        {
            this._resourceIdentifier = resourceIdentifier;
        }

        public string ResourcePath => REGROOT + "\\" + _resourceIdentifier; // for use in registry
        public string Identifier => _resourceIdentifier; // for use in registry

        public static void RegistryDeleteKey(string keyName, string path = UtilRegistry.REGROOT)
        {
            Console.WriteLine("-- Deleting key from registry: " + path + "\\" + keyName);
            RegistryKey progSettings = Registry.CurrentUser.OpenSubKey(path, true);
            try
            {
                progSettings.DeleteSubKey(keyName);
            }
            catch (Exception e)
            {
                Console.WriteLine("Key does not exist? " + e);
            }
            progSettings.Close();
        }

        public static void RegistryDeleteValue(string keyName, string path = UtilRegistry.REGROOT)
        {
            Console.WriteLine("-- Deleting value from registry: " + path + "\\" + keyName);
            RegistryKey progSettings = Registry.CurrentUser.OpenSubKey(path, true);
            progSettings.DeleteValue(keyName, false);
            progSettings.Close();
        }

        public static string[] RegistryLoadSubkeyNames(string key, string path = UtilRegistry.REGROOT)
        {
            string[] keynames;
            RegistryKey progSettings = Registry.CurrentUser.OpenSubKey(path + "\\" + key, true);
            keynames = progSettings.GetSubKeyNames();
            progSettings.Close();
            return keynames;
        }

        public static object RegistryLoadValue(string key, string path = UtilRegistry.REGROOT)
        {
            Console.WriteLine("-- Loading value from registry: " + path + "\\" + key);
            RegistryKey progSettings = Registry.CurrentUser.OpenSubKey(path, true);
            if (progSettings == null)
                return null;

            object setting = progSettings.GetValue(key, false);
            progSettings.Close();

            return setting;
        }

        public static void RegistrySave(string nameOrPath, string key, object value, string basePath = REGROOT)
        {
            //RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(RegKeyBase, true);
            RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(basePath, true);

            // # 1
            //RegistryKey regKey = ProgSettings;
            //regKey = regKey.CreateSubKey(ResourceIdentifier);
            //regKey.SetValue("ValueName", "Value");
            //regKey.Close();

            // #2
            RegistryKey regKey = ProgSettings;
            string[] RegKeys = nameOrPath.Split('\\');
            foreach (string keyA in RegKeys)
            {
                regKey = regKey.CreateSubKey(keyA);
            }
            regKey.SetValue(key, value);

            //if (ProgSettings == null)
            //{
            //    RegistryCreateSubKey(ResourceIdentifier);
            //}

            //ProgSettings = Registry.CurrentUser.OpenSubKey(ResourcePath, true);
            //ProgSettings.SetValue(key, value);
            ProgSettings.Close();
        }

        public void RegistryDeleteValue(string key) => RegistryDeleteValue(key, ResourcePath);

        public object RegistryLoadValue(string key) => RegistryLoadValue(key, ResourcePath);

        public void RegistrySave(string key, object value) => RegistrySave(_resourceIdentifier, key, value, REGROOT);

        //public bool RegistryHasSettingsForTypeKey()
        //{
        //    RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(ResourcePath, false);
        //    bool hasKey = ProgSettings != null && (ProgSettings.ValueCount > 0 || ProgSettings.SubKeyCount > 0);
        //    if (hasKey)
        //    {
        //        ProgSettings.Close();
        //    }
        //    return hasKey;
        //}

        //public void RegistryCreateSubKey(string keyname)
        //{
        //    RegistryKey ProgSettings = Registry.CurrentUser.OpenSubKey(ResourcePath, true);

        //    // create and open app folder/key
        //    if (ProgSettings == null)
        //    {
        //        ProgSettings = Registry.CurrentUser.OpenSubKey("Software", true);
        //        ProgSettings.CreateSubKey(RegKeyBase);
        //        ProgSettings.Close();
        //        ProgSettings = Registry.CurrentUser.OpenSubKey("Software\\" + RegKeyBase, true);
        //    }

        //    if (!ProgSettings.GetSubKeyNames().Contains(keyname))
        //        ProgSettings.CreateSubKey(keyname);
        //    ProgSettings.Close();
        //}
    }
}