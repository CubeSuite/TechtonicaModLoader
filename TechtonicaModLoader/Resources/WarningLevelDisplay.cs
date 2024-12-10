using TechtonicaModLoader.Windows.Dialogs;

namespace TechtonicaModLoader.Resources
{
    internal static class WarningLevelDisplay
    {
        public static SortedDictionary<WarningLevel, string> Strings { get; } = new() {
            { WarningLevel.Info, StringResources.WarningLevelInfo },
            { WarningLevel.Warning, StringResources.WarningLevelWarning },
            { WarningLevel.Error, StringResources.WarningLevelError },
        };
    }
}
