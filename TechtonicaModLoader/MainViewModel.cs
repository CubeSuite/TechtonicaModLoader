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
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Windows;

namespace TechtonicaModLoader.MVVM
{
    public partial class MainViewModel : ObservableObject
    {
        // Members

        private ProfileManager _profileManager;
        private IDialogService _dialogService;
        private Thunderstore _thunderStore;

        // Properties

        private Version ProgramVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
        public string Title => $"Techtonica Mod Loader - V{ProgramVersion.Major}.{ProgramVersion.Minor}.{ProgramVersion.Build}";

        [ObservableProperty]
        private bool _modUpdatesAvailable = false;

        public Profile ActiveProfile {
            get => _profileManager.ActiveProfile;
            set {
                _profileManager.ActiveProfile = value;
                PopulateModsToShow();
            }
        }

        public ObservableCollection<Profile> Profiles => _profileManager.ProfilesList;

        [ObservableProperty]
        private ModListSource? _selectedModList = null;
        public Array? ModLists => Settings.UserSettings.DefaultModList.Options;

        [ObservableProperty]
        private ModListSortOption? _selectedSortOption = null;

        public Array? SortOptions => Settings.UserSettings.DefaultModListSortOption.Options;

        [ObservableProperty]
        private string _searchTerm;

        [ObservableProperty]
        private ObservableCollection<ModViewModel> _modsToShow;

        // Constructors

        public MainViewModel(ProfileManager profileManager, IDialogService dialogService, Thunderstore thunderstore) {
            _profileManager = profileManager;
            _dialogService = dialogService;
            _thunderStore = thunderstore;

            _modsToShow = new ObservableCollection<ModViewModel>();

            _profileManager.PropertyChanged += OnProfileManagerPropertyChanged;
            _thunderStore.PropertyChanged += OnThunderstorePropertyChanged;
        }

        // Commands

        [RelayCommand]
        private void OpenSettings() {
            _dialogService.OpenSettingsDialog();
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
            // ToDo: CheckForModUpdates()
        }

        [RelayCommand]
        private void CreateNewProfile() {
            _dialogService.ShowInfoMessage("Test", "This is a test message", "Close Test Box");
            if(_dialogService.GetStringFromUser(out string name, "Enter Profile Name:", "")) {
                _profileManager.CreateNewProfile(name);
            }
        }

        [RelayCommand]
        private void DeleteProfile() {
            string name = _profileManager.ActiveProfile.Name;
            if(_dialogService.GetUserConfirmation("Delete Profile?", $"Are you sure you want to delete the {name} profile?\nThis cannot be undone.")) {
                _profileManager.DeleteActiveProfile();
            }
        }

        // Events

        

        partial void OnSelectedModListChanged(ModListSource? value) {
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
            if(e.PropertyName == nameof(Thunderstore.ModCache)) {
                PopulateModsToShow();
            }
        }

        // Private Functions

        private void PopulateModsToShow() {
            IEnumerable<ModModel> allMods = _thunderStore.ModCache;
            switch (SelectedModList) {
                case ModListSource.New: allMods = allMods.Where(mod => !Settings.UserSettings?.SeenMods.Value.Contains(mod.ID) ?? true); break;
                case ModListSource.Installed: allMods = allMods.Where(mod => mod.IsDownloaded); break;
                case ModListSource.NotInstalled: allMods = allMods.Where(mod => !mod.IsDownloaded); break;
                case ModListSource.Enabled: allMods = allMods.Where(mod => mod.IsDownloaded && mod.IsEnabled); break;
                case ModListSource.Disabled: allMods = allMods.Where(mod => mod.IsDownloaded && !mod.IsEnabled); break;
            }

            ModsToShow.Clear();
            foreach(ModModel mod in allMods) {
                ModsToShow.Add(new ModViewModel(mod, _profileManager));
            }

            OnPropertyChanged(nameof(ModsToShow));
        }
    }
}
