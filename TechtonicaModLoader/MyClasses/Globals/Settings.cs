using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.PerformanceData;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader
{
    public class Settings
    {
        // How To Add A Setting:
        // 1. Give it a unique name in SettingNames
        // 2. Create it in the region below
        //   2.1 Assign a category like this: General/Examples 
        // 3. Add it to GetAllSettings()
        // 4. Add it to the relevent SetSetting() function

        // Objects & Variables
        public static Settings userSettings = new Settings();
        public const string defaultCategory = "General";

        private static string iconColour = "#FFFFFF";

        #region Settings

        // General
        public BoolSetting logDebugMessages = new BoolSetting(false) {
            name = SettingNames.logDebugMessages,
            description = "Only enable if you're gathering information for a bug report.",
            category = "General",
            OnValueChanged = (bool newValue) => {
                Log.logDebugToFile = newValue;
                Log.Debug($"Set Log.logDebugToFile to '{newValue}'");
            }
        };
        public ButtonSetting showLog = new ButtonSetting() {
            name = SettingNames.showLog,
            description = $"Opens the folder that contains {ProgramData.programName}'s log file.",
            category = "General",
            buttonText = "Show In Explorer",
            OnClick = delegate () { Process.Start(ProgramData.FilePaths.logsFolder); }
        };

        // Game Folder
        public StringSetting gameFolder = new StringSetting() {
            name = SettingNames.gameFolder,
            description = "Techtonica's installation location.",
            category = "Game Folder",
            defaultValue = ""
        };
        public ButtonSetting findGameFolder = new ButtonSetting() {
            name = SettingNames.findGameFolder,
            description = "Have TML search for your Techtonica installation folder.",
            category = "Game Folder",
            buttonText = "Find",
            OnClick = delegate () {
                string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                foreach (char drive in letters) {
                    string steamPath = $"{drive}:/steam/steamapps/common/Techtonica/Techtonica.exe";
                    string steamPath2 = $"{drive}:/ steam/steamapps/common/Techtonica/Techtonica.exe";
                    string steamPath3 = $"{drive}:/SteamLibrary/steamapps/common/Techtonica/Techtonica.exe";
                    string xboxPath = $"{drive}:/XBoxGames/Techtonica/Content/Techtonica.exe";

                    if (File.Exists(xboxPath)) {
                        userSettings.SetSetting(SettingNames.gameFolder, Path.GetDirectoryName(xboxPath), false);
                        Log.Debug($"Found game folder at '{userSettings.gameFolder.name}'");
                        return;
                    }
                    else if (File.Exists(steamPath)) {
                        userSettings.SetSetting(SettingNames.gameFolder, Path.GetDirectoryName(steamPath), false);
                        Log.Debug($"Found game folder at '{userSettings.gameFolder.name}'");
                        return;
                    }
                    else if (File.Exists(steamPath2)) {
                        userSettings.SetSetting(SettingNames.gameFolder, Path.GetDirectoryName(steamPath2), false);
                        Log.Debug($"Found game folder at '{userSettings.gameFolder.name}'");
                        return;
                    }
                    else if (File.Exists(steamPath3)) {
                        userSettings.SetSetting(SettingNames.gameFolder, Path.GetDirectoryName(steamPath3), false);
                        Log.Debug($"Found game folder at '{userSettings.gameFolder.name}'");
                        return;
                    }
                }

                Log.Warning($"Could not find game folder");
                GuiUtils.ShowWarningMessage("Game Folder Not Found", "TML couldn't find your Techtonica installation folder. Please browse to it manually.");
            }
        };
        public ButtonSetting browseForGameFolder = new ButtonSetting() {
            name = SettingNames.browseForGameFolder,
            description = "Manually browse for Techtonica's installation folder.",
            category = "Game Folder",
            buttonText = "Browse",
            OnClick = delegate () {
                System.Windows.Forms.OpenFileDialog browser = new System.Windows.Forms.OpenFileDialog {
                    Filter = ("Techtonica.exe|*.exe")
                };
                if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    if (browser.FileName.EndsWith("Techtonica.exe")) {
                        userSettings.SetSetting(SettingNames.gameFolder, Path.GetDirectoryName(browser.FileName), false);
                    }
                    else {
                        GuiUtils.ShowErrorMessage("Wrong File Selected", "You need to select the file 'Techtonica.exe'.");
                    }
                }
            }
        };

        // Mod Lists
        public ComboSetting defaultSortOption = new ComboSetting() {
            name = SettingNames.defaultSortOption,
            description = "The default sort option to apply to mod lists.",
            category = "Mod Lists",
            defaultValue = StringUtils.GetModListSortOptionName(ModListSortOption.LastUpdated),
            values = StringUtils.GetAllModListSortOptionNamesForCombo()
        };
        public ComboSetting defaultModList = new ComboSetting() {
            name = SettingNames.defaultModList,
            description = "The mod list that is displayed when you open Techtonica Mod Loader",
            category = "Mod Lists",
            defaultValue = StringUtils.GetModListSourceName(ModListSource.NewMods),
            values = StringUtils.GetAllModListSourceNamesForCombo()
        };

        // Backups
        public StringSetting backupsFolder = new StringSetting() {
            name = SettingNames.backupsFolder,
            description = "Where data backups should be stored. A OneDrive location is recommended for important data.",
            category = "Backups",
            defaultValue = BackupManager.defaultBackupsFolder
        };
        public ButtonSetting browseForBackupsFolder = new ButtonSetting() {
            name = SettingNames.browseForBackupsFolder,
            description = "Browse for a folder to store backups in.",
            category = "Backups",
            buttonText = "Browse",
            OnClick = delegate () {
                System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();
                if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    userSettings.SetSetting(SettingNames.backupsFolder, browser.SelectedPath, false);
                }
            }
        };
        public IntSetting numBackups = new IntSetting() {
            name = SettingNames.numBackups,
            description = "The maximum number of data backups to keep.",
            category = "Backups",
            min = 1,
            max = 10,
            defaultValue = 5
        };
        public ButtonSetting openBackups = new ButtonSetting() {
            name = SettingNames.openBackups,
            description = "Opens the Data Backup Management Window.",
            category = "Backups",
            buttonText = "Open Backup Manager",
            OnClick = delegate () { BackupManagementWindow.ShowBackupManagerWindow(); }
        };

        // Theme
        public ButtonSetting restoreDefaultTheme = new ButtonSetting() {
            name = SettingNames.restoreDefaultTheme,
            description = "Resets the theme to the default colours.",
            category = "Theme",
            buttonText = "Restore Default Theme",
            OnClick = delegate () {
                if (GuiUtils.GetUserConfirmation("Restore Default Theme?", "Are you sure you want to restore the default theme? This cannot be undone.")) {
                    userSettings.dimBackground.RestoreDefault();
                    userSettings.normalBackground.RestoreDefault();
                    userSettings.brightBackground.RestoreDefault();
                    userSettings.uiBackground.RestoreDefault();
                    userSettings.accentColour.RestoreDefault();
                    userSettings.textColour.RestoreDefault();
                    Save();
                    SettingsChanged?.Invoke(null, new SettingChangedEventArgs() { changeFromGUI = false });
                    LoadTheme();
                }
            }
        };
        public ColourSetting dimBackground = new ColourSetting() {
            name = SettingNames.dimBackground,
            description = $"The background colour of {ProgramData.programName}.",
            category = "Theme",
            defaultValue = Color.FromRgb(22, 22, 38)
        };
        public ColourSetting normalBackground = new ColourSetting() {
            name = SettingNames.normalBackground,
            description = "Should be slightly brighter than 'Dim Background'.",
            category = "Theme",
            defaultValue = Color.FromRgb(48, 48, 64)
        };
        public ColourSetting brightBackground = new ColourSetting() {
            name = SettingNames.brightBackground,
            description = "Should be slightly brighter than 'Normal Background'.",
            category = "Theme",
            defaultValue = Color.FromRgb(58, 58, 88)
        };
        public ColourSetting uiBackground = new ColourSetting() {
            name = SettingNames.uiBackground,
            description = "Should be slightly brighter than 'Bright Background'.",
            category = "Theme",
            defaultValue = Color.FromRgb(68, 68, 98)
        };
        public ColourSetting accentColour = new ColourSetting() {
            name = SettingNames.accentColour,
            description = $"The accent colour of {ProgramData.programName}.",
            category = "Theme",
            defaultValue = Colors.Orange
        };
        public ColourSetting textColour = new ColourSetting() {
            name = SettingNames.textColour,
            description = $"The text and icon colour of {ProgramData.programName}.",
            category = "Theme",
            defaultValue = Colors.White
        };

        // Hidden - Put in default category
        public BoolSetting isFirstTimeLaunch = new BoolSetting(true) {
            name = SettingNames.isFirstTimeLaunch,
            description = "Only true the first time the program is run.",
            category = defaultCategory,
            isHidden = true
        };
        public StringSetting lastProfile = new StringSetting() {
            name = SettingNames.lastProfile,
            description = "User's last selected profile",
            category = "General",
            defaultValue = "Modded",
            isHidden = true
        };
        public StringSetting seenMods = new StringSetting() {
            name = SettingNames.seenMods,
            description = "Mods seen by the user",
            category = defaultCategory,
            defaultValue = "",
            isHidden = true
        };

        #endregion

        // Constructor

        public Settings() {
            RestoreDefaults(false);
        }

        // Custom Events

        public static event EventHandler<SettingChangedEventArgs> SettingsChanged;

        // Public Functions

        public List<Setting> GetAllSettings() {
            return new List<Setting>() {
                // General
                logDebugMessages,
                showLog,

                // Game Folder
                gameFolder,
                findGameFolder,
                browseForGameFolder,

                // Mod Lists
                defaultSortOption,
                defaultModList,

                // Backups
                backupsFolder,
                browseForBackupsFolder,
                numBackups,
                openBackups,

                // Theme
                restoreDefaultTheme,
                dimBackground,
                normalBackground,
                brightBackground,
                uiBackground,
                accentColour,
                textColour,

                // Hidden
                isFirstTimeLaunch,
                lastProfile,
                seenMods
            };
        }

        public List<Setting> GetSettingsInCategory(string category) {
            return GetAllSettings().Where(setting => setting.category.Contains(category)).ToList();
        }

        public void RestoreDefaults(bool shouldSave = true) {
            foreach (Setting setting in GetAllSettings()) {
                setting.RestoreDefault();
            }

            if (shouldSave) Save();
        }

        public void SetSetting(string name, string value, bool changeFromGUI = true) {
            switch (name) {
                case SettingNames.backupsFolder: backupsFolder.value = value; break;
                case SettingNames.gameFolder: gameFolder.value = value; break;
                case SettingNames.defaultSortOption: defaultSortOption.value = value; break;
                case SettingNames.defaultModList: defaultModList.value = value; break;
                case SettingNames.lastProfile: lastProfile.value = value; break;
                case SettingNames.seenMods: seenMods.value = value; break;

                default:
                    Log.Error($"Could not find the StringSetting named '{name}'");
                    return;
            }

            Log.Info($"Set Setting '{name}' to '{value}'");
            Save();
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs() { changeFromGUI = changeFromGUI });
        }

        public void SetSetting(string name, int value, bool changeFromGUI = true) {
            switch (name) {
                case SettingNames.numBackups: numBackups.value = value; break;
                default:
                    Log.Error($"Could not find the IntSetting named '{name}'");
                    return;
            }

            Log.Info($"Set Setting '{name}' to '{value}'");
            Save();
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs() { changeFromGUI = changeFromGUI });
        }

        public void SetSetting(string name, bool value, bool changeFromGUI = true) {
            switch (name) {
                case SettingNames.logDebugMessages: logDebugMessages.value = value; break;
                case SettingNames.isFirstTimeLaunch: isFirstTimeLaunch.value = value; break;
                
                default:
                    Log.Error($"Could not find the BoolSetting named '{name}'");
                    return;
            }

            Log.Info($"Set Setting '{name}' to '{value}'");
            Save();
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs() { changeFromGUI = changeFromGUI });
        }

        public void SetSetting(string name, Color value, bool changeFromGUI = true) {
            switch (name) {
                case SettingNames.dimBackground: dimBackground.value = value; break;
                case SettingNames.normalBackground: normalBackground.value = value; break;
                case SettingNames.brightBackground: brightBackground.value = value; break;
                case SettingNames.uiBackground: uiBackground.value = value; break;
                case SettingNames.accentColour: accentColour.value = value; break;
                case SettingNames.textColour: textColour.value = value; break;
                default:
                    Log.Error($"Could not find the ColourSetting named '{name}'");
                    return;
            }

            Log.Info($"Set Setting '{name}' to '{value}'");
            Save();
            LoadTheme();
            SettingsChanged?.Invoke(this, new SettingChangedEventArgs() { changeFromGUI = changeFromGUI });
        }

        public static void LoadTheme() {
            Application.Current.Resources["dimBackgroundBrush"] = new SolidColorBrush(userSettings.dimBackground.value);
            Application.Current.Resources["backgroundBrush"] = new SolidColorBrush(userSettings.normalBackground.value);
            Application.Current.Resources["brightBackgroundBrush"] = new SolidColorBrush(userSettings.brightBackground.value);
            Application.Current.Resources["uiBackgroundBrush"] = new SolidColorBrush(userSettings.uiBackground.value);
            Application.Current.Resources["accentBrush"] = new SolidColorBrush(userSettings.accentColour.value);
            Application.Current.Resources["textBrush"] = new SolidColorBrush(userSettings.textColour.value);

            Application.Current.Resources["accentColour"] = userSettings.accentColour.value;
            RecolourIcons();
        }

        // Private Functions

        private bool ValidateNames() {
            List<string> names = GetAllSettings().Select(setting => setting.name).ToList();
            List<string> uniqueNames = names.Distinct().ToList();
            bool passed = names.Count == uniqueNames.Count;
            if (!passed) {
                Log.Warning("Setting names are not unique.");
                Log.Debug($"Names: {string.Join("|", names)}");
            }

            return passed;
        }

        private static void RecolourIcons() {
            string newColourHex = StringUtils.ColourToHex(userSettings.textColour.value);
            if (newColourHex == iconColour) return;

            List<string> svgsToRecolour = new List<string>() {
                "GUI/Up",
                "GUI/Down",
                "ControlBox/Settings",
                "ControlBox/Move",
                "ControlBox/Minimise",
                "ControlBox/Close",
            };
            foreach (string svg in svgsToRecolour) {
                Log.Debug($"Updating {svg} to {newColourHex}");
                string path = $"{ProgramData.FilePaths.resourcesFolder}\\{svg}.svg";

                string OldText = File.ReadAllText(path);
                Log.Debug($"Read svg file");

                string NewText = OldText.Replace(iconColour.ToLower(), newColourHex.ToLower());

                File.WriteAllText(path, NewText);
                Log.Debug($"Wrote svg file");
            }

            iconColour = StringUtils.ColourToHex(userSettings.textColour.value);
            MainWindow.current.controlBox.RefreshIcons();
        }

        // Data Functions

        public static void Save() {
            userSettings.ValidateNames();
            string json = JsonConvert.SerializeObject(userSettings, Formatting.Indented);
            File.WriteAllText(ProgramData.FilePaths.settingsFile, json);
        }

        public static void Load() {
            if (!File.Exists(ProgramData.FilePaths.settingsFile)) {
                userSettings = new Settings();
                userSettings.RestoreDefaults();
                Save();
                return;
            }

            string json = File.ReadAllText(ProgramData.FilePaths.settingsFile);
            userSettings = JsonConvert.DeserializeObject<Settings>(json);
            iconColour = StringUtils.ColourToHex(userSettings.textColour.value);
            LoadTheme();
        }
    }

    public static class SettingNames
    {
        // General
        public const string logDebugMessages = "Log Debug Messages";
        public const string showLog = "Show Log In Explorer";
        public const string renderImages = "Render Images";
        public const string cacheImages = "Cache Images";
        public const string clearCache = "Clear Image Cache";

        // Game Folder
        public const string gameFolder = "Game Folder";
        public const string findGameFolder = "Find Game Folder";
        public const string browseForGameFolder = "Browse For Game Folder";

        // Mod Lists
        public const string defaultSortOption = "Default Sort Option";
        public const string defaultModList = "Default Mod List";

        // Backups
        public const string backupsFolder = "Backups Folder";
        public const string browseForBackupsFolder = "Browse For Backups Folder";
        public const string numBackups = "Backup Count";
        public const string openBackups = "Backups Manager";

        // Theme
        public const string restoreDefaultTheme = "Restore Default Theme";
        public const string dimBackground = "Dim Background Colour";
        public const string normalBackground = "Normal Background Colour";
        public const string brightBackground = "Bright Background Colour";
        public const string uiBackground = "UI Background Colour";
        public const string accentColour = "Accent Colour";
        public const string textColour = "Text Colour";
        
        // Hidden
        public const string isFirstTimeLaunch = "Is First Time Launch";
        public const string lastProfile = "Last Profile";
        public const string seenMods = "Seen Mods";
    }

    public class Setting
    {
        public readonly string type = "None";
        public string name;
        public string description;
        public string category;

        [JsonIgnore]
        public bool isHidden = false;

        public virtual void RestoreDefault() { }

        public Setting() { }
        public Setting(string _type) {
            type = _type;
        }
    }

    public class StringSetting : Setting
    {
        public string value;
        public string defaultValue;

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public StringSetting() : base("String") {}
    }

    public class ComboSetting : Setting
    {
        public string value;
        public string defaultValue;
        public string values;

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public ComboSetting() : base("Combo") { }
    }

    public class IntSetting : Setting
    {
        public int min;
        public int max;
        public int value;
        public int defaultValue;

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public IntSetting() : base("Int") { }
    }

    public class BoolSetting : Setting
    {
        private bool _value;
        public bool value {
            get => _value;
            set {
                _value = value;
                OnValueChanged(value);
            }
        }

        public bool defaultValue;

        [JsonIgnore]
        public Action<bool> OnValueChanged = (bool newValue) => {};

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public BoolSetting(bool _defaultValue) : base("Bool") { defaultValue = _defaultValue; }
    }

    public class ButtonSetting : Setting
    {
        public string buttonText;

        [JsonIgnore]
        public Action OnClick = delegate () { };

        public ButtonSetting() : base("Button") { }
    }

    public class ColourSetting : Setting
    {
        public Color value;
        public Color defaultValue;

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public ColourSetting() : base("Colour") { }
    }

    public class SettingChangedEventArgs : EventArgs
    {
        public bool changeFromGUI;
    }
}
