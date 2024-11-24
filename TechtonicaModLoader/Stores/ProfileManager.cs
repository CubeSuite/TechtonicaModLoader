using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using SharpVectors.Dom.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.MVVM.Mod;
using TechtonicaModLoader.Services;

namespace TechtonicaModLoader.Stores
{
    public partial class ProfileManager : ObservableObject
    {
        // Members

        [ObservableProperty]
        private static ProfileManager _instance;

        private Dictionary<int, Profile> profiles = new Dictionary<int, Profile>();
        private IDialogService _dialogService;

        // Properties

        public Profile ActiveProfile {
            get => profiles[Settings.UserSettings.ActiveProfileID.Value];
            set {
                if (value == null) return;
                Settings.UserSettings.ActiveProfileID.Value = value.Id;
                OnPropertyChanged(nameof(ActiveProfile));
            }
        }

        public ObservableCollection<Profile> ProfilesList { get; } = new ObservableCollection<Profile>();

        // Constructors

        public ProfileManager(IDialogService dialogService) {
            _instance = this;
            _dialogService = dialogService;
        }

        // Public Functions

        public void CreateNewProfile(string name) {
            if(ProfilesList.Select(profile => profile.Name).Contains(name)) {
                if(!_dialogService.GetUserConfirmation("Name Taken", $"The profile name '{name}' is already taken, are you sure you want to use it again?")) {
                    return;
                }
            }

            Profile profile = new Profile(name);
            int id = AddProfile(profile);
            ActiveProfile = profiles[id];
            Save();
        }

        public void DeleteActiveProfile() {
            ProfilesList.Remove(ActiveProfile);
            
            int activeProfileId = ActiveProfile.Id;
            ActiveProfile = profiles[0];
            profiles.Remove(activeProfileId);
            Save();
        }

        // Private Functions
        
        private int AddProfile(Profile profile) {
            if (profile.Id == -1) profile.Id = GetNewProfileID();
            profiles.Add(profile.Id, profile);
            ProfilesList.Add(profile);
            return profile.Id;
        }

        private int GetNewProfileID() {
            if (profiles.Count == 0) return 0;
            else return profiles.Keys.Max() + 1;
        }

        // Data Functions

        private void Save() {
            string json = JsonConvert.SerializeObject(profiles.Values, Formatting.Indented);
            File.WriteAllText(ProgramData.FilePaths.profilesSaveFile, json);
        }

        public void Load() {
            if (!File.Exists(ProgramData.FilePaths.profilesSaveFile)) {
                CreateDefaultProfiles();
                return;
            }

            string json = File.ReadAllText(ProgramData.FilePaths.profilesSaveFile);
            if (string.IsNullOrEmpty(json)) return;

            List<Profile> savedProfiles = JsonConvert.DeserializeObject<List<Profile>>(json ?? "[]") ?? new List<Profile>();
            foreach(Profile profile in savedProfiles) {
                AddProfile(profile);
            }
        }

        private void CreateDefaultProfiles() {
            AddProfile(new Profile("Modded", true));
            AddProfile(new Profile("Development", true));
            AddProfile(new Profile("Vanilla", true));

            Settings.UserSettings.ActiveProfileID.Value = 0;

            Save();
        }
    }

    public partial class Profile : ObservableObject
    {
        // Members

        [ObservableProperty]
        private int _id = -1;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private Dictionary<string, bool> _modEnabledStates;

        private bool _isDefault;
        public bool IsDefault => _isDefault;

        // Constructors

        public Profile(string name, bool isDefault = false) {
            _name = name;
            _isDefault = IsDefault;
            _modEnabledStates = new Dictionary<string, bool>();
        }

        // Public Functions

        public bool IsModEnabled(ModModel mod) {
            if (!ModEnabledStates.ContainsKey(mod.ID)) {
                Log.Error($"Tried to get enabled state for mod ({mod.Name}) that isn't in profile {this}");
                return false;
            }

            return ModEnabledStates[mod.ID];
        }

        // Private Functions

        // Overrides

        public override string ToString() {
            return $"Profile #{Id} '{Name}'";
        }

        public override bool Equals(object? obj) {
            if (obj is null) return false;
            if (obj is not Profile) return false;
            return ((Profile)obj).Id == Id;
        }
    }
}
