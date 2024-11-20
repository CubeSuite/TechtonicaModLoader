using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Mod
{
    public partial class ModViewModel : ObservableObject
    {
        // Members
        private ModModel _mod;

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
            _mod.Downloaded();
        }

        // Constructors

        public ModViewModel(ModModel mod, ProfileManager profileManager) {
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
