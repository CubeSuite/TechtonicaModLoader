using Newtonsoft.Json;

namespace TechtonicaModLoader.MVVM.Settings.ViewModels
{
    public class EnumSetting<T> : SettingBase where T : Enum
    {
        // Members
        private T _value;
        private T _defaultValue;

        // Properties

        public T Value {
            get => _value;
            set {
                _value = value;
                FireSettingUpdated();
            }
        }

        [JsonIgnore] public T[] Options => (T[])Enum.GetValues(typeof(T)); 

        // Constructors

        public EnumSetting(string name, string description, string category, T defaultValue, bool isVisible) : base(name, description, category, isVisible) {
            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        // Public Functions

        public override void RestoreDefault() {
            Value = _defaultValue;
        }
    }
}
