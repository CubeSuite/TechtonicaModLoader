using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.MVVM.Mod;
using TechtonicaModLoader.Services.ThunderstoreModels;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.Services
{
    public partial class Thunderstore : ObservableObject
    {
        // Members

        private ProfileManager _profileManager;

        private List<ModModel> _modCache = new List<ModModel>();
        private const string baseURL = "https://thunderstore.io/c/techtonica/api/v1";
        private string? lastJson;

        internal static readonly HashSet<string> allowedMods = new HashSet<string> {
            "BepInExPack",
            "Helium",
            "LongStackInserters",
            "Official_BepInEx_ConfigurationManager",
            "UnityAudio",
            "UnityExplorer",
        };

        internal static readonly HashSet<string> disallowedMods = new HashSet<string> {
            "r2modman",
            "GaleModManager",
        };

        // Properties

        public ObservableCollection<ModModel> ModCache { get; } = new ObservableCollection<ModModel>();

        // Constructors

        public Thunderstore(ProfileManager profileManager) 
        {
            _profileManager = profileManager;
            StartUpdateThread();
        }

        // Public Functions

        public async void StartUpdateThread() {
            await Task.Run(async () => {
                while (true) {
                    UpdateModCache();
                    await Task.Delay(60000);
                }
            });
        }

        public async void UpdateModCache() {
            Log.Debug("Fetching update from ThunderStore");
            IEnumerable<ThunderStoreMod>? thunderstoreMods = await GetAllThunderstoreMods();
            if (thunderstoreMods == null) return;

            ModCache.Clear();
            foreach(ThunderStoreMod mod in thunderstoreMods) {
                ModCache.Add(new ModModel(mod, _profileManager));
            }

            OnPropertyChanged(nameof(ModCache));
        }

        // Private Functions

        private async Task<string> GetApiData(string apiUrl) {
            try {
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
            catch (Exception e) {
                Log.Error($"Error occurred while getting API data: {e.Message}");
                return "";
            }
        }

        private async Task<IEnumerable<ThunderStoreMod>?> GetAllThunderstoreMods() {
            string endPoint = $"{baseURL}/package/";
            Log.Debug($"Querying ThunderStore API endpoint: '{endPoint}'");

            string json = await GetApiData(endPoint);
            if (string.IsNullOrEmpty(json)) {
                Log.Warning("Couldn't get mods from ThunderStore");
                return null;
            }

            if (json == lastJson) return null;
            lastJson = json;

            List<ThunderStoreMod> thunderstoreMods = JsonConvert.DeserializeObject<List<ThunderStoreMod>>(json) ?? new List<ThunderStoreMod>();
            if(thunderstoreMods.Count() == 0) {
                Log.Warning($"ThunderStore returned no mods from API call");
                return null;
            }

            thunderstoreMods = thunderstoreMods.Where(mod => mod.PassesFilterChecks()).ToList();

            return thunderstoreMods;
        }
    }
}
