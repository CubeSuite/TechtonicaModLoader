using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Settings
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

        public ButtonSetting(string name, string description, string category, string buttonText, Action<UserSettings> onClick) : base(name, description, category) {
            _buttonText = buttonText;
            _onClick = onClick;
        }
    }
}
