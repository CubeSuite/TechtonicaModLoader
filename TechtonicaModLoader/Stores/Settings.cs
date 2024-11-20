using Accessibility;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TechtonicaModLoader.Services;

namespace TechtonicaModLoader.Stores
{
    public class Settings
    {
        #region Settings

        // To add a new setting:
        // 1 - Create _myNewSetting = new Setting<type>() below
        //   1.1 - For int, float, double, use ComparableSetting<type>()
        //
        // 2 - Add _userSettings._myNewSetting to VisibleSettings
        //   2.1 - If it's a hidden setting, skip this step
        //
        // 3 - Add a public getter property MyNewSetting => _myNewSettingic Setting<bool> LogDebugMessages => _logDebugMessages;
        //
        // 4 - If it's an enum setting or a new type that's not yet handled, you'll need to:
        //   4.1 - Add a member to SettingViewModel
        //   4.2 - Make a new constructor in SettingViewModel
        //   4.3 - Add an if statement to SettingViewModel.OnValueChanged()
        //   4.4 - Add an if statement to SettingViewModel.SettingTemplateSelector.SelectTemplate()
        //   4.4 - Add an if statement to SettingsWindowViewModel.PopulateSettingsToShow()

        // General

        public Setting<bool> LogDebugMessages => _logDebugMessages;
        private Setting<bool> _logDebugMessages = new Setting<bool>(
            "Log Debug Messages",
            "Whether Debug messages should be logged to file.",
            "General",
            false
        );

