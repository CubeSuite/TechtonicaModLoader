using CommunityToolkit.Mvvm.ComponentModel;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public abstract partial class SettingBase : ObservableObject
    {
        // Properties

        public string Name { get; }
        public string Description { get; }
        public string Category { get; }

        // Constructors

        public SettingBase(string name, string description, string category) {
            Name = name;
            Description = description;
            Category = category;

            if (!Description.EndsWith('.')) {
                Description += '.';
            }

        }
    }
}
