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
        // Properties

        private Version ProgramVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
        public string Title => $"Techtonica Mod Loader - V{ProgramVersion.Major}.{ProgramVersion.Minor}.{ProgramVersion.Build}";

        [ObservableProperty]
        private bool _modUpdatesAvailable = false;

        public Profile ActiveProfile {
            get => ProfileManager.Instance.ActiveProfile;
            set {
                ProfileManager.Instance.ActiveProfile = value;
                PopulateModsToShow();
            }
        }

        public ObservableCollection<Profile> Profiles => ProfileManager.Instance.ProfilesList;

        [ObservableProperty]
        private ModListSource? _selectedModList = null;
        public Array? ModLists {
            get {
                if (ThunderStore.Instance.Connected) return Settings.UserSettings.DefaultModList.Options;
                else return new ModListSource[] { ModListSource.Downloaded, ModListSource.Enabled, ModListSource.Disabled };
            }
        }

        [ObservableProperty]
        private ModListSortOption? _selectedSortOption = null;

        public Array? SortOptions => Settings.UserSettings.DefaultModListSortOption.Options;

        [ObservableProperty]
        private string _searchTerm = "";

        [ObservableProperty]
        private ObservableCollection<ModViewModel> _modsToShow;

        // Constructors

        public MainViewModel() {
            _modsToShow = new ObservableCollection<ModViewModel>();

            ProfileManager.Instance.PropertyChanged += OnProfileManagerPropertyChanged;
            ThunderStore.Instance.PropertyChanged += OnThunderstorePropertyChanged;
        }

        // Commands

        [RelayCommand]
        private void OpenSettings() {
            DialogService.OpenSettingsDialog();
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
            ThunderStore.Instance.UpdateModCache();
        }

        [RelayCommand]
        private void CreateNewProfile() {
            if(DialogService.GetStringFromUser(out string name, "Enter Profile Name:", "")) {
                ProfileManager.Instance.CreateNewProfile(name);
            }
        }

        [RelayCommand]
        private void DeleteProfile() {
            string name = ProfileManager.Instance.ActiveProfile.Name;
            if(DialogService.GetUserConfirmation("Delete Profile?", $"Are you sure you want to delete the {name} profile?\nThis cannot be undone.")) {
                ProfileManager.Instance.DeleteActiveProfile();
            }
        }

        // Events

        partial void OnSearchTermChanged(string value) {
            PopulateModsToShow();
        }

        partial void OnSelectedModListChanged(ModListSource? value) {
            ModListSource[] onlineLists = new ModListSource[] { ModListSource.All, ModListSource.New, ModListSource.NotDownloaded };

            if(!ThunderStore.Instance.Connected && (SelectedModList == ModListSource.All || SelectedModList == ModListSource.New || SelectedModList == ModListSource.NotDownloaded)) {
                if (MainWindow.current == null) return;

                DialogService.ShowErrorMessage("Not Connected To Thunderstore", "You can't browse online mods while in offline mode");
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
                if (!ThunderStore.Instance.Connected) {
                    SelectedModList = ModListSource.Downloaded;
                }
                else {
                    SelectedModList = Settings.UserSettings.DefaultModList.Value;
                }

                OnPropertyChanged(nameof(SelectedModList));
                OnPropertyChanged(nameof(ModLists));
            }
        }

        // Private Functions

        private void PopulateModsToShow() {
            IEnumerable<ModModel> allMods = ThunderStore.Instance.ModCache.Where(mod => mod.FullName.ToLower().Contains(SearchTerm.ToLower()));

            switch (SelectedModList) {
                case ModListSource.New: allMods = allMods.Where(mod => !Settings.UserSettings?.SeenMods.Value.Contains(mod.ID) ?? true); break;
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
                    ModsToShow.Add(new ModViewModel(mod));
                }
            });

            OnPropertyChanged(nameof(ModsToShow));
        }
    }
}
