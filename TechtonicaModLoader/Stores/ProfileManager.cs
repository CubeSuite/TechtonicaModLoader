using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
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
using TechtonicaModLoader.Services.ThunderstoreModels;

namespace TechtonicaModLoader.Stores
{
    public partial class ProfileManager : ObservableObject
    {
        // Members

        private Dictionary<int, Profile> profiles = new Dictionary<int, Profile>();
        
        private IDialogService dialogService;
        private UserSettings userSettings;

        // Properties

        public Profile ActiveProfile {
            get => profiles[userSettings.ActiveProfileID.Value];
            set {
                if (value == null) return;
                userSettings.ActiveProfileID.Value = value.Id;
                OnPropertyChanged(nameof(ActiveProfile));
            }
        }

        public ObservableCollection<Profile> ProfilesList { get; } = new ObservableCollection<Profile>();

        // Constructors

        public ProfileManager(IDialogService dialogService, UserSettings userSettings) {
            this.dialogService = dialogService;
            this.userSettings = userSettings;
        }

        // Public Functions

        public void CreateNewProfile(string name) {
            if(ProfilesList.Select(profile => profile.Name).Contains(name)) {
                if(!dialogService.GetUserConfirmation("Name Taken", $"The profile name '{name}' is already taken, are you sure you want to use it again?")) {
                    return;
                }
            }

            Profile profile = new Profile(this, name);
            int id = AddProfile(profile);
            ActiveProfile = profiles[id];
            Save();
        }

        public void AddMod(ThunderStoreMod thunderStoreMod) {
            foreach (Profile profile in ProfilesList) {
                profile.AddMod(thunderStoreMod);
            }

            ActiveProfile.ToggleMod(thunderStoreMod.uuid4);
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

        public void Save() {
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
                profile.ProfileManager = this;
                AddProfile(profile);
            }
        }

        private void CreateDefaultProfiles() {
            AddProfile(new Profile(this, "Modded", true));
            AddProfile(new Profile(this, "Development", true));
            AddProfile(new Profile(this, "Vanilla", true));

            userSettings.ActiveProfileID.Value = 0;

            Save();
        }
    }

    public partial class Profile : ObservableObject
    {
        // Members

        ProfileManager _profileManager;

        // Properties

        [ObservableProperty] private int _id = -1;
        [ObservableProperty] private string _name;
        [ObservableProperty] private Dictionary<string, bool> _modEnabledStates;

        private bool _isDefault;
        public bool IsDefault => _isDefault;

        [JsonIgnore] public ProfileManager ProfileManager {
            get => _profileManager;
            set => _profileManager = value;
        }

        // Constructors

        public Profile(ProfileManager profileManager, string name, bool isDefault = false) {
            this._profileManager = profileManager;
            _name = name;
            _isDefault = IsDefault;
            _modEnabledStates = new Dictionary<string, bool>();
        }

        // Public Functions

        public bool IsModEnabled(ModModel mod) {
            if (_profileManager.ActiveProfile.Name == "Vanilla") return false;

            if (!ModEnabledStates.ContainsKey(mod.ID)) return false;
            return ModEnabledStates[mod.ID];
        }

        public void AddMod(ThunderStoreMod mod) {
            ModEnabledStates.Add(mod.uuid4, false); // ToDo: Error handle
            _profileManager.Save();
        }

        public void ToggleMod(string id) {
            ModEnabledStates[id] = !ModEnabledStates[id];
            _profileManager.Save();
        }

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
