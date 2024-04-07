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
    /// Interaction logic for NewModsPanel.xaml
    /// </summary>
    public partial class NewModsPanel : UserControl
    {
        public NewModsPanel() {
            InitializeComponent();
        }

        // Objects & Variables
        public List<Mod> modsOnDisplay = new List<Mod>();

        // Events

        private async void OnPanelLoaded(object sender, RoutedEventArgs e) {
            Profile profile = ProfileManager.GetActiveProfile();
            List<Mod> mods = await ThunderStore.GetAllMods();
            mods = mods.Where(mod => !profile.HasMod(mod)).ToList();
            
            string seenMods = Settings.userSettings.seenMods.value;
            if(!string.IsNullOrEmpty(seenMods) ) {
                mods = mods.Where(mod => !seenMods.Contains(mod.id)).ToList();
                if(mods.Count == 0) {
                    infoLabel.Content = "No New Mods";
                    return;
                }
            }

            mods = ModManager.SortModList(mods, ProgramData.currentSortOption);
            modsOnDisplay = mods;
            Log.Debug($"Showing {mods.Count} new mods");
            modsPanel.Children.Clear();
            foreach (Mod mod in mods) {
                Log.Debug($"Creating panel for new mod '{mod.name}'");
                modsPanel.Children.Add(new OnlineModPanel(mod) { Margin = new Thickness(4, 4, 4, 0) });
                if (string.IsNullOrEmpty(seenMods) || !seenMods.Contains(mod.id)) {
                    if (string.IsNullOrEmpty(seenMods)) {
                        seenMods = mod.id;
                    }
                    else {
                        seenMods += $"|{mod.id}";
                    }

                    Log.Debug($"Added '{mod.name}' to seen mods");
                    Settings.userSettings.SetSetting(SettingNames.seenMods, seenMods, false);
                }
            }
        }

        // Public Functions

        public void SearchModsList(string searchTerm) {
            if(modsOnDisplay.Count == 0) return;

            modsPanel.Children.Clear();
            List<Mod> results = modsOnDisplay.Where(mod => mod.AppearsInSearch(searchTerm)).ToList();
            foreach (Mod mod in results) {
                modsPanel.Children.Add(new OnlineModPanel(mod) { Margin = new Thickness(4, 4, 4, 0) });
            }
        }
    }
}
