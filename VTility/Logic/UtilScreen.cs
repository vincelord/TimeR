using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;

namespace VTility.Logic
{
    internal class UtilScreen
    {
        /* Window Operations */

        public static Screen FindScreenOf(Window window)
        {
            return Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);
        }

        public static List<Screen> FindAllScreens()
        {
            Console.WriteLine(">>>-------------------------------------------------->>>");
            List<Screen> allScreens = new List<Screen>();
            foreach (var screen in Screen.AllScreens)
            {
                // For each screen, add the screen properties to a list box.
                Console.WriteLine("Device Name: " + screen.DeviceName);
                Console.WriteLine("Bounds: " + screen.Bounds.ToString());
                Console.WriteLine("Type: " + screen.GetType().ToString());
                Console.WriteLine("Working Area: " + screen.WorkingArea.ToString());
                Console.WriteLine("Primary Screen: " + screen.Primary.ToString());
                allScreens.Add(screen);
                Console.WriteLine("<<<--------------------------------------------------<<<");
            }

            return allScreens;
        }

        public static Tuple<Screen, Dimensions> FindScreenSmallest()
        {
            Screen smallest = Screen.PrimaryScreen;
            int resX = 0;
            int resY = 0;

            var allScreens = FindAllScreens();
            foreach (var screen in allScreens)
            {
                var condX = (resX == 0 || screen.Bounds.Width < resX);
                var condY = (resY == 0 || screen.Bounds.Height < resY);

                if (condX && condY)
                {
                    resX = screen.Bounds.Width;
                    resY = screen.Bounds.Height;
                    smallest = screen;
                }
            }
            Console.WriteLine("The smallest of the " + allScreens.Count + " monitors is " + smallest + ".");
            return new Tuple<Screen, Dimensions>(smallest, new Dimensions(resX, resY));
        }

        /* Taskbar Stuff */

        /// <summary>
        /// Returns the height of a single Taskbar on that Screen
        /// </summary>
        public static int GetTaskBarHeightOf(Screen thisScreen)
        {
            int bHeight = thisScreen.Bounds.Height;
            int waHeight = thisScreen.WorkingArea.Height;
            var result = (bHeight - waHeight) / 2;

            Console.WriteLine("The single taskbar height of " + thisScreen.DeviceName + " is " + result);
            return result;
        }

        /// <summary>
        /// Returns the width of a single Taskbar on that Screen
        /// </summary>
        public static int GetTaskBarWidthOf(Screen thisScreen)
        {
            int bWidth = thisScreen.Bounds.Width;
            int waWidth = thisScreen.WorkingArea.Width;
            return (bWidth - waWidth) / 2;
        }

        /// <summary>
        /// Returns the Dimensions (width and height) of a single Taskbar on that Screen
        /// </summary>
        public static Dimensions GetTaskBarDimensions(Screen thisScreen)
        {
            return new Dimensions(GetTaskBarWidthOf(thisScreen), GetTaskBarHeightOf(thisScreen));
        }
    }
}