using Newtonsoft.Json;
using System.IO;
using TechtonicaModLoader.MVVM.Settings.ViewModels;

namespace TechtonicaModLoader.Stores
{
    public class UserSettings
    {
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

        public bool LogDebugMessages { 
            get => _logDebugMessages;
            set {
                _logDebugMessages = value;
                Save();
            }
        }
        private bool _logDebugMessages = false;

        public string GameFolder { 
            get => _gameFolder;
            set {
                _gameFolder = value;
                Save();
            }
        }
        private string _gameFolder = string.Empty;

        public ModListSource DefaultModList { 
            get => _defaultModList;
            set {
                _defaultModList = value;
                Save();
            }
        }
        private ModListSource _defaultModList = ModListSource.New;

        public ModListSortOption DefaultModListSortOption { 
            get => _defaultModListSortOption;
            set {
                _defaultModListSortOption = value;
                Save();
            }
        }
        private ModListSortOption _defaultModListSortOption = ModListSortOption.Alphabetical;

        public int ActiveProfileID { 
            get => _activeProfileID;
            set {
                _activeProfileID = value;
                Save();
            }
        }
        private int _activeProfileID = int.MaxValue;

        public List<string> SeenMods { 
            get => _seenMods;
            set {
                _seenMods = value;
                Save();
            }
        }
        private List<string> _seenMods = new();

        // Public Functions

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
            UserSettings settingsFromFile = JsonConvert.DeserializeObject<UserSettings>(json) ?? new UserSettings();

            // General
            _logDebugMessages = settingsFromFile.LogDebugMessages;
            Log.LogDebugToFile = LogDebugMessages;

            // Game Folder
            _gameFolder = settingsFromFile.GameFolder;

            // Mod Lists
            _defaultModList = settingsFromFile.DefaultModList;
            _defaultModListSortOption = settingsFromFile.DefaultModListSortOption;

            // Hidden
            _activeProfileID = settingsFromFile.ActiveProfileID;
            _seenMods = settingsFromFile.SeenMods;
        }
    }
}
