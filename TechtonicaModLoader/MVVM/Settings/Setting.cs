
namespace TechtonicaModLoader.MVVM.Settings.ViewModels
{
    public class Setting<T> : SettingBase
    {
        // Members
        private readonly Func<T> _getValue;
        private readonly Action<T> _setValue;


        // Properties

        public T Value {
            get => _getValue();
            set => _setValue(value);
        }

        // Constructors

        public Setting(string name, string description, string category, Func<T> getValue, Action<T> setValue, bool isVisible) : base(name, description, category, isVisible) {
            _getValue = getValue;
            _setValue = setValue;
        }

        // Public Functions

        public override void RestoreDefault() {
        }
    }
}
