using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader.MyPanels
{
    /// <summary>
    /// Interaction logic for BackupPanel.xaml
    /// </summary>
    public partial class BackupPanel : UserControl
    {
        public BackupPanel(string name) {
            InitializeComponent();
            backupName = name;
            nameLabel.Text = name.Replace(",", "/").Replace("-", ":");
        }

        // Objects & Variables
        private string backupName;

        // Events

        private void OnRestoreClicked(object sender, EventArgs e) {
            if (GuiUtils.GetStringFromUser("Confirm Restore From Backup", "Enter 'Restore' (Case Sensitive) to confirm, or anything else to cancel") == "Restore") {
                BackupManager.CreateBackup();
                GuiUtils.ShowInfoMessage("Auto Backup Complete", $"{ProgramData.programName} has backed up your data.", "Continue");
                BackupManager.RestoreBackup(backupName);
                GuiUtils.ShowInfoMessage("Restore From Backup Complete", $"Your data has been reset to this backup. {ProgramData.programName} will now restart.", "Close");
                ProgramData.safeToSave = false;
                string appPath = Process.GetCurrentProcess().MainModule.FileName;
                Process.Start(appPath);
                Application.Current.Shutdown();
            }
            else {
                GuiUtils.ShowInfoMessage("Restore From Backup Canceled", $"{ProgramData.programName} has not restored your data to this backup.", "Close");
            }
        }

        private void OnDeleteClicked(object sender, EventArgs e) {
            if (GuiUtils.GetUserConfirmation("Delete Backup?", "Are you sure you want to delete this backup? This cannot be undone.")) {
                BackupManager.DeleteBackup($"{Settings.userSettings.backupsFolder.value}\\{backupName}");
                ((StackPanel)Parent).Children.Remove(this);
            }
        }
    }
}
