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
using TechtonicaModLoader.MyClasses;
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader.MyPanels
{
    public partial class InstalledModPanel : UserControl
    {
        public InstalledModPanel() {
            InitializeComponent();
        }

        public InstalledModPanel(string modID) {
            InitializeComponent();
            ShowMod(ModManager.GetMod(modID));
        }

        public InstalledModPanel(Mod mod) {
            InitializeComponent();
            ShowMod(mod);
        }

        // Objects & Variables

        public string modID;

        // Events

        private void EnabledToggled(object sender, EventArgs e) {
            Mod mod = ModManager.GetMod(modID);
            if (mod.IsDependencyOfAnother(out string dependentMod)) {
                enabledBox.IsChecked = true;
                Log.Debug($"Blocking disable, is dependency");
                GuiUtils.ShowInfoMessage("Can't Disable", $"This mod cannot be disabled as it is a dependency of {dependentMod}. Please disable or delete that mod first.");
                return;
            }

            Profile profile = ProfileManager.GetActiveProfile();
            profile.ToggleMod(modID);
            ProfileManager.UpdateProfile(profile);

            string state = enabledBox.IsChecked ? "enabled" : "disabled";
            Log.Debug($"Set mod '{mod.name}' to '{state}' on profile '{profile.name}'");

            if (enabledBox.IsChecked) {
                mod.Install();
            }
            else {
                mod.Uninstall();
            }
        }

        private async void OnUpdateClicked(object sender, EventArgs e) {
            Mod mod = ModManager.GetMod(modID);
            Log.Debug($"Downloading update for mod '{mod.name}'");
            mod.Uninstall();
            ModManager.DeleteMod(mod);

            //GuiUtils.ShowDownloadingGui(mod);
            mod = await ThunderStore.GetMod(modID);
            ModManager.AddMod(mod);
            await mod.DownloadAndInstall();
        }

        private void OnDonateClicked(object sender, EventArgs e) {
            Mod mod = ModManager.GetMod(modID);
            GuiUtils.OpenURL(mod.donationLink);

            mod.hasDonated = GuiUtils.GetUserConfirmation("Hide Donation Button", "Would you like to hide the donation button for this mod?");
            ModManager.UpdateMod(mod);
        }

        private void OnConfigureClicked(object sender, EventArgs e) {
            Mod mod = ModManager.GetMod(modID);
            ModConfig config = ModConfig.FromFile(mod.configFileLocation);
            ModConfig.activeConfig = config;
            ModConfigWindow.EditActiveConfig();
        }

        private void OnViewModPageClicked(object sender, EventArgs e) {
            GuiUtils.OpenURL(ModManager.GetMod(modID).link);
        }

        private void OnDeleteModClicked(object sender, EventArgs e) {
            Mod mod = ModManager.GetMod(modID);
            if (mod.IsDependencyOfAnother(out string dependentMod)) {
                enabledBox.IsChecked = true;
                Log.Debug($"Blocking disable, is dependency");
                GuiUtils.ShowInfoMessage("Can't Disable", $"This mod cannot be disabled as it is a dependency of {dependentMod}. Please disable or delete that mod first.");
                return;
            }

            if (GuiUtils.GetUserConfirmation("Delete Mod?", "Are you sure you want to delete this mod?")) {
                Profile profile = ProfileManager.GetActiveProfile();
                if (profile.IsModEnabled(modID)) {
                    mod.Uninstall();
                }

                profile.mods.Remove(modID);
                ProfileManager.UpdateProfile(profile);

                File.Delete(mod.zipFileLocation);
                ModManager.DeleteMod(mod);

                StackPanel parent = (StackPanel)Parent;
                parent.Children.Remove(this);
            }
        }

        // Public Functions

        public void ShowMod(Mod mod) {
            modID = mod.id;
            enabledBox.IsChecked = ProfileManager.GetActiveProfile().IsModEnabled(modID);
            enabledBox.IsEditable = mod.canBeToggled;
            modNameLabel.Text = mod.name;

            if (mod.isDeprecated) {
                modNameLabel.Foreground = Brushes.Red;
                modTaglineLabel.Foreground = Brushes.Red;
                modTaglineLabel.Text = "Depricated";
            }
            else {
                modTaglineLabel.Text = mod.tagLine;
            }

            string iconPath = string.IsNullOrEmpty(mod.iconLink) ? "pack://application:,,,/UnknownModIcon.png" : mod.iconLink;
            icon.Source = new BitmapImage(new Uri(iconPath));

            if (!mod.updateAvailable) {
                HideUpdateColumn();
            }

            if (string.IsNullOrEmpty(mod.donationLink) || mod.hasDonated) {
                HideDonateColumn();
            }

            if (!mod.HasConfigFile()) {
                HideConfigureColumn();
            }
        }

        // Private Functions

        private void HideUpdateColumn() {
            updateColumn.Width = new GridLength(0);
            mainGrid.Children.Remove(updateButton);
        }

        private void HideDonateColumn() {
            donateColumn.Width = new GridLength(0);
            mainGrid.Children.Remove(donateButton);
        }

        private void HideConfigureColumn() {
            mainGrid.Children.Remove(configureButton);
            configureColumn.Width = new GridLength(0);
        }
    }
}
