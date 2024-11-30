using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.MVVM.ViewModels.Settings;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.Windows.Settings
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        // Members

        private const string defaultCategory = "General";

        private UserSettings? userSettings;
        private IDialogService dialogService;

        private ObservableCollection<SettingBase> _settingViewModels = new ObservableCollection<SettingBase>();

        // Properties

        public ObservableCollection<SettingBase> SettingViewModels => _settingViewModels;
        public IEnumerable<SettingBase> SettingsToShow {
            get {
                return SettingViewModels.Where(setting => 
                    setting.Category == SelectedItem && 
                    setting.IsVisible
                );
            }
        }
        public IEnumerable<string> Categories {
            get {
                return SettingViewModels
                      .Where(setting => setting.IsVisible)
                      .Select(setting => setting.Category)
                      .Distinct();
            }
        }

        public Setting<bool> LogDebugMessages { get; }
        public ButtonSetting ShowLogInExplorer { get; }
        
        public Setting<string> GameFolder { get; }
        public ButtonSetting FindGameFolder { get; }
        public ButtonSetting BrowseForGameFolder { get; }
        
        public EnumSetting<ModListSource> DefaultModList { get; }
        public EnumSetting<ModListSortOption> DefaultModListSortOption { get; }

        public ComparableSetting<int> ActiveProfileID { get; }
        public Setting<bool> DeployNeededSetting { get; }
        public Setting<List<string>> SeenMods { get; }

        [ObservableProperty] string _selectedItem = defaultCategory;
        [ObservableProperty] bool _deployNeeded = false;

        // Constructors

        public SettingsWindowViewModel(UserSettings userSettings, IDialogService dialogService) {
            this.userSettings = userSettings;
            this.dialogService = dialogService;

            // General

            LogDebugMessages = new Setting<bool>(
                name: "Log Debug Messages",
                description: "Whether debug messages should be logged to file. Enable to gather info for a bug report.",
                category: defaultCategory,
                isVisible: true,
                getValueFunc: () => userSettings.LogDebugMessages,
                setValueFunc: value => userSettings.LogDebugMessages = value
            );

            ShowLogInExplorer = new ButtonSetting(
                name: "Show Log In Explorer",
                description: "Opens the folder that contains Techtonica Mod Loader's log file.",
                category: defaultCategory,
                isVisible: true,
                buttonText: "Show In Explorer",
                userSettings: userSettings,
                onClick: delegate (UserSettings settings) {
                    Process.Start(new ProcessStartInfo() {
                        FileName = ProgramData.FilePaths.LogsFolder,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            );

            // Game Folder

            GameFolder = new Setting<string>(
                name: "Game Folder",
                description: "Techtonica's Installation Location.",
                category: "Game Folder",
                isVisible: true,
                getValueFunc: () => userSettings.GameFolder,
                setValueFunc: value => userSettings.GameFolder = value
            );

            FindGameFolder = new ButtonSetting(
                name: "Find Game Folder",
                description: "Have Techtonica Mod Loader search for your Techtonica installation folder.",
                category: "Game Folder",
                isVisible: true,
                buttonText: "Find",
                userSettings: userSettings,
                onClick: delegate (UserSettings settings) {
                    // ToDo: Find Game Folder
                }
            );

            BrowseForGameFolder = new ButtonSetting(
                name: "Browse For Game Folder",
                description: "Manually browse for Techtonica's installation location.",
                category: "Game Folder",
                isVisible: true,
                buttonText: "Browse",
                userSettings: userSettings,
                onClick: delegate(UserSettings settings) {
                    OpenFileDialog browser = new OpenFileDialog { Filter = ("Techtonica.exe|*.exe") };
                    if (browser.ShowDialog() == true) {
                        if (browser.FileName.EndsWith("Techtonica.exe")) {
                            userSettings.GameFolder = Path.GetDirectoryName(browser.FileName) ?? "";
                            OnPropertyChanged(nameof(SettingsToShow));
                        }
                        else {
                            dialogService.ShowErrorMessage("Wrong File Selected", "You need to select the file 'Techtonica.exe'");
                        }
                    }
                }
            );

            // Mod List

            DefaultModList = new EnumSetting<ModListSource>(
                name: "Default Mod List",
                description: "The mod list that is displayed when you open Techtonica Mod Loader.",
                category: "Mod List",
                isVisible: true,
                getValueFunc: () => userSettings.DefaultModList,
                setValueFunc: value => userSettings.DefaultModList = value
            );

            DefaultModListSortOption = new EnumSetting<ModListSortOption>(
                name: "Default Sort Option",
                description: "The default sort option to apply to the mod list.",
                category: "Mod List",
                isVisible: true,
                getValueFunc: () => userSettings.DefaultModListSortOption,
                setValueFunc: value => userSettings.DefaultModListSortOption = value
            );

            // Hidden

            ActiveProfileID = new ComparableSetting<int>(
                name: "ActiveProfileID",
                description: "",
                category: defaultCategory,
                isVisible: false,
                getValueFunc: () => userSettings.ActiveProfileID,
                setValueFunc: value => userSettings.ActiveProfileID = value,
                min: 0,
                max: int.MaxValue
            );

            DeployNeededSetting = new Setting<bool>(
                name: "Deploy Needed",
                description: "",
                category: defaultCategory,
                isVisible: false,
                getValueFunc: () => userSettings.DeployNeeded,
                setValueFunc: value => userSettings.DeployNeeded = value
            );

            SeenMods = new Setting<List<string>>(
                name: "SeenMods",
                description: "",
                category: defaultCategory,
                isVisible: false,
                getValueFunc: () => userSettings.SeenMods,
                setValueFunc: value => userSettings.SeenMods = value
            );

            _settingViewModels.Add(LogDebugMessages);
            _settingViewModels.Add(ShowLogInExplorer);

            _settingViewModels.Add(GameFolder);
            _settingViewModels.Add(FindGameFolder);
            _settingViewModels.Add(BrowseForGameFolder);

            _settingViewModels.Add(DefaultModList);
            _settingViewModels.Add(DefaultModListSortOption);

            _settingViewModels.Add(ActiveProfileID);
            _settingViewModels.Add(SeenMods);

            ValidateSettings();
        }

        // Commands

        [RelayCommand]
        private void RestoreDefaults() {
            if(userSettings == null) {
                string error = "Can't restore defaults for null userSettings";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return;
            }

            userSettings.RestoreDefaults();
            OnPropertyChanged(nameof(SettingsToShow));
        }

        // Private Functions

        private void ValidateSettings() {
            foreach(SettingBase setting in _settingViewModels) {
                if (setting.IsVisible && !setting.Description.EndsWith(".")) {
                    throw new Exception($"Setting \"{setting.Name}\"'s Description Property doesn't end with '.'");
                }

                if(!setting.IsVisible && setting.Category != defaultCategory) {
                    throw new Exception($"Setting \"{setting.Name}\" is hidden, but it's Category property isn't default ('{defaultCategory}')");
                }
            }
        }

        partial void OnSelectedItemChanged(string value) {
            OnPropertyChanged(nameof(SettingsToShow));
        }
    }
}
