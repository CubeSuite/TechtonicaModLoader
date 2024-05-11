using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MyClasses.Globals
{
    // https://stackoverflow.com/questions/444430/how-do-i-focus-a-foreign-window

    public static class WindowSwitcher
    {
        // Objects & Variables
        public static bool gaveFocusSinceLaunch = false;
        private const int SW_SHOWNORMAL = 1;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetForegroundWindow(IntPtr hwnd);

        // Public Functions

        public static void FocusGame() {
            Process[] processes = Process.GetProcessesByName("Techtonica");

            foreach (Process p in processes) {
                ShowWindow(p.MainWindowHandle, SW_SHOWNORMAL);
                SetForegroundWindow(p.MainWindowHandle);
            }

            gaveFocusSinceLaunch = true;
        }
    }
}
