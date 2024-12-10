namespace TechtonicaModLoader.Stores.Settings.V1
{
    internal class SettingsDataV1
    {
        public BasicSettingV1<bool> logDebugMessages = new();
        public BasicSettingV1<string> gameFolder = new();
        public ModListSortOptionSettingV1 defaultSortOption = new();
        public ModListSourceSettingV1 defaultModList = new();
        public BasicSettingV1<string> backupsFolder = new();
        public BasicSettingV1<int> numBackups = new();
        public ColorSettingV1 dimBackground = new();
        public ColorSettingV1 normalBackground = new();
        public ColorSettingV1 brightBackground = new();
        public ColorSettingV1 uiBackground = new();
        public ColorSettingV1 accentColour = new();
        public ColorSettingV1 textColour = new();
        public BasicSettingV1<bool> isFirstTimeLaunch = new();
        public BasicSettingV1<string> lastProfile = new();
        public SeenModsSettingV1 seenMods = new();
    }
}

