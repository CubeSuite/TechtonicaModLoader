using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Xml.Linq;
using TechtonicaModLoader.MVVM.Mod;
using TechtonicaModLoader.Services.ThunderstoreModels;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.Services
{
    public partial class ThunderStore : ObservableObject
    {
        // Members
        private IEnumerable<ThunderStoreMod> thunderStoreModCache = new List<ThunderStoreMod>();
        private Dictionary<string, ModVersion> downloadedMods = new Dictionary<string, ModVersion>();
        private List<string> downloadQueue = new List<string>();
        private const string baseURL = "https://thunderstore.io/c/techtonica/api/v1";
        private string? lastJson;
        
        private IDialogService dialogService;
        private ProfileManager profileManager;

        private List<ModModel> _modCache = new List<ModModel>();

        // Properties

        [ObservableProperty] private bool _connected = true;

        public ObservableCollection<ModModel> ModCache { get; } = new ObservableCollection<ModModel>();

        // Constructors

        public ThunderStore(IDialogService dialogService, ProfileManager profileManager) 
        {
            this.dialogService = dialogService;
            this.profileManager = profileManager;
            StartUpdateThread();
            StartDownloadThread();
        }

        // Events

        partial void OnConnectedChanging(bool value) {
            if(!Connected && value) {
                dialogService.ShowInfoMessage("Reconnected To Thunderstore", "Connection To ThunderStore has been restored, you can now browse online mods again");
            }
        }

        // Public Functions

        public async void UpdateModCache(bool fetchUpdate = true) {
            if(fetchUpdate) await FetchModsList();

            Application.Current.Dispatcher.Invoke(delegate () {
                ModCache.Clear();
                foreach (ThunderStoreMod mod in thunderStoreModCache) {
                    ModCache.Add(new ModModel(mod, this, profileManager));
                }

                OnPropertyChanged(nameof(ModCache));
            });
        }

        public void DownloadMod(string fullName) {
            downloadQueue.Add(fullName);
        }

        public bool IsModDownloaded(string id, ModVersion version) {
            if (!downloadedMods.ContainsKey(id)) return false;
            if (downloadedMods[id] < version) return false;
            return true;
        }

        public bool SearchForMod(string fullName, out ThunderStoreMod? mod) {
            if(thunderStoreModCache == null || thunderStoreModCache.Count() == 0) {
                Log.Error($"ThunderStore.SearchForMod() called before thunderStoreModCache was populated");
                mod = null;
                return false;
            }

            mod = thunderStoreModCache.Where(cachedMod => cachedMod.AllVersionNames.Contains(fullName)).FirstOrDefault();
            return true;
        }

        // Private Functions

        private async void StartUpdateThread() {
            await Task.Run(async () => {
                while (true) {
                    UpdateModCache();
                    await Task.Delay(60000);
                }
            });
        }

        private async void StartDownloadThread() {
            await Task.Run(async () => {
                while (true) {
                    if(downloadQueue.Count > 0) {
                        await DownloadZipFile(downloadQueue[0]);
                        // ToDo: Handle false return
                        downloadQueue.RemoveAt(0);
                    }
                    await Task.Delay(250);
                }
            });
        }

        private async Task FetchModsList() {
            Log.Debug("Fetching update from ThunderStore");
            IEnumerable<ThunderStoreMod>? thunderstoreMods = await GetAllThunderstoreMods();
            if (thunderstoreMods == null) return;

            thunderStoreModCache = thunderstoreMods;
            Save();
        } 

        private async Task<bool> DownloadZipFile(string fullName) {
            ThunderStoreMod? thunderStoreMod = thunderStoreModCache.Where(mod => mod.AllVersionNames.Contains(fullName)).FirstOrDefault();
            if(thunderStoreMod == null) {
                string error = $"Couldn't find mod '{fullName}' in thunderStoreModCache";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                dialogService.ShowErrorMessage($"Couldn't Download {fullName}", "An error occured while trying to download this mod.\nPlease click the bug report button.");
                return false;
            }

            ModModel? model = ModCache.Where(mod => mod.FullName == fullName).FirstOrDefault();
            if(model == null) {
                string error = $"Couldn't find ModModel for '{fullName}'";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }
            else {
                model.IsDownloading = true;
            }

            string zipFileLocation = $"{ProgramData.FilePaths.modsFolder}\\{fullName}.zip";
            
            using (HttpClient client = new HttpClient()) {
                client.DefaultRequestHeaders.Add("Accept", "text/html, application/xhtml+xml, */*");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                try {
                    using (HttpResponseMessage response = await client.GetAsync(thunderStoreMod.versions[0].download_url, HttpCompletionOption.ResponseHeadersRead)) {
                        response.EnsureSuccessStatusCode();

                        using (Stream contentStream = await response.Content.ReadAsStreamAsync(),
                            fileStream = new FileStream(zipFileLocation, FileMode.Create, FileAccess.Write, FileShare.None)) {
                            await contentStream.CopyToAsync(fileStream);
                        }
                    }

                    OnDownloadFinished(thunderStoreMod, zipFileLocation);
                    return true;
                }
                catch (Exception ex) {
                    string error = $"An error occurred while downloading {fullName}: {ex.Message}";
                    Log.Error(error);
                    DebugUtils.CrashIfDebug(error);
                    dialogService.ShowErrorMessage($"Couldn't Download {fullName}", "An error occured while trying to download this mod.\nPlease click the bug report button.");
                    return false;
                }
            }
        }

        private void OnDownloadFinished(ThunderStoreMod thunderStoreMod, string zipFileLocation) {
            downloadedMods.Add(thunderStoreMod.uuid4, ModVersion.Parse(thunderStoreMod.versions[0].version_number));
            Save();

            profileManager.AddMod(thunderStoreMod);
            UpdateModCache(false);

            // ModFilesManager.ProcessZipFile(zipFileLocation);
            // ToDo: DI this
        }

        // API Functions

        private async Task<string> GetApiData(string apiUrl) {
            try {
                using (HttpClient client = new HttpClient()) {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode) {
                        Connected = true;
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
                if (Connected) {
                    dialogService.ShowErrorMessage("Couldn't connect to ThunderStore", "Couldn't fetch mods from Thunderstore. Local cache will be loaded instead.");
                }

                Log.Warning("Couldn't get mods from ThunderStore, switching to local cache");
                Connected = false;
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

        // Data Functions

        private void Save() {
            string json = JsonConvert.SerializeObject(downloadedMods, Formatting.Indented);
            File.WriteAllText(ProgramData.FilePaths.downloadedModsFile, json);

            string modCacheJson = JsonConvert.SerializeObject(thunderStoreModCache);
            File.WriteAllText(ProgramData.FilePaths.modsCacheFile, modCacheJson);
        }

        public void Load() {
            if (File.Exists(ProgramData.FilePaths.downloadedModsFile)) {
                string json = File.ReadAllText(ProgramData.FilePaths.downloadedModsFile);
                downloadedMods = JsonConvert.DeserializeObject<Dictionary<string, ModVersion>>(json ?? "{}") ?? new Dictionary<string, ModVersion>();
            }

            if (File.Exists(ProgramData.FilePaths.modsCacheFile)) {
                string json = File.ReadAllText(ProgramData.FilePaths.modsCacheFile);
                thunderStoreModCache = JsonConvert.DeserializeObject<List<ThunderStoreMod>>(json ?? "[]") ?? new List<ThunderStoreMod>();
            }
        }
    }
}
