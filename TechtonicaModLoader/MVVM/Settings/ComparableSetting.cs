namespace TechtonicaModLoader.MVVM.Settings.ViewModels
{
    public class ComparableSetting<T> : SettingBase where T : IComparable<T>
    {
        // Members

        private T _value;
        private T _defaultValue;
        private T? _min;
        private T? _max;

        private static readonly Type[] typesToCheck = [typeof(int), typeof(float), typeof(double)];

        // Properties

        public T Value {
            get => _value;
            set {
                if (typesToCheck.Contains(typeof(T))) {
                    if (_min != null && value.CompareTo(_min) < 0) {
                        Log.Warning($"Tried to set setting '{Name}' to '{value}' which is less than min '{_min}'");
                        value = _min;
                    }

                    if (_max != null && value.CompareTo(_max) > 0) {
                        Log.Warning($"Tried to set setting '{Name}' to '{value}' which is greater than max '{_max}'");
                        value = _max;
                    }
                }

                _value = value;
                FireSettingUpdated();
            }
        }

        // Constructors

        public ComparableSetting(string name, string description, string category, T defaultValue, T min, T max, bool isVisible) : base(name, description, category, isVisible) {
            _value = defaultValue;
            _defaultValue = defaultValue;
            _min = min;
            _max = max;
        }

        // Public Functions

        public override void RestoreDefault() {
            Value = _defaultValue;
        }
    }
}
