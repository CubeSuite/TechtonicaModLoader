using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using SharpVectors.Dom.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.MVVM.Models;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Services.ThunderstoreModels;

namespace TechtonicaModLoader.Stores
{
    public interface IProfileManager
    {
        Profile ActiveProfile { get; set; }
        ObservableCollection<Profile> ProfilesList { get; }

        event PropertyChangedEventHandler PropertyChanged;

        void AddMod(ThunderStoreMod thunderStoreMod);
        void CreateNewProfile(string name);
        void DeleteActiveProfile();
        void Load();
        void Save();
    }

    public partial class ProfileManager : ObservableObject, IProfileManager
    {
        // Members

        private Dictionary<int, Profile> profiles = new Dictionary<int, Profile>();

        private ILoggerService logger;
        private IDialogService dialogService;
        private IUserSettings userSettings;
        private IProgramData programData;

        // Properties

        public Profile ActiveProfile {
            get => profiles[userSettings.ActiveProfileID];
            set {
                if (value == null) return;
                userSettings.ActiveProfileID = value.Id;
                OnPropertyChanged(nameof(ActiveProfile));
            }
        }

        public ObservableCollection<Profile> ProfilesList { get; } = new ObservableCollection<Profile>();

        // Constructors

        public ProfileManager(IServiceProvider serviceProvider) {
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            dialogService = serviceProvider.GetRequiredService<IDialogService>();
            userSettings = serviceProvider.GetRequiredService<IUserSettings>();
            programData = serviceProvider.GetRequiredService<IProgramData>();

            Load();
        }

        // Public Functions

        public void CreateNewProfile(string name) {
            if (ProfilesList.Select(profile => profile.Name).Contains(name)) {
                if (!dialogService.GetUserConfirmation("Name Taken", $"The profile name '{name}' is already taken, are you sure you want to use it again?")) {
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
            File.WriteAllText(programData.FilePaths.ProfilesFile, json);
        }

        public void Load() {
            if (!File.Exists(programData.FilePaths.ProfilesFile)) {
                CreateDefaultProfiles();
                logger.Warning("Profiles.json not found");
                return;
            }

            string json = File.ReadAllText(programData.FilePaths.ProfilesFile);
            if (string.IsNullOrEmpty(json)) return;

            List<Profile> savedProfiles = JsonConvert.DeserializeObject<List<Profile>>(json ?? "[]") ?? new List<Profile>();
            foreach (Profile profile in savedProfiles) {
                profile.ProfileManager = this;
                AddProfile(profile);
            }

            logger.Info("Profiles loaded");
        }

        private void CreateDefaultProfiles() {
            AddProfile(new Profile(this, "Modded", true));
            AddProfile(new Profile(this, "Development", true));
            AddProfile(new Profile(this, "Vanilla", true));

            userSettings.ActiveProfileID = 0;

            Save();
        }
    }
}
