using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechtonicaModLoader.MVVM.Models;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.ViewModels
{
    public partial class ModViewModel : ObservableObject
    {
        // Members
        private Mod _mod;

        private IProfileManager profileManager;
        private IThunderStore thunderStore;
        private IProgramData programData;

        // Properties

        [ObservableProperty] private string _downloadSource;
        [ObservableProperty] private string _thumbSource;
        [ObservableProperty] private string _updateSource;
        [ObservableProperty] private string _donateSource;
        [ObservableProperty] private string _configureSource;
        [ObservableProperty] private string _viewModPageSource;
        [ObservableProperty] private string _deleteSource;

        [ObservableProperty] private string _id;
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _tagLine;
        [ObservableProperty] private int _downloads;
        [ObservableProperty] private int _rating;
        
        [ObservableProperty] private string _iconLink;
        [ObservableProperty] private string _donationLink;

        [ObservableProperty] private bool _allowDownload;
        [ObservableProperty] private bool _isDownloaded;
        
        [ObservableProperty] private bool _isEnabled;
        [ObservableProperty] private bool _updateAvailable;
        [ObservableProperty] private bool _hasConfigFile;

        public bool HasDonationLink => !string.IsNullOrEmpty(DonationLink);

        public bool AllowToggling => string.Equals(profileManager.ActiveProfile.Name, StringResources.ProfileVanilla, StringComparison.CurrentCulture);
        // ToDo: disable toggling for dependent mods

        // Constructors

        public ModViewModel(Mod mod, IServiceProvider serviceProvider) {
            profileManager = serviceProvider.GetRequiredService<IProfileManager>();
            thunderStore = serviceProvider.GetRequiredService<IThunderStore>();
            programData = serviceProvider.GetRequiredService<IProgramData>();

            _downloadSource = $"{programData.FilePaths.ResourcesFolder}\\ModPanel\\Download.svg";
            _thumbSource = $"{programData.FilePaths.ResourcesFolder}\\ModPanel\\Thumb.svg";
            _updateSource = $"{programData.FilePaths.ResourcesFolder}\\ModPanel\\Update.svg";
            _donateSource = $"{programData.FilePaths.ResourcesFolder}\\ModPanel\\Donate.svg";
            _configureSource = $"{programData.FilePaths.ResourcesFolder}\\ModPanel\\Configure.svg";
            _viewModPageSource = $"{programData.FilePaths.ResourcesFolder}\\ModPanel\\ViewModPage.svg";
            _deleteSource = $"{programData.FilePaths.ResourcesFolder}\\ModPanel\\Delete.svg";

            _mod = mod;
            _mod.IsDownloadingChanged += OnModModelIsDownloadingChanged;

            _id = mod.ID;
            _name = mod.Name;
            _tagLine = mod.TagLine;
            _downloads = mod.Downloads;
            _rating = mod.Rating;

            _iconLink = mod.IconLink;
            _donationLink = mod.DonationLink;

            AllowDownload = !thunderStore?.IsModDownloading(mod.FullName) ?? true;
            _isDownloaded = mod.IsDownloaded;

            _isEnabled = _isDownloaded ? mod.IsEnabled : false;
            _updateAvailable = mod.UpdateAvailable;
            _hasConfigFile = mod.HasConfigFile;

            if (!programData.ModsSeenThisSession.Contains(mod.ID)) {
                programData.ModsSeenThisSession.Add(mod.ID);
            }
        }

        // Events

        partial void OnIsEnabledChanged(bool value) {
            profileManager.ActiveProfile.ToggleMod(_mod.ID);
        }

        private void OnModModelIsDownloadingChanged() {
            AllowDownload = !_mod.IsDownloading;
            OnPropertyChanged(nameof(AllowDownload));
        }

        // Commands

        [RelayCommand]
        private void Download() {
            AllowDownload = false;
            _mod.Download();
        }

        [RelayCommand]
        private void UpdateMod() {

        }

        [RelayCommand]
        private void Donate() {

        }

        [RelayCommand]
        private void ConfigureMod() {

        }

        [RelayCommand]
        private void ViewModPage() {

        }

        [RelayCommand]
        private void DeleteMod() {

        }
    }
}
