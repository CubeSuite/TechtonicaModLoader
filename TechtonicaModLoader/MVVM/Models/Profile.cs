using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Services.ThunderstoreModels;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Models
{
    public partial class Profile : ObservableObject
    {
        // Members

        ProfileManager _profileManager;

        // Properties

        [ObservableProperty] private int _id = -1;
        [ObservableProperty] private string _name;
        [ObservableProperty] private Dictionary<string, bool> _modEnabledStates;

        private bool _isDefault;
        public bool IsDefault => _isDefault;

        [JsonIgnore]
        public ProfileManager ProfileManager {
            get => _profileManager;
            set => _profileManager = value;
        }

        // Constructors

        public Profile(ProfileManager profileManager, string name, bool isDefault = false) {
            this._profileManager = profileManager;
            _name = name;
            _isDefault = IsDefault;
            _modEnabledStates = new Dictionary<string, bool>();
        }

        // Public Functions

        public bool IsModEnabled(Mod mod) {
            if (string.Equals(_profileManager.ActiveProfile.Name, StringResources.ProfileVanilla, StringComparison.CurrentCulture)) return false;

            if (!ModEnabledStates.TryGetValue(mod.ID, out bool value)) return false;
            return value;
        }

        public void AddMod(ThunderStoreMod mod) {
            ModEnabledStates.Add(mod.uuid4, false); // ToDo: Error handle
            _profileManager.Save();
        }

        public void ToggleMod(string id) {
            ModEnabledStates[id] = !ModEnabledStates[id];
            _profileManager.Save();
        }

        public IEnumerable<string> GetModIDsToDeploy() {
            return ModEnabledStates.Where(pair => pair.Value).Select(pair => pair.Key);
        }

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
