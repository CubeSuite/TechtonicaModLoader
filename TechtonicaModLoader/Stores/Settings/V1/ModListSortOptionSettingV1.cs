namespace TechtonicaModLoader.Stores.Settings.V1
{
    internal class ModListSortOptionSettingV1 : BasicSettingV1<string>
    {
        public ModListSortOption GetModListSortOption() {
            return Value switch {
                "Last Updated" => ModListSortOption.LastUpdated,
                "Alphabetical" => ModListSortOption.Alphabetical,
                "Downloads" => ModListSortOption.Downloads,
                "Popularity" => ModListSortOption.Popularity,
                _ => ModListSortOption.Newest,
            };
        }
    }
}
