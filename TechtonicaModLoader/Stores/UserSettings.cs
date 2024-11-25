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
using TechtonicaModLoader.MVVM.Settings;
using TechtonicaModLoader.Services;

namespace TechtonicaModLoader.Stores
{
    public class UserSettings
    {
        #region Settings

        // To add a new setting:
        // 1 - Create _myNewSetting = new Setting<type>() below
        //   1.1 - For int, float, double, use ComparableSetting<type>()
        //
        // 2 - Add _myNewSetting to VisibleSettings
        //   2.1 - If it's a hidden setting, skip this step
        //
        // 3 - Add a public getter property: SettingType MyNewSetting => _myNewSetting;
        //
        // 4 - If it's an enum setting or a new type that's not yet handled, you'll need to:
        //   4.1 - Add a member to SettingViewModel
        //   4.2 - Make a new constructor in SettingViewModel
        //   4.3 - Add an if statement to SettingViewModel.OnValueChanged()
        //   4.4 - Add an if statement to SettingViewModel.SettingTemplateSelector.SelectTemplate()
        //   4.4 - Add an if statement to SettingsWindowViewModel.PopulateSettingsToShow()
        //
        // 5 - Add a line to Settings.Load() if it's not a ButtonSetting

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
            delegate (UserSettings settings) {
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
            delegate (UserSettings settings) {
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
            delegate (UserSettings settings) {
                OpenFileDialog browser = new OpenFileDialog { Filter = ("Techtonica.exe|*.exe") };
                if (browser.ShowDialog() == true) {
                    if (browser.FileName.EndsWith("Techtonica.exe")) {
                        settings.GameFolder.Value = Path.GetDirectoryName(browser.FileName) ?? "";
                        settings.SettingsUpdatedExternally?.Invoke();
                    }
                    else {
                        settings.dialogService.ShowErrorMessage("Wrong File Selected", "You need to select the file 'Techtonica.exe'");
                    }
                }
            }
        );

        // Mod Lists

        public EnumSetting<ModListSource> DefaultModList => _defaultModList;
        private EnumSetting<ModListSource> _defaultModList = new EnumSetting<ModListSource>(
            "Default Mod List",
            "The mod list that is displayed when you open Techtonica Mod Loader",
            "Mod Lists",
            ModListSource.New
        );

        public EnumSetting<ModListSortOption> DefaultModListSortOption => _defaultModListSortOption;
        private EnumSetting<ModListSortOption> _defaultModListSortOption = new EnumSetting<ModListSortOption>(
            "Default Sort Option",
            "The default sort option to apply to mod lists.",
            "Mod Lists",
            ModListSortOption.Alphabetical
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

        private IDialogService dialogService;

        // Events

        public event Action SettingsUpdatedExternally;

        // Properties

        private static bool _loaded = false;
        [JsonIgnore] public bool Loaded => _loaded;

        [JsonIgnore]
        public List<SettingBase> VisibleSettings => new List<SettingBase>() {
            // General
            _logDebugMessages,
            _showLogInExplorer,

            // Game Folder
            _gameFolder,
            _findGameFolder,
            _browseForGameFolder,

            // Mod Lists
            _defaultModList,
            _defaultModListSortOption,
        };

        // Constructors

        public UserSettings(){}
        public UserSettings(IDialogService dialogService) {
            this.dialogService = dialogService;
        }

        // Events

        private void OnSettingUpdated() {
            if(Loaded) Save();
        }

        // Public Functions

        public void RestoreDefaults() {
            _loaded = false;
            foreach (SettingBase setting in VisibleSettings) {
                setting.RestoreDefault();
            }
            _loaded = true;
        }

        public IEnumerable<string> GetCategories() {
            return VisibleSettings.Select(setting => setting.Category).Distinct();
        }

        public IEnumerable<SettingBase> GetSettingsInCategory(string category) {
            return VisibleSettings.Where(setting => setting.Category == category);
        }

        // Data Functions

        public void Save() {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ProgramData.FilePaths.settingsFile, json);
        }

        public void Load() {
            if (!File.Exists(ProgramData.FilePaths.settingsFile)) {
                Save();
                return;
            }

            string json = File.ReadAllText(ProgramData.FilePaths.settingsFile);
            UserSettings settingsFromFile = JsonConvert.DeserializeObject<UserSettings>(json) ?? new UserSettings(dialogService);

            // General
            LogDebugMessages.Value = settingsFromFile.LogDebugMessages.Value;
            Log.LogDebugToFile = LogDebugMessages.Value;

            // Game Folder
            GameFolder.Value = settingsFromFile.GameFolder.Value;

            // Mod Lists
            DefaultModList.Value = settingsFromFile.DefaultModList.Value;
            DefaultModListSortOption.Value = settingsFromFile.DefaultModListSortOption.Value;

            // Hidden
            ActiveProfileID.Value = settingsFromFile.ActiveProfileID.Value;
            SeenMods.Value = settingsFromFile.SeenMods.Value;

            _loaded = true;

            _logDebugMessages.SettingUpdated += OnSettingUpdated;

            _gameFolder.SettingUpdated += OnSettingUpdated;

            _defaultModList.SettingUpdated += OnSettingUpdated;
            _defaultModListSortOption.SettingUpdated += OnSettingUpdated;

            _activeProfileID.SettingUpdated += OnSettingUpdated;
            _seenMods.SettingUpdated += OnSettingUpdated;
        }
    }
}
