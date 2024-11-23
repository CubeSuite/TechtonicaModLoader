using System.Configuration;
using System.Data;
using System.Windows;
using TechtonicaModLoader.MVVM;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Members

        private Log? logger = null;
        private MainViewModel? mainVeiwModel = null;

        // Overrides

        protected override void OnStartup(StartupEventArgs e) {
            DoStartupProcess();

            mainVeiwModel = new MainViewModel();

            mainVeiwModel.SelectedModList = Settings.UserSettings?.DefaultModList.Value;
            mainVeiwModel.SelectedSortOption = Settings.UserSettings?.DefaultModListSortOption.Value;

            MainWindow = new MainWindow() {
                DataContext = mainVeiwModel
            };

            MainWindow.Show();
            base.OnStartup(e);
        }

        // Private Functions

        private void DoStartupProcess() {
            logger = new Log();
            Log.Info("Logger started");
            ProgramData.FilePaths.CreateFolderStructure();
            Log.Info("Created folder structure");
            ProgramData.FilePaths.GenerateResources();
            Log.Info("Generated resources");
            Settings.Load();
            Log.Info("Settings loaded");

            ThunderStore.Instance.Load();
            Log.Info($"ThunderStore loaded");

            ProfileManager.Instance.Load();
            Log.Info("ProfileManager loaded");
        }
    }

}
