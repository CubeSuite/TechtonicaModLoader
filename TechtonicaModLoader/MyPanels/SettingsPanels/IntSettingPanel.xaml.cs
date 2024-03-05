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

        // Objects & Variables
        private string settingName;

        // Events

        private void OnInputBoxValueChanged(object sender, EventArgs e) {
            Settings.userSettings.SetSetting(settingName, inputBox.Value);
        }
    }
}
