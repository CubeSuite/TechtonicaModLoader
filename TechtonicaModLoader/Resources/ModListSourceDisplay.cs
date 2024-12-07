namespace TechtonicaModLoader.Resources
{
    internal static class ModListSourceDisplay
    {
        public static SortedDictionary<ModListSource, string> Strings { get; } = new() {
            { ModListSource.All, StringResources.ModListSourceAll },
            { ModListSource.New, StringResources.ModListSourceNew },
            { ModListSource.Downloaded, StringResources.ModListSourceDownloaded },
            { ModListSource.NotDownloaded, StringResources.ModListSourceNotDownloaded },
            { ModListSource.Enabled, StringResources.ModListSourceEnabled },
            { ModListSource.Disabled, StringResources.ModListSourceDisabled },
        };
    }
}
