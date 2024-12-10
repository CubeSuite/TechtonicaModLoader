namespace TechtonicaModLoader.Resources
{
    internal static class ModListSortOptionDisplay
    {
        public static SortedDictionary<ModListSortOption, string> Strings { get; } = new() {
            { ModListSortOption.LastUpdated, StringResources.ModListSortOptionLastUpdated },
            { ModListSortOption.Newest, StringResources.ModListSortOptionNewest },
            { ModListSortOption.Alphabetical, StringResources.ModListSortOptionAlphabetical },
            { ModListSortOption.Downloads, StringResources.ModListSortOptionDownloads },
            { ModListSortOption.Popularity, StringResources.ModListSortOptionPopularity },
        };
    }
}
