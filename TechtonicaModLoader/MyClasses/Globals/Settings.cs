using MyLogger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
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
        public BoolSetting logDebugMessages = new BoolSetting(SettingNames.logDebugMessages, "Only enable if you're gathering information for a bug report.", "General", false);

        // Backups
        public StringSetting backupsFolder = new StringSetting(SettingNames.backupsFolder, "Where data backups should be stored. A OneDrive location is recommended for important data.", "Backups", BackupManager.defaultBackupsFolder);
        public ButtonSetting browseForBackupsFolder = new ButtonSetting(SettingNames.browseForBackupsFolder, "Browse for a folder to store backups in.", "Backups", "Browse") {
            OnClick = delegate () {
                System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();
                if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    userSettings.SetSetting(SettingNames.backupsFolder, browser.SelectedPath, false);
                }
            }
        };
        public IntSetting numBackups = new IntSetting(SettingNames.numBackups, "The maximum number of data backups to keep.", "Backups", 1, 10, 5);
        public ButtonSetting openBackups = new ButtonSetting(SettingNames.openBackups, "Opens the Data Backup Management Window.", "Backups", "Open Backup Manager") {
            OnClick = delegate () { BackupManagementWindow.ShowBackupManagerWindow(); }
        };

        // Theme
        public ButtonSetting restoreDefaultTheme = new ButtonSetting(SettingNames.restoreDefaultTheme, "Resets the theme to the default colours.", "Theme", "Restore Default Theme") {
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
        public ColourSetting dimBackground = new ColourSetting(SettingNames.dimBackground, $"The background colour of {ProgramData.programName}.", "Theme", Color.FromRgb(22, 22, 38));
        public ColourSetting normalBackground = new ColourSetting(SettingNames.normalBackground, "Should be slightly brighter than 'Dim Background'.", "Theme", Color.FromRgb(48, 48, 64));
        public ColourSetting brightBackground = new ColourSetting(SettingNames.brightBackground, "Should be slightly brighter than 'Normal Background'.", "Theme", Color.FromRgb(58, 58, 88));
        public ColourSetting uiBackground = new ColourSetting(SettingNames.uiBackground, "Should be slightly brighter than 'Bright Background'.", "Theme", Color.FromRgb(68, 68, 98));
        public ColourSetting accentColour = new ColourSetting(SettingNames.accentColour, $"The accent colour of {ProgramData.programName}.", "Theme", Colors.Orange);
        public ColourSetting textColour = new ColourSetting(SettingNames.textColour, $"The text and icon colour of {ProgramData.programName}.", "Theme", Colors.White);

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
                Save();
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
    }

    public class Setting
    {
        public readonly string type = "None";
        public string name;
        public string description;
        public string category;

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

        public StringSetting() : base("String") { }
        public StringSetting(string _name, string _description, string _category, string _defaultValue) : base("String") {
            name = _name;
            description = _description;
            category = _category;
            defaultValue = _defaultValue;
        }
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
        public IntSetting(string _name, string _description, string _category, int _min, int _max, int _defaultValue) : base("Int") {
            name = _name;
            description = _description;
            category = _category;
            min = _min;
            max = _max;
            defaultValue = _defaultValue;
        }
    }

    public class BoolSetting : Setting
    {
        public bool value;
        public bool defaultValue;

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public BoolSetting() : base("Bool") { }
        public BoolSetting(string _name, string _description, string _category, bool _defaultValue) : base("Bool") {
            name = _name;
            description = _description;
            category = _category;
            defaultValue = _defaultValue;
        }
    }

    public class ButtonSetting : Setting
    {
        public string buttonText;

        [JsonIgnore]
        public Action OnClick = delegate () { };

        public ButtonSetting() : base("Button") { }
        public ButtonSetting(string _name, string _description, string _category, string _buttonText) : base("Button") {
            name = _name;
            description = _description;
            category = _category;
            buttonText = _buttonText;
        }
    }

    public class ColourSetting : Setting
    {
        public Color value;
        public Color defaultValue;

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public ColourSetting() : base("Colour") { }
        public ColourSetting(string _name, string _description, string _category, Color _defaultValue) : base("Colour") {
            name = _name;
            description = _description;
            category = _category;
            value = _defaultValue;
            defaultValue = _defaultValue;
        }
    }

    public class SettingChangedEventArgs : EventArgs
    {
        public bool changeFromGUI;
    }
}
