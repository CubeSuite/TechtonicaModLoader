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
    public partial class ButtonSettingPanel : UserControl
    {
        public ButtonSettingPanel(ButtonSetting setting) {
            InitializeComponent();
            nameLabel.Content = setting.name;
            desciptionLabel.Text = setting.description;
            settingAction = setting.OnClick;
            settingButton.ButtonText = setting.buttonText;
        }

        // Objects & Variables
        private Action settingAction;

        // Events

        private void OnSettingButtonClicked(object sender, EventArgs e) {
            settingAction();
        }
    }
}
