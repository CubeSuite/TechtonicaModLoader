using System;
using System.Collections.Generic;
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
    public partial class IntSettingPanel : UserControl
    {
        public IntSettingPanel(IntSetting setting) {
            InitializeComponent();
            settingName = setting.name;
            nameLabel.Content = setting.name;
            desciptionLabel.Text = setting.description;
            inputBox.Value = setting.value;
        }

        public IntSettingPanel(IntConfigOption option) {
            InitializeComponent();
            settingName = option.name;
            nameLabel.Content = option.name;
            desciptionLabel.Text = $"{option.optionType}: {option.GetDescription()}";
            inputBox.Value = option.value;
            type = "ConfigOption";
        }

        // Objects & Variables
        private string settingName;
        private string type = "TMLSetting";

        // Events

        private void OnInputBoxValueChanged(object sender, EventArgs e) {
            if (type == "TMLSetting") {
                Settings.userSettings.SetSetting(settingName, inputBox.Value);
            }
            else {
                ModConfig.activeConfig.UpdateSetting(settingName, inputBox.Value);
            }
        }
    }
}
