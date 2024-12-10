namespace TechtonicaModLoader.Resources
{
    internal static class OfflineModListSourceDisplay
    {
        public static SortedDictionary<OfflineModListSource, string> Strings { get; } = new() {
            { OfflineModListSource.Downloaded, StringResources.OfflineModListSourceDownloaded },
            { OfflineModListSource.Enabled, StringResources.OfflineModListSourceEnabled },
            { OfflineModListSource.Disabled, StringResources.OfflineModListSourceDisabled },
        };
    }
}
