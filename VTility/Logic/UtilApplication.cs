using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace VTility.Logic
{
    public class UtilApplication : Singleton<UtilApplication>
    {
        public const String APP_NAME = "VTility";
        public const String APP_PRFX = "VT_";
        public const String APP_REGROOT = "Software\\" + APP_NAME;

        protected static String APP_LOCATION = System.Reflection.Assembly.GetExecutingAssembly().Location;

        // The path to the key where Windows looks for startup applications
        private static RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);


        public static string ObjectToString(object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, obj);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static bool RegistryHasAutostart()
        {
            // Check to see the current state (running at startup or not)
            if (rkApp.GetValue(APP_NAME) == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                return false;
            }
            else
            {
                if (!rkApp.GetValue(APP_NAME).Equals(APP_LOCATION))
                {
                    rkApp.DeleteValue(APP_NAME, false);
                    return false;
                }

                // The value exists, the application is set to run at startup
                return true;
            }
        }

        public static void RegistrySetAutostart(bool isChecked)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue(APP_NAME, APP_LOCATION);
            }
            else
            {
                registryKey.DeleteValue(APP_NAME);
            }
        }
        public static object StringToObject(string base64String)
        {
            byte[] bytes = Convert.FromBase64String(base64String);
            using (MemoryStream ms = new MemoryStream(bytes, 0, bytes.Length))
            {
                ms.Write(bytes, 0, bytes.Length);
                ms.Position = 0;
                return new BinaryFormatter().Deserialize(ms);
            }
        }

        internal static int ParseAsInt(string text, int minValue)
        {
            var tmp = Regex.Replace(text, "[^0-9]", "");
            if (tmp != null && tmp.Length > 0)
            {
                int outInt = minValue;
                Int32.TryParse(tmp, out outInt);

                if (outInt >= minValue)
                    return outInt;
            }
            return minValue;
        }

        internal static void RemoveFromRegistry()
        {
            if (rkApp.GetValue(APP_NAME) != null)
            {
                Console.Write("Removed app from Registry.");
                rkApp.DeleteValue(APP_NAME, false);
            }
        }
    }
}