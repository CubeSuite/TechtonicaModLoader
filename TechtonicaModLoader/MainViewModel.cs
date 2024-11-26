using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
using TechtonicaModLoader.MVVM.Mod;
using TechtonicaModLoader.MVVM.Settings.ViewModels;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Windows;
using TechtonicaModLoader.Windows.Settings;

namespace TechtonicaModLoader.MVVM
{
    public partial class MainViewModel : ObservableObject
    {
        // Members

        private IDialogService dialogService;
        private readonly SettingsWindowViewModel _settingsViewModel;
        private ProfileManager profileManager;
        private ThunderStore thunderStore;

        // Properties

        private Version ProgramVersion => ProgramData.ProgramVersion;
        public string Title => $"Techtonica Mod Loader - V{ProgramVersion.Major}.{ProgramVersion.Minor}.{ProgramVersion.Build}";

        [ObservableProperty] private bool _modUpdatesAvailable = false;
        [ObservableProperty] private ModListSource? _selectedModList = null;
        [ObservableProperty] private ModListSortOption? _selectedSortOption = null;
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
                if (thunderStore.Connected) return _settingsViewModel.DefaultModList.Options;
                else return new ModListSource[] { ModListSource.Downloaded, ModListSource.Enabled, ModListSource.Disabled };
            }
        }

        public Array? SortOptions => _settingsViewModel.DefaultModListSortOption.Options;

        // Constructors

        public MainViewModel(IDialogService dialogService, SettingsWindowViewModel settingsViewModel, ProfileManager profileManager, ThunderStore thunderStore) {
            _modsToShow = new ObservableCollection<ModViewModel>();
            this.dialogService = dialogService;
            this._settingsViewModel = settingsViewModel;
            this.profileManager = profileManager;
            this.thunderStore = thunderStore;

            profileManager.PropertyChanged += OnProfileManagerPropertyChanged;
            thunderStore.PropertyChanged += OnThunderstorePropertyChanged;
        }

        // Commands

        [RelayCommand]
        private void OpenSettings() {
            // ToDo: Move this
            SettingsWindow window = new SettingsWindow(_settingsViewModel, dialogService);
            window.ShowDialog();
        }

        [RelayCommand]
        private void MinimiseProgram() {
            MainWindow.current.WindowState = WindowState.Minimized;
        }

        [RelayCommand]
        private void CloseProgram() {
            Log.Info("Closing program");
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

        partial void OnSelectedModListChanged(ModListSource? value) {
            ModListSource[] onlineLists = new ModListSource[] { ModListSource.All, ModListSource.New, ModListSource.NotDownloaded };

            if(!thunderStore.Connected && (SelectedModList == ModListSource.All || SelectedModList == ModListSource.New || SelectedModList == ModListSource.NotDownloaded)) {
                if (MainWindow.current == null) return;

                dialogService.ShowErrorMessage("Not Connected To Thunderstore", "You can't browse online mods while in offline mode");
                SelectedModList = ModListSource.Downloaded;
            }

            PopulateModsToShow();
        }

        partial void OnSelectedSortOptionChanged(ModListSortOption? value) {
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
                    SelectedModList = _settingsViewModel.DefaultModList.Value;
                }

                OnPropertyChanged(nameof(SelectedModList));
                OnPropertyChanged(nameof(ModLists));
            }
        }

        // Private Functions

        private void PopulateModsToShow() {
            IEnumerable<ModModel> allMods = thunderStore.ModCache.Where(mod => mod.FullName.ToLower().Contains(SearchTerm.ToLower()));

            switch (SelectedModList) {
                case ModListSource.New: allMods = allMods.Where(mod => !_settingsViewModel.SeenMods.Value.Contains(mod.ID)); break;
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
                foreach (ModModel mod in allMods) {
                    ModsToShow.Add(new ModViewModel(mod, profileManager));
                }
            });

            OnPropertyChanged(nameof(ModsToShow));
        }
    }
}
