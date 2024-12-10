using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.IO.Abstractions;
using TechtonicaModLoader.Stores.Settings.V1;

namespace TechtonicaModLoader.Stores.Settings
{
    internal interface ISettingsFileHandler {
        void Save(SettingsData settingsData);
        SettingsData Load();
    }

    internal class SettingsFileHandler : ISettingsFileHandler
    {
        private readonly ILoggerService logger;
        private readonly IProgramData programData;
        private readonly IServiceProvider serviceProvider;

        public SettingsFileHandler(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            programData = serviceProvider.GetRequiredService<IProgramData>();
        }

        public void Save(SettingsData settingsData) {
            IFileSystem fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
            string json = JsonConvert.SerializeObject(settingsData, Formatting.Indented);
            fileSystem.File.WriteAllText(programData.FilePaths.SettingsFile, json);
        }

        public SettingsData Load() {
            IFileSystem fileSystem = serviceProvider.GetRequiredService<IFileSystem>();
            SettingsData? settingsData = null;
            string settingsFilePath = programData.FilePaths.SettingsFile;

            if (!fileSystem.File.Exists(settingsFilePath)) {
                logger.Info("Settings file not found.");
            }
            else {
                string json = fileSystem.File.ReadAllText(settingsFilePath);
                settingsData = ParseSettingsJson(ref json);
            }

            if (settingsData is null) {
                logger.Warning("Using default settings.");
                settingsData = new();
                Save(settingsData);
            }
            return settingsData;
        }

        internal SettingsData? ParseSettingsJson(ref string json) {
            if (string.IsNullOrEmpty(json)) {
                logger.Error($"Settings file {programData.FilePaths.SettingsFile} is empty.");
                return null;
            }
            SettingsData? settingsFromFile = null;
            try {
                settingsFromFile = JsonConvert.DeserializeObject<SettingsData>(json);
            }
            catch (JsonReaderException) {
                logger.Error("Error parsing settings.");
                logger.Info("Attempting to parse as V1 schema.");
                settingsFromFile = ConvertV1Json(json);
            }
            catch (Exception ex) {
                logger.Error($"Unexpected exception parsing settings: {ex.GetType().Name}");
                logger.Debug(ex.ToString());
            }

            if (settingsFromFile is null)  logger.Error($"Settings file {programData.FilePaths.SettingsFile} is invalid.");
            else logger.Info("Settings loaded");

            return settingsFromFile;
        }

        private SettingsData? ConvertV1Json(string json) {
            if (string.IsNullOrEmpty(json)) return null;

            SettingsDataV1? v1Data = null;
            try {
                v1Data = JsonConvert.DeserializeObject<SettingsDataV1>(json);
            }
            catch (JsonReaderException) {
                logger.Warning("Failed to parse settings as V1 schema.");
            }
            catch (Exception ex) {
                logger.Error($"Unexpected exception parsing settings: {ex.GetType().Name}");
                logger.Debug(ex.ToString());
            }

            if (v1Data is null) return null;

            SettingsData settingsData = new() {
                LogDebugMessages = v1Data.logDebugMessages.Value,
                GameFolder = v1Data.gameFolder.Value ?? string.Empty,
                DefaultModList = v1Data.defaultModList.GetModListSortOption(),
                DefaultModListSortOption = v1Data.defaultSortOption.GetModListSortOption(),
                BackupsFolder = v1Data.backupsFolder.Value ?? string.Empty,
                NumBackups = v1Data.numBackups.Value,
                IsFirstTimeLaunch = v1Data.isFirstTimeLaunch.Value,
                DimBackground = v1Data.dimBackground.GetColor(),
                NormalBackground = v1Data.normalBackground.GetColor(),
                BrightBackground = v1Data.brightBackground.GetColor(),
                UiBackground = v1Data.uiBackground.GetColor(),
                AccentColour = v1Data.accentColour.GetColor(),
                TextColour = v1Data.textColour.GetColor(),
                ActiveProfileID = GetActiveProfile(v1Data.lastProfile.Value),
                SeenMods = v1Data.seenMods.GetSeenMods()
            };

            Save(settingsData);
            return settingsData;
        }

        private int GetActiveProfile(string? targetProfile) {
            if (string.IsNullOrEmpty(targetProfile)) return 0;

            IProfileManager profileManager = serviceProvider.GetRequiredService<IProfileManager>();
            for (int i = 0; i < profileManager.ProfilesList.Count; i++) {
                if (string.Equals(targetProfile, profileManager.ProfilesList[i].Name, StringComparison.CurrentCulture)) return i;
            }
            return 0;
        }
    }
}
