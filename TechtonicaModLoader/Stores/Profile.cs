using CommunityToolkit.Mvvm.ComponentModel;
using TechtonicaModLoader.MVVM.Mod;

namespace TechtonicaModLoader.Stores
{
    public partial class Profile : ObservableObject
    {
        // Members

        [ObservableProperty]
        private int _id = -1;

        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private Dictionary<string, bool> _modEnabledStates;

        private bool _isDefault;
        public bool IsDefault => _isDefault;

        // Constructors

        public Profile(string name, bool isDefault = false) {
            _name = name;
            _isDefault = IsDefault;
            _modEnabledStates = new Dictionary<string, bool>();
        }

        // Public Functions

        public bool IsModEnabled(ModModel mod) {
            if (!ModEnabledStates.ContainsKey(mod.ID)) {
                Log.Error($"Tried to get enabled state for mod ({mod.Name}) that isn't in profile {this}");
                return false;
            }

            return ModEnabledStates[mod.ID];
        }

        // Private Functions

        // Overrides

        public override string ToString() {
            return $"Profile #{Id} '{Name}'";
        }

        public override bool Equals(object? obj) {
            if (obj is null) return false;
            if (obj is not Profile) return false;
            return ((Profile)obj).Id == Id;
        }
    }
}
