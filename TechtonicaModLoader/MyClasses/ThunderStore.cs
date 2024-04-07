using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Modes;
using TechtonicaModLoader.MyClasses.ThunderStoreResponses;

namespace TechtonicaModLoader
{
    public static class ThunderStore
    {
        // Objects & Variables
        private static string baseURL = "https://thunderstore.io/c/techtonica/api/v1";

        // Public Functions

        public static async Task<ThunderStoreMod> GetThunderStoreMod(string id) {
            string endPoint = $"{baseURL}/package/{id}/";
            Log.Debug($"Querying ThunderStore API endpoint: '{endPoint}'");
            string json = await GetApiData(endPoint);
            if (string.IsNullOrEmpty(json)) {
                Log.Debug($"Got no response from ThunderStore");
                return null;
            }

            return JsonConvert.DeserializeObject<ThunderStoreMod>(json);
        }

        public static async Task<Mod> SearchForMod(string fullName) {
            Log.Debug($"Searching for mod: '{fullName}'");
            
            List<ThunderStoreMod> mods = await GetAllThunderStoreMods();
            foreach(ThunderStoreMod mod in mods) {
                foreach(ThunderStoreVerion version in mod.versions) {
                    if(version.full_name == fullName) {
                        Log.Debug($"Found mod '{fullName}'");
                        return new Mod(mod);
                    }
                }
            }

            string error = $"Could not find a mod with full_name= '{fullName}'";
            Log.Error(error);
            DebugUtils.CrashIfDebug(error);
            return null;
        }

        public static async Task<Mod> GetMod(string id) {
            Log.Debug($"Getting mod: '{id}'");
            return new Mod(await GetThunderStoreMod(id));
        }

        public static async Task<List<Mod>> GetAllMods() {
            Log.Debug("Getting all mods from Thunderstore");
            List<ThunderStoreMod> thunderStoreMods = await GetAllThunderStoreMods();
            Log.Debug($"Got {thunderStoreMods.Count} mods from ThunderStore");
            List<Mod> mods = new List<Mod>();
            foreach (ThunderStoreMod thunderStoreMod in thunderStoreMods) {
                mods.Add(new Mod(thunderStoreMod));
            }

            return mods;
        }

        // Private Functions

        private static async Task<string> GetApiData(string apiUrl) {
            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode) {
                    return await response.Content.ReadAsStringAsync();
                }
                else {
                    string error = $"HTTP request failed with status code {response.StatusCode}";
                    Log.Error(error);
                    DebugUtils.CrashIfDebug(error);
                    return "";
                }
            }
        }

        private static async Task<List<ThunderStoreMod>> GetAllThunderStoreMods() {
            string endPoint = $"{baseURL}/package/";
            Log.Debug($"Querying ThunderStore API endpoint: '{endPoint}'");

            string json = await GetApiData(endPoint);
            if (string.IsNullOrEmpty(json)) {
                string error = $"API returned empty JSON";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return new List<ThunderStoreMod>();
            }


            List<ThunderStoreMod> mods = JsonConvert.DeserializeObject<List<ThunderStoreMod>>(json);
            Log.Debug($"Got {mods.Count} mods from ThunderStore");
            
            List<ThunderStoreMod> depricatedMods = mods.Where(mod => mod.is_deprecated).ToList();
            mods = mods.Where(mod => !mod.is_deprecated).ToList();
            mods = mods.Where(mod => mod.name != "r2modman").ToList();
            Log.Debug($"Removed r2modman from results");


            Log.Debug($"Removed {depricatedMods.Count} deprecated mods");

            return mods;
        }
    }
}
