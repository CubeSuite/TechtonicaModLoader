using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Settings.ViewModels
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        // Members

        private UserSettings _userSettings;
        private readonly IDialogService _dialogService;

        // Properties

        [ObservableProperty] private ObservableCollection<SettingViewModel> _settingsToShow;
        [ObservableProperty] private object? _selectedItem;

        public Setting<bool> LogDebugMessages => _logDebugMessages;
        private Setting<bool> _logDebugMessages;

        public ButtonSetting ShowLogInExplorer => _showLogInExplorer;
        private ButtonSetting _showLogInExplorer;

        public Setting<string> GameFolder => _gameFolder;
        private Setting<string> _gameFolder;

        public ButtonSetting FindGameFolder => _findGameFolder;
        private ButtonSetting _findGameFolder;

        public ButtonSetting BrowseForGameFolder => _browseForGameFolder;
        private ButtonSetting _browseForGameFolder;

        public EnumSetting<ModListSource> DefaultModList => _defaultModList;
        private EnumSetting<ModListSource> _defaultModList;

        public EnumSetting<ModListSortOption> DefaultModListSortOption => _defaultModListSortOption;
        private EnumSetting<ModListSortOption> _defaultModListSortOption;

        public ComparableSetting<int> ActiveProfileID => _activeProfileID;
        private ComparableSetting<int> _activeProfileID;

        public Setting<List<string>> SeenMods => _seenMods;
        private Setting<List<string>> _seenMods;

        public IEnumerable<string> Categories => _userSettings.GetCategories();

        // Events

        public event Action? CloseButtonClicked;

        // Constructors

        public SettingsWindowViewModel(IDialogService dialogService) {
            _dialogService = dialogService;
            _userSettings = new();
            _settingsToShow = new();

#region Field Initializers
            _logDebugMessages = new Setting<bool>(
                "Log Debug Messages",
                "Whether Debug messages should be logged to file.",
                "General",
                () => _userSettings.LogDebugMessages,
                v => _userSettings.LogDebugMessages = v,
                isVisible: true);

            _showLogInExplorer = new ButtonSetting(
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
                },
                isVisible: true);

            // Game Folder
            _gameFolder = new Setting<string>(
                "Game Folder",
                "Techtonica's installation location",
                "Game Folder",
                () => _userSettings.GameFolder,
                v => _userSettings.GameFolder = v,
                isVisible: true);

            _findGameFolder = new ButtonSetting(
                "Find Game Folder",
                "Have TML search for your Techtonica installation folder.",
                "Game Folder",
                "Find",
                delegate (UserSettings settings) {
                    // ToDo: Find game folder
                },
                isVisible: true);

            _browseForGameFolder = new ButtonSetting(
                "Browse For Game Folder",
                "Manually browse for Techtonica's installation folder.",
                "Game Folder",
                "Browse",
                delegate (UserSettings settings) {
                    OpenFileDialog browser = new OpenFileDialog { Filter = ("Techtonica.exe|*.exe") };
                    if (browser.ShowDialog() == true) {
                        if (browser.FileName.EndsWith("Techtonica.exe")) {
                            _userSettings.GameFolder = Path.GetDirectoryName(browser.FileName) ?? "";
                            OnSettingsUpdatedExternally();
                        }
                        else {
                            _dialogService.ShowErrorMessage("Wrong File Selected", "You need to select the file 'Techtonica.exe'");
                        }
                    }
                },
                isVisible: true);

            // Mod Lists

            _defaultModList = new EnumSetting<ModListSource>(
                "Default Mod List",
                "The mod list that is displayed when you open Techtonica Mod Loader",
                "Mod Lists",
                ModListSource.New,
                isVisible: true);

            _defaultModListSortOption = new EnumSetting<ModListSortOption>(
                "Default Sort Option",
                "The default sort option to apply to mod lists.",
                "Mod Lists",
                ModListSortOption.Alphabetical,
                isVisible: true);

            // Hidden

            _activeProfileID = new ComparableSetting<int>(
                "Active Profile ID",
                "The ID of the user's current profile",
                "General",
                0,
                0,
                int.MaxValue,
                isVisible: false);

            _seenMods = new Setting<List<string>>(
                "Seen Mods",
                "Mods that have appeared in TML",
                "General",
                () => _userSettings.SeenMods,
                v => _userSettings.SeenMods = v,
                isVisible: false);
#endregion

            SelectedItem = "General";
            PopulateSettingsToShow("General");
        }

        // Commands

        [RelayCommand]
        private void RestoreDefaults() {
            if(_dialogService.GetUserConfirmation("Restore Defaults?", "Are you sure you want to restore the default settings? This cannot be undone.")) {
                _userSettings = new();
            }
        }

        [RelayCommand]
        private void CloseDialog() {
            CloseButtonClicked?.Invoke();
        }

        // Events

        partial void OnSelectedItemChanged(object? value) {
            string category = value?.ToString() ?? "Null Category";
            Log.Debug($"Show settings under category {category}");
            PopulateSettingsToShow(category);
        }

        private void OnSettingsUpdatedExternally() {
            if(SelectedItem == null) {
                Log.Warning("SettingsWindowViewModel.SelectedItem is null, showing General settings instead");
            }

            PopulateSettingsToShow(SelectedItem?.ToString() ?? "General");
        }

        // Private Functions

        private void PopulateSettingsToShow(string category) {
            SettingsToShow.Clear();
            IEnumerable<SettingBase> settingsToShow = _userSettings.GetSettingsInCategory(category);
            foreach (SettingBase setting in settingsToShow) {
                if (setting is Setting<bool> boolSetting) SettingsToShow.Add(new SettingViewModel(boolSetting, _userSettings));
                else if (setting is Setting<string> stringSetting) SettingsToShow.Add(new SettingViewModel(stringSetting, _userSettings));
                else if (setting is ButtonSetting buttonSetting) SettingsToShow.Add(new SettingViewModel(buttonSetting, _userSettings));
                else if (setting is EnumSetting<ModListSortOption> modListSortSetting) SettingsToShow.Add(new SettingViewModel(modListSortSetting, _userSettings));
                else if (setting is EnumSetting<ModListSource> modListSourceSetting) SettingsToShow.Add(new SettingViewModel(modListSourceSetting, _userSettings));
            }
        }
    }
}
