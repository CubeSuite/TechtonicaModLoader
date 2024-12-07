using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO;

namespace TechtonicaModLoader.Stores
{
    public interface IUserSettings
    {
        // Properties
        bool LogDebugMessages { get; set; }
        
        string GameFolder { get; set; }
        
        ModListSource DefaultModList { get; set; }
        ModListSortOption DefaultModListSortOption { get; set; }
        
        int ActiveProfileID { get; set; }
        bool DeployNeeded { get; set; }
        List<string> SeenMods { get; set; }

        // Events

        event Action DeployNeededChanged;

        // Public Functions

        void RestoreDefaults();
        void Save();
    }

    public class UserSettings : IUserSettings
    {
        // Members

        private ILoggerService logger;
        private IProgramData programData;
        private IDebugUtils debugUtils;

        private bool loaded = false;

        private bool _logDebugMessages = false;

        private string _gameFolder = "";

        private ModListSource _defaultModList = ModListSource.New;
        private ModListSortOption _defaultModListSortOption = ModListSortOption.Newest;

        private int _activeProfileID = 0;
        private bool _deployNeeded = false;
        private List<string> _seenMods = new List<string>();

        // Events

        public event Action DeployNeededChanged;

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
                DeployNeededChanged?.Invoke();
            }
        }

        public List<string> SeenMods {
            get => _seenMods;
            set {
                _seenMods = value;
                if (loaded) Save();
            }
        }

        // Constructors

        public UserSettings(){}

        public UserSettings(IServiceProvider serviceProvider)
        {
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            programData = serviceProvider.GetRequiredService<IProgramData>();
            debugUtils = serviceProvider.GetRequiredService<IDebugUtils>();

            Load(serviceProvider);
        }

        // Public Functions

        public void Save() {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(programData.FilePaths.SettingsFile, json);
        }

        private void Load(IServiceProvider serviceProvider) {
            if (!File.Exists(programData.FilePaths.SettingsFile)) {
                loaded = true;
                logger.Warning("Settings file not found");
                return;
            }

            string json = File.ReadAllText(programData.FilePaths.SettingsFile);
            UserSettings? settingsFromFile = JsonConvert.DeserializeObject<UserSettings>(json);
            if(settingsFromFile == null) {
                string error = "Parsed Settings.json is null";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);
                return;
            }

            LogDebugMessages = settingsFromFile.LogDebugMessages;
            logger.LogDebugToFile = LogDebugMessages;

            GameFolder = settingsFromFile.GameFolder;
            DefaultModList = settingsFromFile.DefaultModList;
            DefaultModListSortOption = settingsFromFile.DefaultModListSortOption;
            ActiveProfileID = settingsFromFile.ActiveProfileID;
            SeenMods = settingsFromFile.SeenMods;

            programData.FilePaths.BepInExFolder = $"{GameFolder}\\BepInEx";

            loaded = true;
            logger.Info("Settings loaded");
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
