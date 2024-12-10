using System.Xml.Linq;

namespace TechtonicaModLoader.Stores.Settings.V1
{
    internal class ModListSourceSettingV1 : BasicSettingV1<string>
    {
        public ModListSource GetModListSortOption() {
            return Value switch {
                "Installed" => ModListSource.Downloaded,
                "New Mods" => ModListSource.New,
                "Online" => ModListSource.NotDownloaded,
                _ => ModListSource.New,
            };
        }
    }
}
