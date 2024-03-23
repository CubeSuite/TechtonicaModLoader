using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Modes;
using TechtonicaModLoader.MyClasses;

namespace TechtonicaModLoader
{
    public static class ProfileManager
    {
        // Objects & Variables
        private static int activeProfile;
        private static Dictionary<int, Profile> profiles = new Dictionary<int, Profile>();

        // Public Functions

        public static void AddProfile(Profile profile) {
            if (profile.id == -1) profile.id = GetNewProfileID();
            if (DoesProfileExist(profile)) {
                Log.Error($"Cannot add profile '{profile.name}' - id is already in use");
                return;
            }

            profiles[profile.id] = profile;
            Log.Debug($"Added profile '{profile.name}'");
        }
        
        public static void UpdateProfile(Profile profile) {
            if (!DoesProfileExist(profile)) {
                Log.Error($"Cannot update profile '{profile.name}' - Unknown id: '{profile.id}'");
                return;
            }

            profiles[profile.id] = profile;
            Log.Debug($"Updated profile '{profile.name}'");
        } 

        public static void LoadProfile(int id) {
            activeProfile = id;
        }

        public static Profile GetProfile(int id) {
            if (!DoesProfileExist(id)) {
                Log.Error($"Cannot get profile with unknown id: '{id}'");
                return null;
            }

            return profiles[id];
        }

        public static Profile GetProfileByName(string name) {
            foreach(Profile profile in profiles.Values) {
                if(profile.name == name) {
                    return profile;
                }
            }

            Log.Error($"Could not get unknown profile - '{name}'");
            return null;
        }

        public static Profile GetActiveProfile() {
            if (DoesProfileExist(activeProfile)) {
                return profiles[activeProfile];
            }
            else {
                string error = $"Cannot retrieve active profile (id: {activeProfile}), does not exist.";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return new Profile();
            }
        }

        public static List<Profile> GetAllProfiles() {
            return profiles.Values.ToList();
        }

        public static List<string> GetAllProfileNames() {
            return GetAllProfiles().Select(profile => profile.name).ToList();
        }

        public static bool DoesProfileExist(int id) {
            return profiles.ContainsKey(id);
        }

        // Private Functions

        private static int GetNewProfileID() {
            if(profiles.Count == 0) return 0;
            else return profiles.Keys.Max() + 1;
        }

        // Data Functions

        public static void Save() {
            string json = JsonConvert.SerializeObject(GetAllProfiles());
            File.WriteAllText(ProgramData.FilePaths.profilesSaveFile, json);
        }

        public static async Task<string> Load() {
            if(!File.Exists(ProgramData.FilePaths.profilesSaveFile)) {
                Save();
                return "";
            }

            string json = File.ReadAllText(ProgramData.FilePaths.profilesSaveFile);
            List<Profile> profilesFromFile = JsonConvert.DeserializeObject<List<Profile>>(json);
            foreach(Profile profile in profilesFromFile) {
                AddProfile(profile);
            }

            return "";
        }

        public static async Task<string> CreateDefaultProfiles() {
            Log.Debug($"Creating default profiles");
            
            Mod bepInEx = await ThunderStore.GetMod(ProgramData.bepInExID);
            ModManager.AddMod(bepInEx);
            Log.Debug($"Got BepInEx");

            Log.Debug($"Got Unity Explorer");

            Profile modded = new Profile() { name = "Modded" };
            modded.AddMod(ProgramData.bepInExID);
            
            Profile development = new Profile() { name = "Development" };
            development.AddMod(ProgramData.bepInExID);

            Profile vanilla = new Profile() { name = "Vanilla" };

            AddProfile(modded);
            AddProfile(development);
            AddProfile(vanilla);

            LoadProfile(modded);
            Log.Debug($"Created default profiles");
            await bepInEx.DownloadAndInstall();

            if (!Directory.Exists(ProgramData.FilePaths.bepInExPluginsFolder)) {
                GuiUtils.ShowInfoMessage("Launching Game", "Techtonica needs to run once with only BepInEx installed. Quit at the title screen and continue installing mods.");
                FileStructureUtils.StartGame();
            }

            return "";

        }

        #region Overloads

        // Public Functions

        public static bool DoesProfileExist(Profile profile) {
            return DoesProfileExist(profile.id);
        }

        public static void LoadProfile(Profile profile) {
            LoadProfile(profile.id);
        }

        #endregion
    }
}
