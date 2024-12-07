using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
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

        private readonly ILoggerService logger;
        private readonly IUserSettings userSettings;
        private readonly IDialogService dialogService;
        private readonly IDebugUtils debugUtils;
        private readonly IProgramData programData;

        private readonly SettingViewModelHelper settingViewModelHelper;

        // Properties

        public List<SettingBase> SettingViewModels => settingViewModelHelper.SettingViewModels;

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

        [ObservableProperty] string _selectedItem = defaultCategory;
        [ObservableProperty] bool _deployNeeded = false;
        [ObservableProperty] string _closeSource;

        // Constructors

        public SettingsWindowViewModel(IServiceProvider serviceProvider) {
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            userSettings = serviceProvider.GetRequiredService<IUserSettings>();
            dialogService = serviceProvider.GetRequiredService<IDialogService>();
            debugUtils = serviceProvider.GetRequiredService<IDebugUtils>();
            programData = serviceProvider.GetRequiredService<IProgramData>();

            settingViewModelHelper = new SettingViewModelHelper(serviceProvider, OnSettingChanged);

            _closeSource = $"{programData.FilePaths.ResourcesFolder}\\ControlBox\\Close.svg";
        }

        // Commands

        [RelayCommand]
        private void RestoreDefaults() {
            userSettings.RestoreDefaults();
            OnPropertyChanged(nameof(SettingsToShow));
        }

        // Events

        partial void OnSelectedItemChanged(string value) {
            OnPropertyChanged(nameof(SettingsToShow));
        }

        private void OnSettingChanged() {
            OnPropertyChanged(nameof(SettingsToShow));
        }
    }
}
