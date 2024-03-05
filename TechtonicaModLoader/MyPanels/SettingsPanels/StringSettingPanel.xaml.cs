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
            desciptionLabel.Text = setting.description;
            inputBox.Input = setting.value;
            inputBox.Hint = setting.name;
        }

        // Objects & Variables
        private string settingName;

        // Events

        private void OnInputBoxTextChanged(object sender, EventArgs e) {
            Settings.userSettings.SetSetting(settingName, inputBox.Input);
        }
    }
}
