using SharpVectors.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TechtonicaModLoader.MyWindows;
using TechtonicaModLoader.MyWindows.GetWindows;

namespace TechtonicaModLoader
{
    public static class GuiUtils
    {
        private static Dictionary<string, LoadingWindow> downloadingWindows = new Dictionary<string, LoadingWindow>();

        public static void OpenURL(string url) {
            if (GetUserConfirmation("Open Link?", $"Do you want to open this link?\n{url}")) {
                ProcessStartInfo info = new ProcessStartInfo() {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(info);
            }
        }

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

        public static void ShowDownloadingGui(string modName) {
            LoadingWindow window = new LoadingWindow();
            window.SetProgress($"Downloading {modName}", 0, 1);
            downloadingWindows.Add(modName, window);
            window.Show();
        }

        public static void ShowInstallingGui(string modName) {
            if (downloadingWindows.ContainsKey(modName)) {
                downloadingWindows[modName].SetProgress($"Installing {modName}", 0 , 1);
            }
            else {
                string error = $"ShowInstallGui() called for modName that is not in dictionary: {modName}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static void HideDownloadingGui(string modName) {
            if(downloadingWindows.ContainsKey(modName)) {
                downloadingWindows[modName].Close();
                downloadingWindows.Remove(modName);
            }
            else {
                string error = $"HideDownloadingGui() called for modName that is not in dictionary: {modName}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }
        }
    }

    public static class FileStructureUtils 
    {
        public static bool ValidateGameFolder() {
            return File.Exists($"{ProgramData.FilePaths.gameFolder}\\Techtonica.exe");
        }

        public static void StartGame() {
            if (!ValidateGameFolder()) {
                Log.Error($"Cannot start game, gameFolder is invalid");
                return;
            }

            string bepinexConfigPath = $"{ProgramData.FilePaths.bepInExConfigFolder}/BepInEx.cfg";
            if (!File.Exists(bepinexConfigPath)) {
                Log.Error("Could not find BepInEx config file.");
                return;
            }

            string text = File.ReadAllText(bepinexConfigPath);
            text = text.Replace("HideManagerGameObject = false", "HideManagerGameObject = true");
            File.WriteAllText(bepinexConfigPath, text);

            string[] configFiles = Directory.GetFiles(ProgramData.FilePaths.configsFolder);
            foreach(string configFile in configFiles) {
                string newPath = configFile.Replace(ProgramData.FilePaths.configsFolder, ProgramData.FilePaths.bepInExConfigFolder);
                if (File.Exists(newPath)) File.Delete(newPath);
                File.Copy(configFile, newPath);
            }

            Process.Start($"{Settings.userSettings.gameFolder.value}\\Techtonica.exe");
        }

        public static void ClearUnzipFolder() {
            Log.Debug($"Clearing Unzip folder");
            DeleteFolder(ProgramData.FilePaths.unzipFolder);
            Directory.CreateDirectory(ProgramData.FilePaths.unzipFolder);
        }

        public static bool SearchForConfigFile(string folder, out string configFile) {
            Log.Debug($"Searching folder '{folder}' for config file");
            configFile = "Not Found";

            string[] files = Directory.GetFiles(folder);
            foreach (string file in files) {
                if (file.EndsWith(".cfg")) {
                    Log.Debug($"Found config file at '{file}'");
                    configFile = file;
                    return true;
                }
            }

            string[] directories = Directory.GetDirectories(folder);
            foreach (string directory in directories) {
                if (SearchForConfigFile(directory, out configFile)) {
                    return true;
                }
            }

            Log.Debug($"Did not find config file");
            return false;
        }

        public static bool SearchForMarkdownFile(string folder, out string markdownFile) {
            Log.Debug($"Searching folder '{folder}' for markdown file");
            markdownFile = "Not Found";

            string[] files = Directory.GetFiles(folder);
            foreach (string file in files) {
                if (file.EndsWith(".md")) {
                    Log.Debug($"Found markdown file at '{file}'");
                    markdownFile = file;
                    return true;
                }
            }

            string[] directories = Directory.GetDirectories(folder);
            foreach (string directory in directories) {
                if (SearchForConfigFile(directory, out markdownFile)) {
                    return true;
                }
            }

            Log.Debug($"Did not find markdown file");
            return false;
        }

        public static List<string> SearchForDllFiles(string folder) {
            Log.Debug($"Searching folder '{folder}' for .dll files");
            List<string> dllFiles = new List<string>();

            string[] files = Directory.GetFiles(folder);
            foreach (string file in files) {
                if (file.EndsWith(".dll")) {
                    dllFiles.Add(file);
                    Log.Debug($"Found .dll file '{file}'");
                }
            }

            string[] directories = Directory.GetDirectories(folder);
            foreach (string directory in directories) {
                dllFiles.AddRange(SearchForDllFiles(directory));
            }

            return dllFiles;
        }

        public static List<string> SearchForXmlFiles(string folder) {
            Log.Debug($"Searching folder '{folder}' for .xml files");

            List<string> xmlFiles = Directory.GetFiles(folder).Where(
                file => file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)
            ).ToList();

            foreach(string directory in Directory.GetDirectories(folder)) {
                xmlFiles.AddRange(SearchForXmlFiles(directory));
            }

            return xmlFiles;
        }

        public static List<string> CopyFolder(string source, string destination) {
            Log.Debug($"Copying folder from '{source}' to '{destination}'");
            string[] files = Directory.GetFiles(source);
            string[] folders = Directory.GetDirectories(source);

            List<string> copiedFiles = new List<string>();

            foreach (string file in files) {
                string newPath = file.Replace(source, destination);
                if (File.Exists(newPath)) {
                    File.Delete(newPath);
                }

                File.Copy(file, newPath);
                copiedFiles.Add(newPath);
            }

            foreach (string folder in folders) {
                string newPath = folder.Replace(source, destination);
                Directory.CreateDirectory(newPath);
                List<string> copiedSubFiles = CopyFolder(folder, newPath);
                copiedFiles.AddRange(copiedSubFiles);
            }

            copiedFiles = copiedFiles.Distinct().ToList();
            return copiedFiles;
        }

        public static void DeleteFolder(string folder) {
            Log.Debug($"Deleting folder '{folder}'");
            string[] files = Directory.GetFiles(folder);
            string[] subFolders = Directory.GetDirectories(folder);

            foreach (string file in files) {
                File.Delete(file);
            }

            foreach (string subFolder in subFolders) {
                DeleteFolder(subFolder);
            }

            Directory.Delete(folder);
        }
    }

    public static partial class StringUtils
    {
        public static string ColourToHex(Color color) {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }

    public static class DebugUtils 
    {
        public static void CrashIfDebug(string error) {
            if (!ProgramData.isDebugBuild) return;
            throw new Exception(error);
        }
    }
}
