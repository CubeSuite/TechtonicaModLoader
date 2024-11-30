using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Stores
{
    public interface IUserSettings
    {
        bool LogDebugMessages { get; set; }
        
        string GameFolder { get; set; }
        
        ModListSource DefaultModList { get; set; }
        ModListSortOption DefaultModListSortOption { get; set; }
        
        int ActiveProfileID { get; set; }
        bool DeployNeeded { get; set; }
        List<string> SeenMods { get; set; }

        void Load();
        void RestoreDefaults();
        void Save();
    }

    public class UserSettings : IUserSettings
    {
        // Members

        private bool loaded = false;

        private bool _logDebugMessages = false;

        private string _gameFolder = "";

        private ModListSource _defaultModList = ModListSource.New;
        private ModListSortOption _defaultModListSortOption = ModListSortOption.Newest;

        private int _activeProfileID = 0;
        private bool _deployNeeded = false;
        private List<string> _seenMods = new List<string>();

        // Properties

        public bool LogDebugMessages {
            get => _logDebugMessages;
            set {
                _logDebugMessages = value;
                if (loaded) Save();
            }
        }

        public string GameFolder {
            get => _gameFolder;
            set {
                _gameFolder = value;
                if (loaded) Save();
            }
        }

        public ModListSource DefaultModList {
            get => _defaultModList;
            set {
                _defaultModList = value;
                if (loaded) Save();
            }
        }

        public ModListSortOption DefaultModListSortOption {
            get => _defaultModListSortOption;
            set {
                _defaultModListSortOption = value;
                if (loaded) Save();
            }
        }

        public int ActiveProfileID {
            get => _activeProfileID;
            set {
                _activeProfileID = value;
                if (loaded) Save();
            }
        }

        public bool DeployNeeded {
            get => _deployNeeded;
            set {
                _deployNeeded = value;
                if (loaded) Save();
            }
        }

        public List<string> SeenMods {
            get => _seenMods;
            set {
                _seenMods = value;
                if (loaded) Save();
            }
        }

        // Public Functions

        public void Save() {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(ProgramData.FilePaths.SettingsFile, json);
        }

        public void Load() {
            if (!File.Exists(ProgramData.FilePaths.SettingsFile)) {
                loaded = true;
                return;
            }

            string json = File.ReadAllText(ProgramData.FilePaths.SettingsFile);
            UserSettings settingsFromFile = JsonConvert.DeserializeObject<UserSettings>(json) ?? new UserSettings();

            LogDebugMessages = settingsFromFile.LogDebugMessages;
            Log.LogDebugToFile = LogDebugMessages;

            GameFolder = settingsFromFile.GameFolder;
            DefaultModList = settingsFromFile.DefaultModList;
            DefaultModListSortOption = settingsFromFile.DefaultModListSortOption;
            ActiveProfileID = settingsFromFile.ActiveProfileID;
            SeenMods = settingsFromFile.SeenMods;

            ProgramData.FilePaths.BepInExFolder = $"{GameFolder}\\BepInEx";

            loaded = true;
        }

        public void RestoreDefaults() {
            _logDebugMessages = false;

            _gameFolder = "";
            _defaultModList = ModListSource.New;
            _defaultModListSortOption = ModListSortOption.Newest;

            Save();
        }
    }
}
