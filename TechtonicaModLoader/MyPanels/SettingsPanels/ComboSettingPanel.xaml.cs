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
    public partial class ComboSettingPanel : UserControl
    {
        public ComboSettingPanel(ComboSetting setting) {
            InitializeComponent();
            settingName = setting.name;
            nameLabel.Content = setting.name;
            descriptionLabel.Text = setting.description;
            inputBox.SetItems(setting.values);

            string item = string.IsNullOrEmpty(setting.value) ? setting.defaultValue : setting.value;
            inputBox.SetItem(item);
        }

        // Objects & Variables
        private string settingName;

        // Events

        private void OnInputBoxSelectedItemChanged(object sender, EventArgs e) {
            Settings.userSettings.SetSetting(settingName, inputBox.SelectedItem);
        }
    }
}
