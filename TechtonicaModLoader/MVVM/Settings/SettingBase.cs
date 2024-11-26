using Newtonsoft.Json;

namespace TechtonicaModLoader.MVVM.Settings.ViewModels
{
    public abstract class SettingBase
    {
        // Members
        private string _name;
        private string _description;
        private string _category;
        private bool _isVisible;

        // Properties

        [JsonIgnore] public string Name => _name;
        [JsonIgnore] public string Description => _description;
        [JsonIgnore] public string Category => _category;
        [JsonIgnore] public bool IsVisible => _isVisible = false;

        // Events

        public event Action SettingUpdated = delegate { }; // ToDo: test this

        // Constructors

        public SettingBase(string name, string description, string category, bool isVisible) {
            _name = name;
            _description = description;
            _category = category;
            _isVisible = isVisible;
        }

        // Public Functions

        public abstract void RestoreDefault();

        // Protected Functions

        protected void FireSettingUpdated() {
            SettingUpdated?.Invoke();
        }
    }
}
