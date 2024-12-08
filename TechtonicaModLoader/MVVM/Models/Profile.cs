using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Services.ThunderstoreModels;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Models
{
    public partial class Profile : ObservableObject
    {
        // Properties

        [ObservableProperty] private int _id = -1;
        [ObservableProperty] private string _name;
        [ObservableProperty] private Dictionary<string, bool> _modEnabledStates;

        public bool IsDefault { get; }

        [JsonIgnore]
        public IProfileManager ProfileManager { get; set; }

        // Constructors

        public Profile(IProfileManager profileManager, string name, bool isDefault = false) {
            ProfileManager = profileManager;
            _name = name;
            IsDefault = isDefault;
            _modEnabledStates = new Dictionary<string, bool>();
        }

        // Public Functions

        public bool IsModEnabled(Mod mod) {
            if (string.Equals(ProfileManager.ActiveProfile.Name, StringResources.ProfileVanilla, StringComparison.CurrentCulture)) return false;
            return ModEnabledStates.TryGetValue(mod.ID, out bool value) && value;
        }

        public void AddMod(ThunderStoreMod mod) {
            ModEnabledStates.Add(mod.uuid4, false); // ToDo: Error handle
            ProfileManager.Save();
        }

        public void ToggleMod(string id) {
            ModEnabledStates[id] = !ModEnabledStates[id];
            ProfileManager.Save();
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

        public override int GetHashCode() {
            return HashCode.Combine(Id, Name);
        }
    }
}
