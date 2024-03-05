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
    /// <summary>
    /// Interaction logic for BoolSettingPanel.xaml
    /// </summary>
    public partial class BoolSettingPanel : UserControl
    {
        public BoolSettingPanel(BoolSetting setting) {
            InitializeComponent();
            settingName = setting.name;
            nameLabel.Content = setting.name;
            desciptionLabel.Text = setting.description;
            checkBox.IsChecked = setting.value;
        }

        // Objects & Variables
        private string settingName;

        // Events

        private void OnCheckBoxIsCheckedChanged(object sender, EventArgs e) {
            Settings.userSettings.SetSetting(settingName, checkBox.IsChecked);
        }
    }
}
