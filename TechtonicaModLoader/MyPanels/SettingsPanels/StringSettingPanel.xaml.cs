using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TechtonicaModLoader.MyClasses;

namespace TechtonicaModLoader.MyPanels.SettingsPanels
{
    /// <summary>
    /// Interaction logic for StringSettingPanel.xaml
    /// </summary>
    public partial class StringSettingPanel : UserControl
    {
        public StringSettingPanel(StringSetting setting) {
            InitializeComponent();
            settingName = setting.name;
            nameLabel.Content = setting.name;
            descriptionLabel.Text = setting.description;
            inputBox.Input = setting.value;
            inputBox.Hint = setting.name;
        }

        public StringSettingPanel(KeyboardShortcutConfigOption option) {
            type = ConfigOptionTypes.keyboardShortcutOption;
            InitializeComponent();
            settingName = option.name;
            nameLabel.Content = option.name;
            descriptionLabel.Text = $"{option.optionType}: {option.GetDescription()}";
            inputBox.Input = option.value;
            inputBox.Hint = option.name;
        }

        public StringSettingPanel(KeyCodeConfigOption option) {
            type = ConfigOptionTypes.keycodeOption;
            InitializeComponent();
            settingName = option.name;
            nameLabel.Content = option.name;
            descriptionLabel.Text = $"{option.optionType}: {option.GetDescription()}";
            inputBox.Input = option.value;
            inputBox.Hint = option.name;
        }

        public StringSettingPanel(StringConfigOption option) {
            type = ConfigOptionTypes.stringOption;
            InitializeComponent();
            settingName = option.name;
            nameLabel.Content = option.name;
            descriptionLabel.Text = $"{option.optionType}: {option.GetDescription()}";
            inputBox.Input = option.value;
            inputBox.Hint = option.name;
        }

        public StringSettingPanel(FloatConfigOption option) {
            type = ConfigOptionTypes.floatOption;
            InitializeComponent();
            settingName = option.name;
            nameLabel.Content = option.name;
            descriptionLabel.Text = $"{option.optionType}: {option.GetDescription()}";
            inputBox.Input = option.value.ToString();
            inputBox.Hint = option.name;
        }

        public StringSettingPanel(DoubleConfigOption option) {
            type = ConfigOptionTypes.doubleOption;
            InitializeComponent();
            settingName = option.name;
            nameLabel.Content = option.name;
            descriptionLabel.Text = $"{option.optionType}: {option.GetDescription()}";
            inputBox.Input = option.value.ToString();
            inputBox.Hint = option.name;
        }

        // Objects & Variables
        public string settingName;
        private string type = "TMLSetting";

        // Events

        private void OnInputBoxTextChanged(object sender, EventArgs e) {
            switch (type) {
                case "TMLSetting": Settings.userSettings.SetSetting(settingName, inputBox.Input); break;
                case ConfigOptionTypes.stringOption: ModConfig.activeConfig.UpdateSetting(settingName, inputBox.Input); break;
                case ConfigOptionTypes.keyboardShortcutOption: ModConfig.activeConfig.UpdateSetting(settingName, inputBox.Input); break;
                case ConfigOptionTypes.keycodeOption: ModConfig.activeConfig.UpdateSetting(settingName, inputBox.Input); break;

                case ConfigOptionTypes.floatOption:
                    if(float.TryParse(inputBox.Input, out float floatValue)) {
                        ModConfig.activeConfig.UpdateSetting(settingName, floatValue);
                    }
                    else if(!string.IsNullOrEmpty(inputBox.Input)){
                        GuiUtils.ShowErrorMessage("Invalid Number", $"TML couldn't process '{inputBox.Input}' into a number");
                    }
                    break;

                case ConfigOptionTypes.doubleOption:
                    if (double.TryParse(inputBox.Input, out double doubleValue)) {
                        ModConfig.activeConfig.UpdateSetting(settingName, doubleValue);
                    }
                    else if (!string.IsNullOrEmpty(inputBox.Input)) {
                        GuiUtils.ShowErrorMessage("Invalid Number", $"TML couldn't process '{inputBox.Input}' into a number");
                    }
                    break;
            }
        }
    }
}
