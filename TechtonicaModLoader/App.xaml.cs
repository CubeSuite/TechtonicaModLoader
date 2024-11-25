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
        private IDialogService? dialogService;
        private UserSettings? userSettings;
        private ProfileManager? profileManager;
        private ThunderStore? thunderStore;

        // Overrides

        protected override void OnStartup(StartupEventArgs e) {
            DoStartupProcess();

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

            dialogService = new DialogService();

            userSettings = new UserSettings(dialogService);
            userSettings.Load();
            Log.Info("Settings loaded");

            profileManager = new ProfileManager(dialogService, userSettings);
            profileManager.Load();
            Log.Info("ProfileManager loaded");

            thunderStore = new ThunderStore(dialogService, profileManager);
            thunderStore.Load();
            Log.Info($"ThunderStore loaded");

            mainVeiwModel = new MainViewModel(dialogService, userSettings, profileManager, thunderStore);
            mainVeiwModel.SelectedModList = userSettings?.DefaultModList.Value;
            mainVeiwModel.SelectedSortOption = userSettings?.DefaultModListSortOption.Value;
            Log.Info($"MainViewModel loaded");
            Log.Info($"Running V{ProgramData.ProgramVersion.Major}.{ProgramData.ProgramVersion.Minor}.{ProgramData.ProgramVersion.Build}");
        }
    }

}
