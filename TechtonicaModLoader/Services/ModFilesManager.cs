using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TechtonicaModLoader.MVVM.Models;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Services.ThunderstoreModels;

namespace TechtonicaModLoader.Stores
{
    public interface IModFilesManager
    {
        bool ProcessZipFile(string zipFileLocation, ThunderStoreMod mod);
        bool DeployMods();
    }

    public class ModFilesManager : IModFilesManager
    {
        // Members
        private ILoggerService logger;
        private IDialogService dialogService;
        private IProfileManager profileManager;
        private IProgramData programData;
        private IDebugUtils debugUtils;

        private Dictionary<string, HashSet<string>> modFiles;

        // Constructors

        public ModFilesManager(IServiceProvider serviceProvider) {
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            dialogService = serviceProvider.GetRequiredService<IDialogService>();
            profileManager = serviceProvider.GetRequiredService<IProfileManager>();
            programData = serviceProvider.GetRequiredService<IProgramData>();
            debugUtils = serviceProvider.GetRequiredService<IDebugUtils>();

            modFiles = new Dictionary<string, HashSet<string>>();
            Load();
        }

        // Public Functions

        public bool ProcessZipFile(string zipFileLocation, ThunderStoreMod mod) {
            logger.Info($"Processing zip file: '{zipFileLocation}'");

            if (!File.Exists(zipFileLocation)) {
                string error = $"zip file does not exist: '{zipFileLocation}'";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);
            }

            UnzipToTemp(zipFileLocation, out string extractedFolderPath);
            CopyToDataFolders(extractedFolderPath, mod);
            DeleteFolder(extractedFolderPath);

            logger.Debug("Saving ModFiles.json");
            Save();

            logger.Info($"Successfully processed zip file: '{zipFileLocation}'");
            return true;
        }

