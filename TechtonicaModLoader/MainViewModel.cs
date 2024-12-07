using Accessibility;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TechtonicaModLoader.MVVM.Models;
using TechtonicaModLoader.MVVM.ViewModels;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Windows;
using TechtonicaModLoader.Windows.Settings;

namespace TechtonicaModLoader.MVVM
{
    public partial class MainViewModel : ObservableObject
    {
        // Members

        private IServiceProvider serviceProvider;
        private ILoggerService logger;
        private IDialogService dialogService;
        private IProfileManager profileManager;
        private IThunderStore thunderStore;
        private IModFilesManager modFilesManager;
        private IProgramData programData;
        private IUserSettings userSettings;

        // Properties

        private Version ProgramVersion => programData.ProgramVersion;
        public string Title => $"Techtonica Mod Loader - V{ProgramVersion.Major}.{ProgramVersion.Minor}.{ProgramVersion.Build}";

        [ObservableProperty] private string _settingsSource;
        [ObservableProperty] private string _minimiseSource;
        [ObservableProperty] private string _closeSource;
        [ObservableProperty] private string _addSource;

        [ObservableProperty] private bool _modUpdatesAvailable = false;
        [ObservableProperty] private ModListSource _selectedModList;
        [ObservableProperty] private ModListSortOption _selectedSortOption;
        [ObservableProperty] private string _searchTerm = "";
        [ObservableProperty] private ObservableCollection<ModViewModel> _modsToShow;

        public Profile ActiveProfile {
            get => profileManager.ActiveProfile;
            set {
                profileManager.ActiveProfile = value;
                PopulateModsToShow();
            }
        }

        public ObservableCollection<Profile> Profiles => profileManager.ProfilesList;

        public Array? ModLists {
            get {
                if (thunderStore.Connected) return Enum.GetValues(typeof(ModListSource));
                else return new ModListSource[] { ModListSource.Downloaded, ModListSource.Enabled, ModListSource.Disabled };
            }
        }

        public Array? SortOptions => Enum.GetValues(typeof(ModListSortOption));

        public bool DeployNeeded => userSettings.DeployNeeded;
        public string LaunchButtonText => DeployNeeded ? "Deploy & Launch Game" : "Launch Game";

        // Constructors

        public MainViewModel(IServiceProvider serviceProvider) {
            _modsToShow = new ObservableCollection<ModViewModel>();

            this.serviceProvider = serviceProvider;
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            dialogService = serviceProvider.GetRequiredService<IDialogService>();
            profileManager = serviceProvider.GetRequiredService<IProfileManager>();
            thunderStore = serviceProvider.GetRequiredService<IThunderStore>();
            modFilesManager = serviceProvider.GetRequiredService<IModFilesManager>();
            programData = serviceProvider.GetRequiredService<IProgramData>();
            userSettings = serviceProvider.GetRequiredService<IUserSettings>();

            SelectedModList = userSettings.DefaultModList;
            SelectedSortOption = userSettings.DefaultModListSortOption;

            profileManager.PropertyChanged += OnProfileManagerPropertyChanged;
            thunderStore.PropertyChanged += OnThunderstorePropertyChanged;
            userSettings.DeployNeededChanged += OnDeployNeededChanged;

            _settingsSource = $"{programData.FilePaths.ResourcesFolder}\\ControlBox\\Settings.svg";
            _minimiseSource = $"{programData.FilePaths.ResourcesFolder}\\ControlBox\\Minimise.svg";
            _closeSource = $"{programData.FilePaths.ResourcesFolder}\\ControlBox\\Close.svg";
            _addSource = $"{programData.FilePaths.ResourcesFolder}\\GUI\\Add.svg";
        }

        // Commands

        [RelayCommand]
        private void DeployMods() {
            if (modFilesManager.DeployMods()) {
                userSettings.DeployNeeded = false;
            }
        }

        [RelayCommand]
        private void LaunchGame() {
            if (DeployNeeded) DeployMods();
        }

        [RelayCommand]
        private void OpenSettings() {
            dialogService.OpenSettingsDialog(serviceProvider);
        }

