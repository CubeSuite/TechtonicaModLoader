using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TechtonicaModLoader.MyWindows;
using TechtonicaModLoader.MyWindows.GetWindows;

namespace TechtonicaModLoader
{
    public static class GuiUtils
    {
        public static bool GetUserConfirmation(string title, string description) {
            return GetYesNoWindow.GetYesNo(title, description);
        }

        public static string GetStringFromUser(string title, string hint) {
            return GetStringWindow.GetString(title, hint);
        }

        public static int GetIntFromUser(string title, int min, int max, int? defaultValue = null) {
            return GetIntWindow.GetInt(title, min, max, defaultValue);
        }

        public static void ShowInfoMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.ShowInfo(title, description, closeButtonText);
        }

        public static void ShowWarningMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.ShowWarning(title, description, closeButtonText);
        }

        public static void ShowErrorMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.ShowError(title, description, closeButtonText);
        }
    }

    public static partial class StringUtils
    {
        public static string ColourToHex(Color color) {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
