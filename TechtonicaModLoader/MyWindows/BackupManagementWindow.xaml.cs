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
using System.Windows.Shapes;
using TechtonicaModLoader.MyPanels;

namespace TechtonicaModLoader.MyWindows
{
    /// <summary>
    /// Interaction logic for BackupManagementWindow.xaml
    /// </summary>
    public partial class BackupManagementWindow : Window
    {
        public BackupManagementWindow() {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;
        }

        // Objects & Variables

        // Events

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            RefreshBackupList();
        }

        private async void OnPerformBackupClicked(object sender, EventArgs e) {
            performBackupButton.IsHitTestVisible = false;
            await BackupManager.AutoBackup();
            RefreshBackupList();
            performBackupButton.IsHitTestVisible = true;
        }

        private void OnCloseClicked(object sender, EventArgs e) {
            Close();
        }

        // Private Functions

        private void RefreshBackupList() {
            backupsPanel.Children.Clear();
            List<string> backups = BackupManager.GetAllBackups();
            foreach (string backup in backups) {
                backupsPanel.Children.Add(new BackupPanel(backup) { Margin = new Thickness(4, 2, 4, 2) });
            }
        }

        // Return Functions

        public string result;
        private string GetResult() { return result; }
        public static string ShowBackupManagerWindow() {
            BackupManagementWindow window = new BackupManagementWindow();
            window.ShowDialog();
            return window.GetResult();
        }
    }
}
