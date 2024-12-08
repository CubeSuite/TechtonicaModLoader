using System.Drawing;

namespace TechtonicaModLoader.Stores.Settings
{
    internal class SettingsData
    {
        public static readonly Version CurrentSchemaVersion = new(2, 0);

        public Version SchemaVersion = CurrentSchemaVersion;
        public bool LogDebugMessages = false;
        public string GameFolder = "";
        public ModListSource DefaultModList = ModListSource.New;
        public ModListSortOption DefaultModListSortOption = ModListSortOption.Newest;
        public string BackupsFolder = "";
        public int NumBackups = 0;
        public bool IsFirstTimeLaunch = true;
        public int ActiveProfileID = 0;
        public bool DeployNeeded = false;
        public List<string> SeenMods = [];
        public Color DimBackground = Color.FromArgb(22, 22, 38);
        public Color NormalBackground = Color.FromArgb(48, 48, 64);
        public Color BrightBackground = Color.FromArgb(58, 58, 88);
        public Color UiBackground = Color.FromArgb(68, 68, 98);
        public Color AccentColour = Color.Orange;
        public Color TextColour = Color.White;

        public SettingsData() {
        }

        public static bool operator ==(SettingsData x, SettingsData y) => x.Equals(y);
        public static bool operator !=(SettingsData x, SettingsData y) => !(x == y);

        public bool Equals(SettingsData? other) {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (SeenMods.Count != other.SeenMods.Count) return false;
            for (int i = 0; i < SeenMods.Count; ++i) {
                if (!SeenMods[i].Equals(other.SeenMods[i])) return false;
            }

            return SchemaVersion.Equals(other.SchemaVersion) &&
                LogDebugMessages.Equals(other.LogDebugMessages) &&
                GameFolder.Equals(other.GameFolder) &&
                DefaultModList.Equals(other.DefaultModList) &&
                DefaultModListSortOption.Equals(other.DefaultModListSortOption) &&
                BackupsFolder.Equals(other.BackupsFolder) &&
                NumBackups.Equals(other.NumBackups) &&
                IsFirstTimeLaunch.Equals(other.IsFirstTimeLaunch) &&
                ActiveProfileID.Equals(other.ActiveProfileID) &&
                DeployNeeded.Equals(other.DeployNeeded) &&
                DimBackground.Equals(other.DimBackground) &&
                NormalBackground.Equals(other.NormalBackground) &&
                BrightBackground.Equals(other.BrightBackground) &&
                UiBackground.Equals(other.UiBackground) &&
                AccentColour.Equals(other.AccentColour) &&
                TextColour.Equals(other.TextColour);
        }
        public override bool Equals(object? obj) => Equals(obj as SettingsData);

        public override int GetHashCode() {
            int hash1 = HashCode.Combine(SchemaVersion, LogDebugMessages, GameFolder, DefaultModList, DefaultModListSortOption, BackupsFolder, NumBackups, IsFirstTimeLaunch);
            int hash2 = HashCode.Combine(ActiveProfileID, DeployNeeded, DimBackground, NormalBackground, BrightBackground, UiBackground, AccentColour, TextColour);
            HashCode seenModsHash = new();
            for (int i = 0; i < SeenMods.Count; ++i) {
                seenModsHash.Add(SeenMods[i]);
            }
            return HashCode.Combine(hash1, hash2, seenModsHash.ToHashCode());
        }
    }
}
