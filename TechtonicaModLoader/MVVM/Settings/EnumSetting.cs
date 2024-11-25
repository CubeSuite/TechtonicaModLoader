using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.Settings
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

        public EnumSetting(string name, string description, string category, T defaultValue) : base(name, description, category) {
            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        // Public Functions

        public override void RestoreDefault() {
            Value = _defaultValue;
        }
    }
}
