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
using TechtonicaModLoader.Modes;
using TechtonicaModLoader.MyClasses;

namespace TechtonicaModLoader.MyPanels
{
    /// <summary>
    /// Interaction logic for InstalledModsPanel.xaml
    /// </summary>
    public partial class InstalledModsPanel : UserControl
    {
        public InstalledModsPanel() {
            InitializeComponent();
        }

        // Objects & Variables
        public List<Mod> modsOnDisplay = new List<Mod>();

        // Events

        private void OnPanelLoaded(object sender, RoutedEventArgs e) {
            Profile profile = ProfileManager.GetActiveProfile();
            profile.SortMods(ProgramData.currentSortOption);
            Log.Debug($"Showing {profile.mods.Count} installed mods");
            modsPanel.Children.Clear();
            foreach(string id in profile.mods.Keys) {
                Log.Debug($"Creating panel for installed mod {id}");
                modsPanel.Children.Add(new InstalledModPanel(id) { Margin = new Thickness(4, 4, 4, 0) });
                modsOnDisplay.Add(ModManager.GetMod(id));
            }
        }

        // Public Functions

        public void SearchModsList(string searchTerm) {
            modsPanel.Children.Clear();
            List<Mod> results = modsOnDisplay.Where(mod => mod.AppearsInSearch(searchTerm)).ToList();
            foreach(Mod mod in results) {
                modsPanel.Children.Add(new InstalledModPanel(mod) { Margin = new Thickness(4, 4, 4, 0) });
            }
        }
    }
}
