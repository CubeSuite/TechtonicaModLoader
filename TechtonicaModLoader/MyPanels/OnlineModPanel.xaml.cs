using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TechtonicaModLoader.Modes;

namespace TechtonicaModLoader.MyPanels
{
    /// <summary>
    /// Interaction logic for OnlineModPanel.xaml
    /// </summary>
    public partial class OnlineModPanel : UserControl
    {
        public OnlineModPanel() {
            InitializeComponent();
        }

        public OnlineModPanel(Mod modToShow) {
            InitializeComponent();
            showMod(modToShow);
        }

        // Objects & Variables

        public Mod mod;

        // Events

        private void OnViewModPageClicked(object sender, EventArgs e) {
            GuiUtils.OpenURL(mod.link);
        }

        private async void OnDownloadClicked(object sender, EventArgs e) {
            //GuiUtils.ShowDownloadingGui(mod);
            // ToDo: Show Downloading gui 
            ModManager.AddMod(mod);
            await mod.DownloadAndInstall();
            MainWindow.current.RefreshCurrentModList();
        }

        // Public Functions

        public void showMod(Mod modToShow) {
            mod = modToShow;
            nameLabel.Content = modToShow.name;
            taglineLabel.Text = modToShow.tagLine;
            icon.Source = new BitmapImage(new Uri(modToShow.iconLink));
        }
    }
}
