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
    public partial class ColourSettingPanel : UserControl
    {
        public ColourSettingPanel(ColourSetting setting) {
            InitializeComponent();
            settingName = setting.name;
            nameLabel.Content = setting.name;
            desciptionLabel.Text = setting.description;
            previewBorder.Background = new SolidColorBrush(setting.value);
        }

        // Objects & Variables
        private string settingName;

        // Events

        private void OnChangeColourClicked(object sender, EventArgs e) {
            System.Windows.Forms.ColorDialog picker = new System.Windows.Forms.ColorDialog();
            if (picker.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                Color colour = Color.FromRgb(picker.Color.R, picker.Color.G, picker.Color.B);
                Settings.userSettings.SetSetting(settingName, colour);
                previewBorder.Background = new SolidColorBrush(colour);
            }
        }
    }
}