        [RelayCommand]
        private void MinimiseProgram() {
            MainWindow.current.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private void CloseProgram() {
            logger.Info("Closing program");
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void UpdateAllMods() {
            // ToDo: UpdateAllMods()
        }

        [RelayCommand]
        private void CheckForModUpdates() {
            thunderStore.UpdateModCache();
        }

        [RelayCommand]
        private void CreateNewProfile() {
            if(dialogService.GetStringFromUser(out string name, "Enter Profile Name:", "")) {
                profileManager.CreateNewProfile(name);
            }
        }

        [RelayCommand]
        private void DeleteProfile() {
            string name = profileManager.ActiveProfile.Name;
            if(dialogService.GetUserConfirmation("Delete Profile?", $"Are you sure you want to delete the {name} profile?\nThis cannot be undone.")) {
                profileManager.DeleteActiveProfile();
            }
        }

        // Events

        partial void OnSearchTermChanged(string value) {
            PopulateModsToShow();
        }

        partial void OnSelectedModListChanged(ModListSource value) {
            ModListSource[] onlineLists = new ModListSource[] { ModListSource.All, ModListSource.New, ModListSource.NotDownloaded };

            if(!thunderStore.Connected && (SelectedModList == ModListSource.All || SelectedModList == ModListSource.New || SelectedModList == ModListSource.NotDownloaded)) {
                if (MainWindow.current == null) return;

                dialogService.ShowErrorMessage("Not Connected To Thunderstore", "You can't browse online mods while in offline mode");
                SelectedModList = ModListSource.Downloaded;
            }

            PopulateModsToShow();
        }

        partial void OnSelectedSortOptionChanged(ModListSortOption value) {
            PopulateModsToShow();
        }

        private void OnProfileManagerPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            switch (e.PropertyName) {
                case nameof(ProfileManager.ActiveProfile): OnPropertyChanged(nameof(ActiveProfile)); break;
                case nameof(ProfileManager.ProfilesList): OnPropertyChanged(nameof(Profiles)); break;
            }
        }

        private void OnThunderstorePropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if(e.PropertyName == nameof(ThunderStore.ModCache)) {
                PopulateModsToShow();
            }

            if(e.PropertyName == nameof(ThunderStore.Connected)) {
                if (!thunderStore.Connected) {
                    SelectedModList = ModListSource.Downloaded;
                }
                else {
                    SelectedModList = userSettings.DefaultModList;
                }

                OnPropertyChanged(nameof(SelectedModList));
                OnPropertyChanged(nameof(ModLists));
            }
        }

        private void OnDeployNeededChanged() {
            OnPropertyChanged(nameof(DeployNeeded));
        }

        private void OnProgramDataPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            OnPropertyChanged(nameof(DeployNeeded));
            OnPropertyChanged(nameof(LaunchButtonText));
        }

        // Private Functions

        private void PopulateModsToShow() {
            IEnumerable<Mod> allMods = thunderStore.ModCache.Where(mod => mod.FullName.ToLower().Contains(SearchTerm.ToLower()));

            switch (SelectedModList) {
                case ModListSource.New: allMods = allMods.Where(mod => !userSettings.SeenMods.Contains(mod.ID)); break;
                case ModListSource.Downloaded: allMods = allMods.Where(mod => mod.IsDownloaded); break;
                case ModListSource.NotDownloaded: allMods = allMods.Where(mod => !mod.IsDownloaded); break;
                case ModListSource.Enabled: allMods = allMods.Where(mod => mod.IsDownloaded && mod.IsEnabled); break;
                case ModListSource.Disabled: allMods = allMods.Where(mod => mod.IsDownloaded && !mod.IsEnabled); break;
            }

            switch (SelectedSortOption) {
                case ModListSortOption.LastUpdated: allMods = allMods.OrderByDescending(mod => mod.LastUpdated); break;
                case ModListSortOption.Newest: allMods = allMods.OrderByDescending(mod => mod.DateUploaded); break;
                case ModListSortOption.Alphabetical: allMods = allMods.OrderBy(mod => mod.Name); break;
                case ModListSortOption.Downloads: allMods = allMods.OrderByDescending(mod => mod.Downloads); break;
                case ModListSortOption.Popularity: allMods = allMods.OrderByDescending(mod => mod.Rating); break;
            }

            Application.Current.Dispatcher.Invoke(delegate () {
                ModsToShow.Clear();
                foreach (Mod mod in allMods) {
                    ModsToShow.Add(new ModViewModel(mod, serviceProvider));
                }
            });

            OnPropertyChanged(nameof(ModsToShow));
        }
    }
}
