using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public partial class ButtonSetting : SettingBase
    {
        // Members

        private UserSettings userSettings;

        // Properties

        public string ButtonText { get; }
        public Action<UserSettings> OnClick { get; }

        // Commands

        [RelayCommand]
        private void ExecuteButtonAction() {
            OnClick?.Invoke(userSettings);
        }

        // Constructors

        public ButtonSetting(string name, string description, string category, string buttonText, Action<UserSettings> onClick, UserSettings userSettings) : base(name, description, category) {
            ButtonText = buttonText;
            OnClick = onClick;
            this.userSettings = userSettings;
        }
    }
}
