using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using TechtonicaModLoader.MVVM.Models;
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

        // Properties

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

        public bool AllowToggling => profileManager.ActiveProfile.Name != "Vanilla";
        // ToDo: disable toggling for dependent mods

        // Constructors

        public ModViewModel(Mod mod, IProfileManager profileManager, IThunderStore thunderStore) {
            this.profileManager = profileManager;
            this.thunderStore = thunderStore;
            
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

            if (!ProgramData.ModsSeenThisSession.Contains(mod.ID)) {
                ProgramData.ModsSeenThisSession.Add(mod.ID);
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
