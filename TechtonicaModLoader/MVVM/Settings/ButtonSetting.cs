using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Settings.ViewModels
{
    public class ButtonSetting : SettingBase
    {
        // Members
        private string _buttonText;
        private Action<UserSettings> _onClick;

        // Properties
        public string ButtonText => _buttonText;
        public Action<UserSettings> OnClick => _onClick;

        // Public Functions

        public override void RestoreDefault() { }

        // Constructors

        public ButtonSetting(string name, string description, string category, string buttonText, Action<UserSettings> onClick, bool isVisible) : base(name, description, category, isVisible) {
            _buttonText = buttonText;
            _onClick = onClick;
        }
    }
}
