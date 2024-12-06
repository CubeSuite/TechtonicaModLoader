using TechtonicaModLoader.Windows.Dialogs;

namespace TechtonicaModLoader.Resources
{
    internal static class WarningLevelDisplay
    {
        public static SortedDictionary<WarningLevel, string> Strings { get; } = new() {
            { WarningLevel.Info, "" },
            { WarningLevel.Warning, "" },
            { WarningLevel.Error, "" },
        };
    }
}
