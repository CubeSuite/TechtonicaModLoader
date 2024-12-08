using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace TechtonicaModLoader.Stores.Settings
{
    public interface IUserSettings
    {
        // Properties

        bool LogDebugMessages { get; set; }
        string GameFolder { get; set; }
        ModListSource DefaultModList { get; set; }
        ModListSortOption DefaultModListSortOption { get; set; }
        string BackupsFolder { get; set; }
        int NumBackups { get; set; }
        bool IsFirstTimeLaunch { get; set; }
        int ActiveProfileID { get; set; }
        bool DeployNeeded { get; set; }
        List<string> SeenMods { get; set; }
        Color DimBackground { get; set; }
        Color NormalBackground { get; set; }
        Color BrightBackground { get; set; }
        Color UiBackground { get; set; }
        Color AccentColour { get; set; }
        Color TextColour { get; set; }

        // Events

        event Action DeployNeededChanged;

        // Public Methods

        void RestoreDefaults();
    }

    public class UserSettings : IUserSettings
    {
        // Members

        private readonly ISettingsFileHandler settingsFileHandler;
        private readonly SettingsData settingsData;

        // Events

        public event Action? DeployNeededChanged;

        // Properties
        #region Properties
        public bool LogDebugMessages {
            get => settingsData.LogDebugMessages;
            set {
                settingsData.LogDebugMessages = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public string GameFolder {
            get => settingsData.GameFolder;
            set {
                settingsData.GameFolder = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public ModListSource DefaultModList {
            get => settingsData.DefaultModList;
            set {
                settingsData.DefaultModList = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public ModListSortOption DefaultModListSortOption {
            get => settingsData.DefaultModListSortOption;
            set {
                settingsData.DefaultModListSortOption = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public string BackupsFolder {
            get => settingsData.BackupsFolder;
            set {
                settingsData.BackupsFolder = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public int NumBackups {
            get => settingsData.NumBackups;
            set {
                settingsData.NumBackups = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public bool IsFirstTimeLaunch {
            get => settingsData.IsFirstTimeLaunch;
            set {
                settingsData.IsFirstTimeLaunch = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public int ActiveProfileID {
            get => settingsData.ActiveProfileID;
            set {
                settingsData.ActiveProfileID = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public bool DeployNeeded {
            get => settingsData.DeployNeeded;
            set {
                settingsData.DeployNeeded = value;
                settingsFileHandler.Save(settingsData);
                DeployNeededChanged?.Invoke();
            }
        }

        public List<string> SeenMods {
            get => settingsData.SeenMods;
            set {
                settingsData.SeenMods = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public Color DimBackground {
            get => settingsData.DimBackground;
            set {
                settingsData.DimBackground = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public Color NormalBackground {
            get => settingsData.NormalBackground;
            set {
                settingsData.NormalBackground = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public Color BrightBackground {
            get => settingsData.BrightBackground;
            set {
                settingsData.BrightBackground = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public Color UiBackground {
            get => settingsData.UiBackground;
            set {
                settingsData.UiBackground = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public Color AccentColour {
            get => settingsData.AccentColour;
            set {
                settingsData.AccentColour = value;
                settingsFileHandler.Save(settingsData);
            }
        }

        public Color TextColour {
            get => settingsData.TextColour;
            set {
                settingsData.TextColour = value;
                settingsFileHandler.Save(settingsData);
            }
        }
        #endregion

        // Constructors

        public UserSettings(IServiceProvider serviceProvider) {
            settingsFileHandler = serviceProvider.GetRequiredService<ISettingsFileHandler>();
            settingsData = settingsFileHandler.Load();
        }

        // Public Methods

        public void RestoreDefaults() {
            settingsData.LogDebugMessages = false;

            settingsData.GameFolder = "";
            settingsData.DefaultModList = ModListSource.New;
            settingsData.DefaultModListSortOption = ModListSortOption.Newest;

            settingsFileHandler.Save(settingsData);
        }
    }
}
