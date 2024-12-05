using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechtonicaModLoader.MVVM.ViewModels.Settings;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.Windows.Settings
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        // Members

        private const string defaultCategory = "General";

        private readonly UserSettings userSettings;
        private readonly SettingViewModelHelper settingViewModelHelper;

        // Properties

        public IEnumerable<SettingBase> SettingsToShow {
            get {
                return SettingViewModels.Where(setting => 
                    setting.Category == SelectedItem
                );
            }
        }

        public IEnumerable<string> Categories {
            get {
                return SettingViewModels
                      .Select(setting => setting.Category)
                      .Distinct();
            }
        }

        public List<SettingBase> SettingViewModels => settingViewModelHelper.SettingViewModels;

        [ObservableProperty] string _selectedItem = defaultCategory;
        [ObservableProperty] bool _deployNeeded = false;

        // Constructors

        public SettingsWindowViewModel(UserSettings userSettings, IDialogService dialogService) {
            this.userSettings = userSettings;
            settingViewModelHelper = new(this.userSettings, dialogService, SettingChanged, defaultCategory);
        }

        // Commands

        [RelayCommand]
        private void RestoreDefaults() {
            userSettings.RestoreDefaults();
            OnPropertyChanged(nameof(SettingsToShow));
        }

        // Private Functions

        partial void OnSelectedItemChanged(string value) {
            OnPropertyChanged(nameof(SettingsToShow));
        }

        private void SettingChanged() {
            OnPropertyChanged(nameof(SettingsToShow));
        }
    }
}
