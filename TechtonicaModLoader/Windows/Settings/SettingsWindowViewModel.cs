using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.MVVM.Settings;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Windows.Settings.Setting;

namespace TechtonicaModLoader.Windows.Settings
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        // Members

        private UserSettings userSettings;
        private IDialogService dialogService;

        // Properties

        [ObservableProperty] private ObservableCollection<SettingViewModel> _settingsToShow;
        [ObservableProperty] private object? _selectedItem;

        public IEnumerable<string> Categories => userSettings.GetCategories();

        // Events

        public event Action CloseButtonClicked;

        // Constructors

        public SettingsWindowViewModel(UserSettings userSettings, IDialogService dialogService) {
            this.userSettings = userSettings;
            this.dialogService = dialogService;
            _settingsToShow = new ObservableCollection<SettingViewModel>();
            userSettings.SettingsUpdatedExternally += OnSettingsUpdatedExternally;

            SelectedItem = "General";
            PopulateSettingsToShow("General");
        }

        // Commands

        [RelayCommand]
        private void RestoreDefaults() {
            if(dialogService.GetUserConfirmation("Restore Defaults?", "Are you sure you want to restore the default settings? This cannot be undone.")) {
                userSettings.RestoreDefaults();
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
            IEnumerable<SettingBase> settingsToShow = userSettings.GetSettingsInCategory(category);
            foreach (SettingBase setting in settingsToShow) {
                if (setting is Setting<bool> boolSetting) SettingsToShow.Add(new SettingViewModel(boolSetting, userSettings));
                else if (setting is Setting<string> stringSetting) SettingsToShow.Add(new SettingViewModel(stringSetting, userSettings));
                else if (setting is ButtonSetting buttonSetting) SettingsToShow.Add(new SettingViewModel(buttonSetting, userSettings));
                else if (setting is EnumSetting<ModListSortOption> modListSortSetting) SettingsToShow.Add(new SettingViewModel(modListSortSetting, userSettings));
                else if (setting is EnumSetting<ModListSource> modListSourceSetting) SettingsToShow.Add(new SettingViewModel(modListSourceSetting, userSettings));
            }
        }
    }
}
