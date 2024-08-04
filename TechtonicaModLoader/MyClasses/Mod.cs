using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TechtonicaModLoader.MyClasses;
using TechtonicaModLoader.MyClasses.ThunderStoreResponses;
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader.Modes
{
    public class Mod
    {
        // Members
        public string id;
        public string name;
        public string author;
        public ModVersion version;
        public string tagLine;
        public string description;
        public DateTime dateUpdated;
        public int ratingScore;
        public int downloads;
        public bool isDeprecated;
        public bool hasDonated;
        public bool updateAvailable;
        public List<string> categories = new List<string>();
        public List<string> dependencies = new List<string>();

        public string link;
        public string iconLink;
        public string donationLink;
        public string zipFileDownloadLink;

        public string zipFileLocation;
        public string configFileLocation;
        public string markdownFileLocation;
        public List<string> installedFiles = new List<string>();

        public bool canBeToggled = true;

        private bool isDownloading = false;

        // Constructors

        public Mod() { }
        public Mod fromJson(string json) {
            return JsonConvert.DeserializeObject<Mod>(json);
        }

        public Mod(ThunderStoreMod thunderStoreMod) {
            id = thunderStoreMod.uuid4;
            name = thunderStoreMod.name;
            author = thunderStoreMod.owner;
            dateUpdated = DateTime.Parse(thunderStoreMod.date_updated);
            ratingScore = thunderStoreMod.rating_score;
            isDeprecated = thunderStoreMod.is_deprecated;
            categories = thunderStoreMod.categories;

            link = thunderStoreMod.package_url;
            donationLink = thunderStoreMod.donation_link;

            if (thunderStoreMod.versions.Count > 0) {
                ThunderStoreVerion versionInfo = thunderStoreMod.versions[0];
                version = ModVersion.Parse(versionInfo.version_number);
                tagLine = versionInfo.description;
                dependencies = versionInfo.dependencies;

                iconLink = versionInfo.icon;
                zipFileDownloadLink = versionInfo.download_url;
            }

            downloads = 0;
            foreach (ThunderStoreVerion versionInfo in thunderStoreMod.versions) {
                downloads += versionInfo.downloads;
            }
        }

        // Events

        private void OnDownloadFinished(object sender, AsyncCompletedEventArgs e) {
            Log.Debug($"Mod '{name}' finished downloading");
            isDownloading = false;
        }

        // Public Functions

        public async Task<bool> DownloadAndInstall() {
            GuiUtils.ShowDownloadingGui(name);

            if (!await CheckDependencies()) {
                GuiUtils.HideDownloadingGui(name);
                return false;
            }

            Log.Debug("CheckDependencies() passed");

            if (!await DownloadZipFile()) {
                GuiUtils.HideDownloadingGui(name);
                return false;
            }

            Log.Debug("DownloadZipFile() passed");

            GuiUtils.ShowInstallingGui(name);
            if (!Install()) {
                GuiUtils.HideDownloadingGui(name);
                return false;
            }

            GuiUtils.HideDownloadingGui(name);
            return true;
        }

        public bool Install() {
            if (!CheckForZipFile()) return false;
            Log.Debug("CheckForZipFile() passed");

            if (!FileStructureUtils.ValidateGameFolder()) {
                GuiUtils.ShowErrorMessage("Invalid Game Folder", "Please check that your game folder is correct in the settings.");
                return false;
            }
            Log.Debug("ValidateGameFolder() passed");

            if (!CopyZipToModsFolder()) return false;
            Log.Debug("CopyZipToModsFolder() passed");

            FileStructureUtils.ClearUnzipFolder();
            Log.Debug("ClearUnzipFolder() passed");

            if (!UnzipToTempFolder()) return false;
            Log.Debug("UnzipToTempFolder() passed");

            if (id != ProgramData.bepInExID) {
                if (FileStructureUtils.SearchForConfigFile(ProgramData.FilePaths.unzipFolder, out configFileLocation)) {
                    string pathInDataConfigsFolder = configFileLocation.Replace(Path.GetDirectoryName(configFileLocation), ProgramData.FilePaths.configsFolder);
                    if (File.Exists(pathInDataConfigsFolder)) {
                        ModConfig oldConfig = ModConfig.FromFile(pathInDataConfigsFolder);
                        ModConfig newConfig = ModConfig.FromFile(configFileLocation);

                        foreach (ConfigOption option in oldConfig.options) {
                            if (!newConfig.HasSetting(option.name)) continue;
                            newConfig.UpdateSetting(option);
                        }

                        newConfig.SaveToFile();
                        File.Delete(pathInDataConfigsFolder);
                    }

                    File.Copy(configFileLocation, pathInDataConfigsFolder);
                    configFileLocation = pathInDataConfigsFolder;
                    installedFiles.Add(pathInDataConfigsFolder);
                    Log.Info($"Installed config file for '{name}'");
                }

                if (FileStructureUtils.SearchForMarkdownFile(ProgramData.FilePaths.unzipFolder, out markdownFileLocation)) {
                    string newPath = markdownFileLocation.Replace(Path.GetDirectoryName(markdownFileLocation), ProgramData.FilePaths.markdownFiles);
                    newPath = newPath.Replace("README", name);
                    if (File.Exists(newPath)) {
                        Log.Warning($"Config file already exists at '{newPath}' - deleting original");
                        File.Delete(newPath);
                    }

                    File.Copy(markdownFileLocation, newPath);
                    markdownFileLocation = newPath;
                    installedFiles.Add(markdownFileLocation);
                    Log.Info($"Cached markdown file for '{name}'");
                }

                string pluginsFolder = $"{ProgramData.FilePaths.unzipFolder}/plugins";
                string patchersFolder = $"{ProgramData.FilePaths.unzipFolder}/patchers";
                string dataFolder = $"{ProgramData.FilePaths.unzipFolder}/Techtonica_Data";
                string rootFolder = $"{ProgramData.FilePaths.unzipFolder}/Techtonica";
                bool hasPluginFiles = Directory.Exists(pluginsFolder);
                bool hasPatcherFiles = Directory.Exists(patchersFolder);
                bool hasDataFiles = Directory.Exists(dataFolder);
                bool hasRootFiles = Directory.Exists(rootFolder);

                Log.Debug($"hasPluginFiles: {hasPluginFiles}");
                Log.Debug($"hasPatcherFiles: {hasPatcherFiles}");
                Log.Debug($"hasDataFiles: {hasDataFiles}");
                Log.Debug($"hasRootFiles: {hasRootFiles}");

                if (hasPluginFiles) {
                    string targetFolder = $"{ProgramData.FilePaths.bepInExPluginsFolder}\\{name}";
                    Directory.CreateDirectory(targetFolder);
                    List<string> dllFiles = FileStructureUtils.SearchForDllFiles(pluginsFolder);
                    InstallFiles(dllFiles, targetFolder);
                }

                if (hasPatcherFiles) {
                    List<string> dllFiles = FileStructureUtils.SearchForDllFiles(patchersFolder);
                    InstallFiles(dllFiles, ProgramData.FilePaths.bepInExPatchersFolder);
                }

                if (hasDataFiles) {
                    List<string> dataFiles = Directory.GetFiles(dataFolder).ToList();
                    InstallFiles(dataFiles, ProgramData.FilePaths.gameDataFolder);
                }

                if (hasRootFiles) {
                    List<string> rootFiles = Directory.GetFiles(rootFolder).ToList();
                    InstallFiles(rootFiles, ProgramData.FilePaths.gameFolder);
                }

                if (!hasPluginFiles && !hasPatcherFiles && !hasDataFiles && !hasRootFiles) {
                    List<string> dllFiles = FileStructureUtils.SearchForDllFiles(ProgramData.FilePaths.unzipFolder);
                    InstallFiles(dllFiles, ProgramData.FilePaths.bepInExPluginsFolder);
                }
            }
            else {
                Log.Debug($"Installing BepInEx");
                canBeToggled = false;
                installedFiles = FileStructureUtils.CopyFolder($"{ProgramData.FilePaths.unzipFolder}/BepInExPack", ProgramData.FilePaths.gameFolder);
            }

            FileStructureUtils.ClearUnzipFolder();
            Log.Info($"Successfully installed '{name}'");

            Profile activeProfile = ProfileManager.GetActiveProfile();
            activeProfile.AddMod(id);

            return true;
        }

        public void Uninstall() {
            if (name == "BepInExPack") return;

            Log.Info($"Uninstalling mod '{name}'");

            List<string> foldersToDelete = new List<string>();
            foreach (string file in installedFiles) {
                if (file.EndsWith(".md") || file.EndsWith(".zip") || file.EndsWith(".cfg")) continue;
                if (File.Exists(file)) {
                    if (DoesFileHaveNamedParentFolder(file)) {
                        foldersToDelete.Add(Path.GetDirectoryName(file));
                    }

                    Log.Debug($"Deleting file '{Path.GetFileName(file)}'");
                    File.Delete(file);
                }
            }

            foldersToDelete = foldersToDelete.Distinct().ToList();
            foreach (string folder in foldersToDelete) {
                if (folder == ProgramData.FilePaths.gameFolder || folder == ProgramData.FilePaths.gameDataFolder) continue;
                Log.Debug($"Deleting folder '{folder.Replace(ProgramData.FilePaths.gameFolder, "")}'");
                Directory.Delete(folder);
            }

            installedFiles.Clear();
        }

        public bool HasConfigFile() {
            return !string.IsNullOrEmpty(configFileLocation) &&
                    configFileLocation != "Not Found" &&
                    name != "BepInExPack";
        }

        public bool HasMarkdownFile() {
            return !string.IsNullOrEmpty(markdownFileLocation) &&
                    markdownFileLocation != "Not Found";
        }

        public bool AppearsInSearch(string searchTerm) {
            string searchableParts = $"{name}|{author}|{tagLine}|{description}";
            return searchableParts.ToLower().Contains(searchTerm.ToLower());
        }

        public string ToJson(bool indented = true) {
            return JsonConvert.SerializeObject(this, indented ? Formatting.Indented : Formatting.None);
        }

        public bool IsDependencyOfAnother(out string dependentMod) {
            foreach(Mod mod in ModManager.GetAllMods()) {
                foreach(string dependency in mod.dependencies) {
                    if (dependency.Contains(name)) {
                        dependentMod = mod.name;
                        Log.Debug($"Mod '{name}' is a dependency of {mod.name} - {dependency}");
                        return true;
                    }
                }
            }
            dependentMod = "None";
            return false;
        }

        // Private Functions

        private async Task<bool> CheckDependencies() {
            if (dependencies.Count != 0) {
                foreach (string dependency in dependencies) {
                    Mod mod = await ThunderStore.SearchForMod(dependency);
                    if (mod == null) {
                        string error = $"Failed to find dependent mod - {dependency}";
                        Log.Error(error);
                        GuiUtils.ShowErrorMessage("Mod Download Failed", error);
                        return false;
                    }

                    if (!ModManager.DoesModExist(mod)) {
                        Log.Debug($"Downloading new dependent mod '{mod.name}'");
                        ModManager.AddMod(mod);
                        ModManager.Save();
                        await mod.DownloadAndInstall();
                        Log.Debug($"Downloaded and installed new dependent mod '{mod.name}'");
                    }
                }
            }
            else {
                Log.Debug($"Mod '{name}' has no dependencies");
            }

            return true;
        }

        private async Task<bool> DownloadZipFile() {
            try {
                zipFileLocation = $"{ProgramData.FilePaths.unzipFolder}\\{name}.zip";
                isDownloading = true;

                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webClient.DownloadFileCompleted += OnDownloadFinished;
                webClient.DownloadFileAsync(new Uri(zipFileDownloadLink), zipFileLocation);

                while (isDownloading) {
                    await Task.Delay(10);
                }

                return true;
            }
            catch (Exception e) {
                GuiUtils.ShowErrorMessage("Mod Download Failed", "Something went wrong while downloading this mod, please try again later.");
                string error = $"Error occurred while downloading file: {e.Message}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return false;
            }
        }

        private bool CheckForZipFile() {
            if (File.Exists(zipFileLocation)) {
                Log.Debug($"Mod '{name}' has a valid zipFileLocation");
                return true;
            }

            string error = $"Could not install mod '{name}' - Cannot find .zip file";
            Log.Error($"{error} at '{zipFileLocation}'");
            GuiUtils.ShowErrorMessage("Mod Download Failed", error);
            return false;
        }

        private bool CopyZipToModsFolder() {
            string source = Path.GetDirectoryName(zipFileLocation);
            string newPath = zipFileLocation.Replace(source, ProgramData.FilePaths.modsFolder);

            if (zipFileLocation == newPath) {
                Log.Debug($"Mod '{name}' has been installed before");
                return true;
            }
            
            if (File.Exists(newPath)) {
                Log.Warning($"Mods folder already contains '{Path.GetFileName(zipFileLocation)}' - deleting original");
                File.Delete(newPath);
            }

            File.Copy(zipFileLocation, newPath);
            zipFileLocation = newPath;
            Log.Debug($"Copied '{Path.GetFileName(zipFileLocation)}' to Mods folder");
            return true;
        }

        private bool UnzipToTempFolder() {
            Log.Debug($"Unzipping '{Path.GetFileName(zipFileLocation)}'");
            using (ZipArchive archive = ZipFile.OpenRead(zipFileLocation)) {
                foreach (ZipArchiveEntry entry in archive.Entries) {
                    string entryFilePath = Path.Combine(ProgramData.FilePaths.unzipFolder, entry.FullName);
                    Directory.CreateDirectory(Path.GetDirectoryName(entryFilePath));
                    if (!entryFilePath.EndsWith("/")) {
                        entry.ExtractToFile(entryFilePath, true);
                        Log.Debug($"Extracted '{entryFilePath}'");
                        if (entryFilePath.EndsWith(".cfg")) {
                            configFileLocation = entryFilePath.Replace(ProgramData.FilePaths.unzipFolder, ProgramData.FilePaths.gameFolder);
                            Log.Debug($"Found config file, store at '{configFileLocation}'");
                        }
                    }
                }
            }

            return true;
        }

        private bool DoesFileHaveNamedParentFolder(string file) {
            string parentFolder = Path.GetDirectoryName(file).Split('\\').Last();
            List<string> validParents = new List<string>() {
                "patchers",
                "plugins",
                "config",
                "MarkdownFiles"
            };
            return !validParents.Contains(parentFolder);
        }

        private bool InstallFiles(List<string> files, string targetFolder) {
            Log.Debug($"Installing {files.Count} files to '{targetFolder}'");
            foreach (string file in files) {
                try {
                    string newPath = file.Replace("/", "\\").Replace(Path.GetDirectoryName(file), targetFolder);
                    Log.Debug($"Installing '{file}' to '{newPath}'");

                    if (File.Exists(newPath)) {
                        Log.Warning($"File already exists - deleting original");
                        File.Delete(newPath);
                    }

                    File.Copy(file, newPath);
                    installedFiles.Add(newPath);

                    Log.Debug($"Installed file successfully");
                }
                catch (Exception e) {
                    Log.Error($"Error occurred while installing file: '{e.Message}'");
                    Log.Error(e.StackTrace);
                    GuiUtils.ShowErrorMessage("Mod Download Failed", "An error occurred while installing this mod. Please contact the developer.");
                    return false;
                }
            }

            return true;
        }
    }

    public struct ModVersion
    {
        public int major;
        public int minor;
        public int build;

        public static ModVersion Parse(string input) {
            try {
                string[] parts = input.Split('.');
                return new ModVersion() {
                    major = int.Parse(parts[0]),
                    minor = int.Parse(parts[1]),
                    build = int.Parse(parts[2]),
                };
            }
            catch (Exception e) {
                string error = $"Error occurred while parsing Version '{input}': {e.Message}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return new ModVersion() {
                    major = 0,
                    minor = 0,
                    build = 0
                };
            }
        }

        public override string ToString() {
            return $"{major}.{minor}.{build}";
        }

        public static bool operator >(ModVersion v1, ModVersion v2) {
            if (v1.major > v2.major) return true;
            if (v1.major < v2.major) return false;

            if (v1.minor > v2.minor) return true;
            if (v1.minor < v2.minor) return false;

            return v1.build > v2.build;
        }

        public static bool operator <(ModVersion v1, ModVersion v2) {
            if (v1.major < v2.major) return true;
            if (v1.major > v2.major) return false;

            if (v1.minor < v2.minor) return true;
            if (v1.minor > v2.minor) return false;

            return v1.build < v2.build;
        }
    }
}
