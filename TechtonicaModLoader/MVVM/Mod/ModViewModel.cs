using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TechtonicaModLoader.Models;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Mod
{
    public partial class ModViewModel : ObservableObject
    {
        // Members
        private readonly ModModel _mod;

        private readonly IProfileManager _profileManager;

        // Properties

        [ObservableProperty] private string _id;
        [ObservableProperty] private string _name;
        [ObservableProperty] private string _tagLine;
        
        [ObservableProperty] private string _iconLink;
        
        [ObservableProperty] private bool _isDownloaded;
        
        [ObservableProperty] private bool _isEnabled;

        // Commands

        [RelayCommand]
        private void DownloadMod() {
            _mod.Download();
        }

        // Constructors

        public ModViewModel(ModModel mod, IProfileManager profileManager) {
            _mod = mod;
            _profileManager = profileManager;

            _id = mod.ID;
            _name = mod.Name;
            _tagLine = mod.TagLine;

            _iconLink = mod.IconLink;
            
            _isDownloaded = mod.IsDownloaded;

            _isEnabled = mod.IsEnabled;

            Settings.UserSettings?.SeenMods.Value.Add(_id);
            Settings.Save();
        }
    }
}
