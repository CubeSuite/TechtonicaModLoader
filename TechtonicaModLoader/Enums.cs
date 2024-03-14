using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace TechtonicaModLoader
{
    #region ModListSource

    public enum ModListSource
    {
        Installed,
        NewMods,
        Online,
        Null
    }

    public static partial class StringUtils
    {
        public static ModListSource GetModListSourceFromName(string name) {
            switch (name) {
                case "Installed": return ModListSource.Installed;
                case "New Mods": return ModListSource.NewMods;
                case "Online": return ModListSource.Online;
                case "Null": return ModListSource.Null;
                default: return ModListSource.Null;
            }
        }

        public static string GetModListSourceName(ModListSource modListSource) {
            switch (modListSource) {
                case ModListSource.Installed: return "Installed";
                case ModListSource.NewMods: return "New Mods";
                case ModListSource.Online: return "Online";
                case ModListSource.Null: return "Null";
                default: return Enum.GetName(typeof(ModListSource), modListSource);
            }
        }

        public static List<string> GetAllModListSourceNames() {
            List<string> names = new List<string>();
            foreach (ModListSource modListSource in Enum.GetValues(typeof(ModListSource))) {
                if (modListSource != ModListSource.Null) {
                    string name = GetModListSourceName(modListSource);
                    if (name != "Null") names.Add(name);
                }
            }

            return names;
        }

        public static string GetAllModListSourceNamesForCombo() {
            return string.Join("|", GetAllModListSourceNames());
        }
    }

    #endregion

    #region ModListSortOption

    public enum ModListSortOption
    {
        Alphabetical,
        LastUpdated,
        Downloads,
        Popularity,
        Null,
    }

    public static partial class StringUtils
    {
        public static ModListSortOption GetModListSortOptionFromName(string name) {
            switch (name) {
                case "Last Updated": return ModListSortOption.LastUpdated;
                case "Alphabetical": return ModListSortOption.Alphabetical;
                case "Downloads": return ModListSortOption.Downloads;
                case "Popularity": return ModListSortOption.Popularity;
                default:
                    Log.Error($"No member of ModListSortOption has name '{name}'");
                    return ModListSortOption.Null;
            }
        }

        public static string GetModListSortOptionName(ModListSortOption modListSortOption) {
            switch (modListSortOption) {
                case ModListSortOption.LastUpdated: return "Last Updated";
                case ModListSortOption.Alphabetical: return "Alphabetical";
                case ModListSortOption.Downloads: return "Downloads";
                case ModListSortOption.Popularity: return "Popularity";
                case ModListSortOption.Null: return "Null";
                default:
                    string defaultName = Enum.GetName(typeof(ModListSortOption), modListSortOption);
                    Log.Error($"Name not set for ModListSortOption member: {defaultName}");
                    return defaultName;
            }
        }

        public static List<string> GetAllModListSortOptionNames() {
            List<string> allNames = new List<string>();
            foreach (ModListSortOption modListSortOption in Enum.GetValues(typeof(ModListSortOption))) {
                string name = GetModListSortOptionName(modListSortOption);
                if(name != "Null") allNames.Add(name);
            }

            return allNames;
        }

        public static string GetAllModListSortOptionNamesForCombo() {
            return string.Join("|", GetAllModListSortOptionNames());
        }
    }

    #endregion
}
