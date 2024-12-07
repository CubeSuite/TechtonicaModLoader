using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using TechtonicaModLoader.MVVM.Models;
using TechtonicaModLoader.MVVM.ViewModels;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

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
        private SortedDictionary<ModListSource, string>? offlineModListSourceStrings = null;

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

        public IReadOnlyCollection<KeyValuePair<ModListSource, string>> ModLists {
            get {
                if (thunderStore.Connected) return ModListSourceDisplay.Strings;
                else {
                    offlineModListSourceStrings ??= new() {
                            { ModListSource.Downloaded, ModListSourceDisplay.Strings[ModListSource.Downloaded] },
                            { ModListSource.Enabled, ModListSourceDisplay.Strings[ModListSource.Enabled] },
                            { ModListSource.Disabled, ModListSourceDisplay.Strings[ModListSource.Disabled] },
                        };
                    return offlineModListSourceStrings;
                }
            }
        }

        public IReadOnlyCollection<KeyValuePair<ModListSortOption, string>> SortOptions => ModListSortOptionDisplay.Strings;

        public bool DeployNeeded => userSettings.DeployNeeded;
        public string LaunchButtonText => DeployNeeded ? StringResources.DeployAndLaunchGame : StringResources.LaunchGame;
        public string UpdateAllButtonText => StringResources.UpdateAllButtonText;
        public string CheckForUpdatesButtonText => StringResources.CheckForUpdatesButtonText;
        public string DeployButtonText => StringResources.DeployButtonText;
        public string ProfileLabelText => StringResources.ProfileLabelText;
        public string ModListLabelText => StringResources.ModListLabelText;
        public string SortByLabelText => StringResources.SortByLabelText;
        public string SearchBarDefaultText => StringResources.SearchBarDefaultText;

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
            if(dialogService.GetStringFromUser(out string name, StringResources.NewProfileTitle, "")) {
                profileManager.CreateNewProfile(name);
            }
        }

        [RelayCommand]
        private void DeleteProfile() {
            string name = profileManager.ActiveProfile.Name;
            if(dialogService.GetUserConfirmation(StringResources.DeleteProfileTitle, string.Format(StringResources.DeleteProfileMessage, name))) {
                profileManager.DeleteActiveProfile();
            }
        }

        // Events

        partial void OnSearchTermChanged(string value) {
            PopulateModsToShow();
        }

        partial void OnSelectedModListChanged(ModListSource value) {
            if(!thunderStore.Connected && (SelectedModList == ModListSource.All || SelectedModList == ModListSource.New || SelectedModList == ModListSource.NotDownloaded)) {
                if (MainWindow.current == null) return;

                dialogService.ShowErrorMessage(StringResources.ThunderstoreOfflineTitle, StringResources.ThunderstoreOfflineMessage);
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
