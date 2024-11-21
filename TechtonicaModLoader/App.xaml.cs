using System.Windows;
using TechtonicaModLoader.Models;
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
        private DialogService? dialogService = null;

        private readonly IProfileManager _profileManager = new ProfileManager();

        // Overrides

        protected override void OnStartup(StartupEventArgs e) {
            dialogService = new DialogService();
            
            DoStartupProcess();

            mainVeiwModel = new MainViewModel(
                _profileManager, 
                dialogService, 
                new Thunderstore(_profileManager)
            );

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
            Settings.Load(dialogService ?? new DialogService());
            Log.Info("Settings loaded");

            _profileManager.Load();
            Log.Info("ProfileManager loaded");
        }
    }

}
