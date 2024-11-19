using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Modes;
using TechtonicaModLoader.MyClasses;
using TechtonicaModLoader.MyClasses.ThunderStoreResponses;

namespace TechtonicaModLoader
{
    public static class ModManager
    {
        // Objects & Variables

        // The release of 1.0, used to filter mods that are likely broken.
        internal static readonly DateTime techtonicaReleaseDate = new DateTime(2024, 11, 7);
        internal static HashSet<string> allowedMods = new HashSet<string> { "BepInExPack", "UnityAudio" };

        private static Dictionary<string, Mod> mods = new Dictionary<string, Mod>();

        // Public Functions

        public static void AddMod(Mod mod) {
            if (!DoesModExist(mod)) {
                mods.Add(mod.id, mod);
            }
            else {
                Log.Warning($"Skipped adding known mod - {mod.name}");
            }
        }

        public static void UpdateMod(Mod mod) {
            if (DoesModExist(mod)) {
                mods[mod.id] = mod;
                Log.Debug($"Updated mod with id '{mod.id}'");
            }
            else {
                Log.Error($"Could not update unknown mod with id '{mod.id}'");
            }
        }

        public static bool DoesModExist(string id) {
            return mods.ContainsKey(id);
        }

        public static Mod GetMod(string id) {
            if (DoesModExist(id)) {
                return mods[id];
            }
            else {
                Log.Error($"Could not get unknown mod with id - {id}");
                return null;
            }
        }

        public static List<Mod> GetAllMods() {
            return mods.Values.ToList();
        }

        public static int getModsCount() {
            return mods.Count;
        }

        public static void DeleteMod(string id) {
            if (DoesModExist(id)) {
                mods.Remove(id);
                Log.Debug($"Deleted mod with id '{id}'");
            }
            else {
                Log.Error($"Could not delete mod with unknown id - {id}");
            }
        }

        public static List<Mod> SortModList(List<Mod> modsToSort, ModListSortOption sortOption) {
            switch (sortOption) {
                case ModListSortOption.Alphabetical: return modsToSort.OrderBy(mod => mod.name).ToList();
                case ModListSortOption.LastUpdated: return modsToSort.OrderByDescending(mod => mod.dateUpdated).ToList();
                case ModListSortOption.Downloads: return modsToSort.OrderByDescending(mod => mod.downloads).ToList();
                case ModListSortOption.Popularity: return modsToSort.OrderByDescending(mod => mod.ratingScore).ToList();
            }

            string error = $"Could not sort mods with unknown sort option: '{StringUtils.GetModListSortOptionName(sortOption)}'";
            Log.Error(error);
            DebugUtils.CrashIfDebug(error);
            return modsToSort;
        }

        public static async Task<bool> CheckForUpdates() {
            bool anyModNeedsUpdate = false;
            Log.Debug($"Checking for mod updates");
            for(int i =  0; i < mods.Count; i++) {
                Mod mod = mods.Values.ToList()[i];
                if (mod.id == ProgramData.bepInExID) continue;

                Log.Debug($"Checking '{mod.name}' for updates");

                ThunderStoreMod thunderStoreMod = await ThunderStore.GetThunderStoreMod(mod.id);
                if (thunderStoreMod == null) continue;

                ModVersion latestVersion = ModVersion.Parse(thunderStoreMod.versions[0].version_number);
                mod.updateAvailable = latestVersion > mod.version;
                if (mod.updateAvailable) anyModNeedsUpdate = true;

                mod.name = thunderStoreMod.name;
                mod.author = thunderStoreMod.owner;
                mod.dateUpdated = DateTime.Parse(thunderStoreMod.date_updated);
                mod.ratingScore = thunderStoreMod.rating_score;
                mod.isDeprecated = thunderStoreMod.is_deprecated;
                mod.categories = thunderStoreMod.categories;
                mod.link = thunderStoreMod.package_url;
                mod.donationLink = thunderStoreMod.donation_link;

                UpdateMod(mod);

                Log.Info($"Updated info for mod '{mod.name}'");
            }

            return anyModNeedsUpdate;
        }

        // Data Functions

        public static void Save() {
            string json = JsonConvert.SerializeObject(mods.Values.ToList());
            File.WriteAllText(ProgramData.FilePaths.modsSaveFile, json);
        }

        public static void Load() {
            if (File.Exists(ProgramData.FilePaths.modsSaveFile)) {
                string json = File.ReadAllText(ProgramData.FilePaths.modsSaveFile);
                List<Mod> mods = JsonConvert.DeserializeObject<List<Mod>>(json);
                foreach (Mod mod in mods) {
                    if (!string.IsNullOrEmpty(mod.configFileLocation) && mod.configFileLocation.Contains(ProgramData.FilePaths.bepInExConfigFolder)) {
                        mod.configFileLocation = mod.configFileLocation.Replace(ProgramData.FilePaths.bepInExConfigFolder, ProgramData.FilePaths.configsFolder);
                    }

                    // Only add mods that are newer than the 1.0 release date or on the allowed list.
                    if (mod.dateUpdated >= techtonicaReleaseDate || allowedMods.Contains(mod.name)) {
                        AddMod(mod);
                    }
                    else {
                        File.Delete(mod.zipFileLocation);
                        DeleteMod(mod);
                    }
                }
            }
        }

        #region Overloads

        // Public Functions

        public static bool DoesModExist(Mod mod) {
            return DoesModExist(mod.id);
        }

        public static void DeleteMod(Mod mod) {
            DeleteMod(mod.id);
        }

        #endregion
    }
}
