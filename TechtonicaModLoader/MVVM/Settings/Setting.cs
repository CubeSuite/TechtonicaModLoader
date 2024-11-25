using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.Settings
{
    public class Setting<T> : SettingBase
    {
        // Members

        private T _value;
        private T _defaultValue;

        private static readonly Type[] typesToCheck = [typeof(int), typeof(float), typeof(double)];


        // Properties

        public T Value {
            get => _value;
            set {
                _value = value;
                FireSettingUpdated();
            }
        }

        // Constructors

        public Setting(string name, string description, string category, T defaultValue) : base(name, description, category) {
            _value = defaultValue;
            _defaultValue = defaultValue;
        }

        // Public Functions

        public override void RestoreDefault() {
            Value = _defaultValue;
        }
    }
}