        [JsonIgnore]
        public ButtonSetting ShowLogInExplorer => _showLogInExplorer;
        private ButtonSetting _showLogInExplorer = new ButtonSetting(
            "Show Log In Explorer",
            "Opens the folder that contains Techtonica Mod Loader's log file.",
            "General",
            "Show In Explorer",
            delegate () {
                Process.Start(new ProcessStartInfo() {
                    FileName = ProgramData.FilePaths.logsFolder,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
        );

        // Game Folder

        public Setting<string> GameFolder => _gameFolder;
        private Setting<string> _gameFolder = new Setting<string>(
            "Game Folder",
            "Techtonica's installation location",
            "Game Folder",
            ""
        );

        [JsonIgnore]
        public ButtonSetting FindGameFolder => _findGameFolder;
        private ButtonSetting _findGameFolder = new ButtonSetting(
            "Find Game Folder",
            "Have TML search for your Techtonica installation folder.",
            "Game Folder",
            "Find",
            delegate () {
                // ToDo: Find game folder
            }
        );

        [JsonIgnore]
        public ButtonSetting BrowseForGameFolder => _browseForGameFolder;
        private ButtonSetting _browseForGameFolder = new ButtonSetting(
            "Browse For Game Folder",
            "Manually browse for Techtonica's installation folder.",
            "Game Folder",
            "Browse",
            delegate () {
                OpenFileDialog browser = new OpenFileDialog { Filter = ("Techtonica.exe|*.exe") };
                if (browser.ShowDialog() == true) {
                    if (browser.FileName.EndsWith("Techtonica.exe")) {
                        UserSettings.GameFolder.Value = Path.GetDirectoryName(browser.FileName) ?? "";
                        UserSettings.SettingsUpdatedExternally?.Invoke();
                    }
                    else {
                        UserSettings._dialogService.ShowErrorMessage("Wrong File Selected", "You need to select the file 'Techtonica.exe'");
                    }
                }
            }
        );

        // Mod Lists

        public EnumSetting<ModListSortOption> DefaultModListSortOption => _defaultModListSortOption;
        private EnumSetting<ModListSortOption> _defaultModListSortOption = new EnumSetting<ModListSortOption>(
            "Default Sort Option",
            "The default sort option to apply to mod lists.",
            "Mod Lists",
            ModListSortOption.Alphabetical
        );

        public EnumSetting<ModListSource> DefaultModList => _defaultModList;
        private EnumSetting<ModListSource> _defaultModList = new EnumSetting<ModListSource>(
            "Default Mod List",
            "The mod list that is displayed when you open Techtonica Mod Loader",
            "Mod Lists",
            ModListSource.New
        );

        // Hidden

        public ComparableSetting<int> ActiveProfileID => _activeProfileID;
        private ComparableSetting<int> _activeProfileID = new ComparableSetting<int>(
            "Active Profile ID",
            "The ID of the user's current profile",
            "General",
            0,
            0,
            int.MaxValue
        );

        public Setting<List<string>> SeenMods => _seenMods;
        private Setting<List<string>> _seenMods = new Setting<List<string>>(
            "Seen Mods",
            "Mods that have appeared in TML",
            "General",
            new List<string>()
        );

        #endregion

        // Members

        private static Settings? _userSettings = null;
        private static bool _loaded = false;
        private DialogService _dialogService;

        // Events

        #pragma warning disable CS8618
        public event Action SettingsUpdatedExternally;
        #pragma warning restore CS8618

        // Properties

        public static bool Loaded => _loaded;

        public static Settings? UserSettings => _userSettings;

        public static List<SettingBase> VisibleSettings => new List<SettingBase>() {
            // General
            _userSettings._logDebugMessages,
            _userSettings._showLogInExplorer,

            // Game Folder
            _userSettings._gameFolder,
            _userSettings._findGameFolder,
            _userSettings._browseForGameFolder,

            // Mod Lists
            _userSettings._defaultModList,
            _userSettings._defaultModListSortOption,

        };

        // Constructors

        public Settings(){}

        public Settings(DialogService dialogService) {
            _dialogService = dialogService;
        }

        // Public Functions

        public static void RestoreDefaults() {
            _loaded = false;
            foreach (SettingBase setting in VisibleSettings) {
                setting.RestoreDefault();
            }
            _loaded = true;
        }

        public static IEnumerable<string> GetCategories() {
            return VisibleSettings.Select(setting => setting.Category).Distinct();
        }

        public static IEnumerable<SettingBase> GetSettingsInCategory(string category) {
            return VisibleSettings.Where(setting => setting.Category == category);
        }

        // Data Functions

        public static void Save() {
            string json = JsonConvert.SerializeObject(_userSettings, Formatting.Indented);
            File.WriteAllText(ProgramData.FilePaths.settingsFile, json);
        }

        public static void Load(DialogService dialogService) {
            if (!File.Exists(ProgramData.FilePaths.settingsFile)) {
                _userSettings = new Settings(dialogService);
                Save();
                return;
            }

            string json = File.ReadAllText(ProgramData.FilePaths.settingsFile);
            _userSettings = JsonConvert.DeserializeObject<Settings>(json) ?? new Settings(dialogService);
            _userSettings._dialogService = dialogService;
            _loaded = true;
        }
    }

    public abstract class SettingBase 
    {
        // Members
        private string _name;
        private string _description;
        private string _category;

        // Properties

        [JsonIgnore] public string Name => _name;
        [JsonIgnore] public string Description => _description;
        [JsonIgnore] public string Category => _category;

        // Constructors

        public SettingBase(string name, string description, string category) {
            _name = name;
            _description = description;
            _category = category;
        }

        // Public Functions

        public abstract void RestoreDefault();
    }

    public class Setting<T> : SettingBase 
    {
        // Members

        private T _value;
        private T _defaultValue;

        private static readonly Type[] typesToCheck = [typeof(int), typeof(float), typeof(double)];

        // Properties

        public T Value {
            get => _value;
            set {
                _value = value;
                if (Settings.Loaded) Settings.Save();
            }
        }

        // Constructors

        public Setting(string name, string description, string category, T defaultValue) : base(name, description, category) {
            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        // Public Functions

        public override void RestoreDefault() {
            Value = _defaultValue;
        }
    }

    public class ComparableSetting<T> : SettingBase where T : IComparable<T>
    {
        // Members
        
        private T _value;
        private T _defaultValue;
        private T? _min;
        private T? _max;

        private static readonly Type[] typesToCheck = [typeof(int), typeof(float), typeof(double)];

        // Properties

        public T Value {
            get => _value;
            set {
                if (typesToCheck.Contains(typeof(T))) {
                    if (_min != null && value.CompareTo(_min) < 0) {
                        Log.Warning($"Tried to set setting '{Name}' to '{value}' which is less than min '{_min}'");
                        value = _min;
                    }

                    if (_max != null && value.CompareTo(_max) > 0) {
                        Log.Warning($"Tried to set setting '{Name}' to '{value}' which is greater than max '{_max}'");
                        value = _max;
                    }
                }

                _value = value;
                if (Settings.Loaded) Settings.Save();
            }
        }

        // Constructors

        public ComparableSetting(string name, string description, string category, T defaultValue, T min, T max) : base(name, description, category) {
            _value = defaultValue;
            _defaultValue = defaultValue;
            _min = min;
            _max = max;
        }

        // Public Functions

        public override void RestoreDefault() {
            Value = _defaultValue;
        }
    }

    public class ButtonSetting : SettingBase 
    {
        // Members
        private string _buttonText;
        private Action _onClick;

        // Properties
        public string ButtonText => _buttonText;
        public Action OnClick => _onClick;

        // Public Functions

        public override void RestoreDefault(){}

        // Constructors

        public ButtonSetting(string name, string description, string category, string buttonText, Action onClick) : base(name, description, category) {
            _buttonText = buttonText;
            _onClick = onClick;
        }
    }

    public class EnumSetting<T> : SettingBase where T: Enum 
    {
        // Members
        private T _value;
        private T _defaultValue;

        // Properties

        public T Value {
            get => _value;
            set {
                _value = value;
                if(Settings.Loaded) Settings.Save();
            }
        }

        [JsonIgnore]
        public T[] Options => (T[])Enum.GetValues(typeof(T));

        // Constructors

        public EnumSetting(string name, string description, string category, T defaultValue) : base(name, description, category)
        {
            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        // Public Functions

        public override void RestoreDefault() {
            Value = _defaultValue;
        }
    }
}
