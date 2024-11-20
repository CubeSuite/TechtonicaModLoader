using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Windows.Settings.Setting;

namespace TechtonicaModLoader.Windows.Settings
{
    public partial class SettingsWindowViewModel : ObservableObject
    {
        // Members

        private SettingsWindow _view;
        private DialogService _dialogService;

        // Properties

        [ObservableProperty]
        private ObservableCollection<SettingViewModel> _settingsToShow = new ObservableCollection<SettingViewModel>();

        [ObservableProperty]
        private object _selectedItem;

        public IEnumerable<string> Categories => Stores.Settings.GetCategories();

        // Commands

        [RelayCommand]
        private void RestoreDefaults() {

            // ToDo: GetConfirmation
            Stores.Settings.RestoreDefaults();
        }

        [RelayCommand]
        private void CloseSettings() {
            _view.Close();
        }

        // Events

        partial void OnSelectedItemChanged(object value) {
            string category = value.ToString() ?? "Null Category";
            Log.Debug($"Show settings under category {category}");
            PopulateSettingsToShow(category);
        }

        private void OnSettingsUpdatedExternally() {
            if(SelectedItem == null) {
                Log.Warning("SettingsWindowViewModel.SelectedItem is null, showing General settings instead");
            }

            PopulateSettingsToShow(SelectedItem?.ToString() ?? "General");
        }

        // Constructors

        public SettingsWindowViewModel(SettingsWindow view, DialogService dialogService)
        {
            Stores.Settings.UserSettings.SettingsUpdatedExternally += OnSettingsUpdatedExternally;
            
            _view = view;
            _dialogService = dialogService;

            SelectedItem = "General";
            PopulateSettingsToShow("General");
        }

        // Private Functions

        private void PopulateSettingsToShow(string category) {
            SettingsToShow.Clear();
            IEnumerable<SettingBase> settingsToShow = Stores.Settings.GetSettingsInCategory(category);
            foreach (SettingBase setting in settingsToShow) {
                if (setting is Setting<bool> boolSetting) SettingsToShow.Add(new SettingViewModel(boolSetting));
                else if (setting is Setting<string> stringSetting) SettingsToShow.Add(new SettingViewModel(stringSetting));
                else if (setting is ButtonSetting buttonSetting) SettingsToShow.Add(new SettingViewModel(buttonSetting));
                else if (setting is EnumSetting<ModListSortOption> modListSortSetting) SettingsToShow.Add(new SettingViewModel(modListSortSetting));
                else if (setting is EnumSetting<ModListSource> modListSourceSetting) SettingsToShow.Add(new SettingViewModel(modListSourceSetting));
            }
        }
    }
}