        public bool DeployMods() {
            logger.Info("Attempting to deploy mods");
            // ToDo: Think about developer mode

            try {
                logger.Debug("Creating BepInEx directories");
                Directory.CreateDirectory(programData.FilePaths.BepInExPatchesFolder);
                Directory.CreateDirectory(programData.FilePaths.BepInExPluginsFolder);
                Directory.CreateDirectory(programData.FilePaths.BepInExConfigsFolder);

                List<string> filesToInstall = GetFilesToInstall();
                if (filesToInstall.Count() == 0) {
                    logger.Warning($"Profile has no mods to deploy, aborting");
                    return true;
                }

                foreach (string file in filesToInstall) {
                    logger.Debug($"Attempting to install {file}");
                    string newPath = CopyFile(file, programData.FilePaths.RootFolder, programData.FilePaths.BepInExFolder);
                    logger.Debug($"Successfully installed to {newPath}");
                }

                logger.Info("Successfully deployed mods");
                return true;
            }
            catch (Exception e) {
                string error = $"Error occurred while deploying mods: {e.Message}";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);

                if (!programData.IsDebugBuild) {
                    dialogService.ShowErrorMessage("Couldn't Deploy Mods", "An error occurred while trying to deploy mods. Please click the bug report button");
                    // ToDo: Auto display and populate bug report view
                }

                return false;
            }
        }

        // Data Functions

        private void Save() {
            string json = JsonConvert.SerializeObject(modFiles, Formatting.Indented);
            File.WriteAllText(programData.FilePaths.ModFilesFile, json);
        }

        private void Load() {
            if (!File.Exists(programData.FilePaths.ModFilesFile)) return;

            string json = File.ReadAllText(programData.FilePaths.ModFilesFile);
            modFiles = JsonConvert.DeserializeObject<Dictionary<string, HashSet<string>>>(json) ?? new Dictionary<string, HashSet<string>>();
        }

        // Private Functions

        private bool UnzipToTemp(string zipFileLocation, out string extractedFolderPath) {
            extractedFolderPath = "";
            string zipName = Path.GetFileNameWithoutExtension(zipFileLocation);

            logger.Info($"Unzipping '{zipName}'");
            try {
                using (ZipArchive archive = ZipFile.OpenRead(zipFileLocation)) {
                    foreach (ZipArchiveEntry entry in archive.Entries) {
                        extractedFolderPath = $"{programData.FilePaths.UnzipFolder}\\{zipName}";
                        string entryFilePath = Path.Combine(extractedFolderPath, entry.FullName);
                        
                        Directory.CreateDirectory(Path.GetDirectoryName(entryFilePath) ?? throw new Exception("GetDirectoryName() returned null"));
                        if (!entryFilePath.EndsWith("/")) {
                            entry.ExtractToFile(entryFilePath, true);
                            logger.Debug($"Extracted '{entryFilePath}'");
                        }
                    }
                }

                logger.Debug($"Attempting to delete '{zipFileLocation}'");
                File.Delete(zipFileLocation);

                logger.Info($"Successfully unzipped & deleted '{zipFileLocation}'");
                return true;
            }
            catch (Exception e) {
                string error = $"Error occured while unzipping '{zipName}'.zip: {e.Message}";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);

                if (!programData.IsDebugBuild) {
                    dialogService.ShowErrorMessage("Error Occurred While Unzipping Mod", "Please click the bug report button.");
                    // ToDo: Auto open and populate bug-report View
                    // ToDo: Remove from downloaded mods - Maybe don't add it to downloaded mods until all files are extracted to data folders successfully
                }

                return false;
            }
        }

        private void AddToModFiles(ThunderStoreMod mod, string filePath) {
            if (modFiles.ContainsKey(mod.uuid4)) {
                modFiles[mod.uuid4].Add(filePath);
            }
            else {
                modFiles.Add(mod.uuid4, new HashSet<string>() { filePath });
            }

            Save();
        }

        private void CopyToDataFolders(string extractedFolderPath, ThunderStoreMod mod) {
            logger.Info($"Copying mod files from '{extractedFolderPath}'");

            string[] patches = Array.Empty<string>();
            string[] plugins = Array.Empty<string>();
            string[] configs = Array.Empty<string>();

            string patchesFolder = $"{extractedFolderPath}\\patches";
            string pluginsFolder = $"{extractedFolderPath}\\plugins";
            string configsFolder = $"{extractedFolderPath}\\config";

            if (Directory.Exists(patchesFolder)) patches = SearchForFiles(patchesFolder);
            if (Directory.Exists(pluginsFolder)) plugins = SearchForFiles(pluginsFolder);
            if (Directory.Exists(configsFolder)) configs = SearchForFiles(configsFolder, "*.cfg");

            if(patches.Count() == 0 && plugins.Count() == 0) {
                logger.Warning("Didn't find any .dll files in patches or plugins folders. Searching extractedFolderPath instead. DLL's will be assumed to be plugins");
                plugins = SearchForFiles(extractedFolderPath, "dll");
            }

            foreach(string patch in patches) {
                logger.Debug($"Copying patch file {patch}");
                string newPath = CopyFile(patch, patchesFolder, programData.FilePaths.PatchesFolder);
                AddToModFiles(mod, newPath);
                logger.Debug($"Copied patch file {Path.GetFileName(patch)}");
            }

            foreach(string plugin in plugins) {
                logger.Debug($"Copying plugin file {Path.GetFileName(plugin)}");
                string newPath = CopyFile(plugin, pluginsFolder, programData.FilePaths.PluginsFolder);
                AddToModFiles(mod, newPath);
                logger.Debug($"Copied plugin file {Path.GetFileName(plugin)}");
            }

            foreach(string config in configs) {
                logger.Debug($"Copying config file {Path.GetFileName(config)}");
                string newPath = CopyFile(config, configsFolder, programData.FilePaths.ConfigsFolder);
                AddToModFiles(mod, newPath);
                logger.Debug($"Copied config file {Path.GetFileName(config)}");
            }

            logger.Info($"Successfully copied files to data folder");
        }

        private List<string> GetFilesToInstall() {
            List<string> filesToInstall = new List<string>();

            IEnumerable<string> modIDsToDeploy = profileManager.ActiveProfile.GetModIDsToDeploy();
            foreach (string modToDeploy in modIDsToDeploy) {
                if (modFiles.ContainsKey(modToDeploy)) {
                    filesToInstall.AddRange(modFiles[modToDeploy]);
                }
                else {
                    string error = $"Cannot deploy mod with id '{modToDeploy}' as it is not present in modFiles";
                    logger.Error(error);
                    debugUtils.CrashIfDebug(error);

                    if (!programData.IsDebugBuild) {
                        dialogService.ShowErrorMessage(StringResources.DeployModsErrorTitle, StringResources.DeployModsErrorMessage);
                        // ToDo: Auto open and populate bug report view
                    }

                    return new List<string>();
                }
            }

            return filesToInstall;
        }

        // Util Functions

        private string[] SearchForFiles(string folder, string fileTypeExtension = "") {
            return Directory.GetFiles(folder, fileTypeExtension, SearchOption.AllDirectories);
        }

        private void DeleteFolder(string folder) {
            try {
                string[] files = Directory.GetFiles(folder);
                foreach (string file in files) {
                    File.Delete(file);
                }

                string[] subFolders = Directory.GetDirectories(folder);
                foreach (string subFolder in subFolders) {
                    DeleteFolder(subFolder);
                }

                Directory.Delete(folder);
            }
            catch (Exception e) {
                string error = $"Error occurred while deleting '{folder}': {e.Message}";
                logger.Error(error);
                logger.Debug(e.StackTrace ?? "Couldn't log StackTrace, is null");

                if (!ProgramData.IsDebugBuild) {
                    dialogService.ShowErrorMessage(StringResources.DeleteFolderErrorTitle, StringResources.DeployModsErrorMessage);
                    // ToDo: Open bug report view
                }
            }
        }

        private string CopyFile(string file, string currentFolder, string destinationTopFolder) {
            string relativePath = Path.GetRelativePath(currentFolder, file);
            string destinationPath = Path.Combine(destinationTopFolder, relativePath);
            string? destinationFolder = Path.GetDirectoryName(destinationPath);

            if (destinationFolder != null) {
                Directory.CreateDirectory(destinationFolder);
            }
            else {
                throw new Exception("Path.GetDirectoryName(destinationPath) returned null");
            }

            File.Copy(file, destinationPath, overwrite: true); // ToDo: think about overwrite: true | false for developer mode?
            return destinationPath;
        }
    }
}
