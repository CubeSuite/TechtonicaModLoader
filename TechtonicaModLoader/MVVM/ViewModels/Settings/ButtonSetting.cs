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
        // Properties

        public string ButtonText { get; }
        public Action OnClick { get; }

        // Commands

        [RelayCommand]
        private void ExecuteButtonAction() {
            OnClick?.Invoke();
        }

        // Constructors

        public ButtonSetting(string name, string description, string category, string buttonText, Action onClick) : base(name, description, category) {
            ButtonText = buttonText;
            OnClick = onClick;
        }
    }
}
