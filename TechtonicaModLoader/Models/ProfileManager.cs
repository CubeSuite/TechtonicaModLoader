using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.Models
{
    public partial class ProfileManager : ObservableObject, IProfileManager
    {
        // Members
        private readonly Dictionary<int, Profile> profiles = new();

        // Properties

        public Profile ActiveProfile {
            get => profiles[Settings.UserSettings.ActiveProfileID.Value];
            set {
                if (value == null) return;
                Settings.UserSettings.ActiveProfileID.Value = value.Id;
                OnPropertyChanged(nameof(ActiveProfile));
            }
        }

        public ObservableCollection<Profile> ProfilesList { get; } = new();

        // Public Functions

        public void CreateNewProfile(string name) {
            if (ProfilesList.Select(profile => profile.Name).Contains(name)) {
                throw new ArgumentException($"The profile '{name}' already exists.");
            }

            Profile profile = new(name);
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

            List<Profile> savedProfiles = JsonConvert.DeserializeObject<List<Profile>>(json ?? "[]") ?? new();
            foreach (Profile profile in savedProfiles) {
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
}
