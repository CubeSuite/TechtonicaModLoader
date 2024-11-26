using System.Configuration;
using System.Data;
using System.Windows;
using TechtonicaModLoader.MVVM;
using TechtonicaModLoader.MVVM.Settings.ViewModels;
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
        private readonly SettingsWindowViewModel _settingsWindowViewModel;
        private readonly IDialogService _dialogService;
        private UserSettings userSettings;
        private ProfileManager? profileManager;
        private ThunderStore? thunderStore;

        public App() {
            _dialogService = new DialogService();
            _settingsWindowViewModel = new SettingsWindowViewModel(_dialogService);
        }

        // Overrides

        protected override void OnStartup(StartupEventArgs e) {

            DoStartupProcess();

            MainWindow = new MainWindow() {
                DataContext = mainVeiwModel
            };

            MainWindow.Show();

            // TODO: Here would be a good place to init any settings.

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

            userSettings.Load();
            Log.Info("Settings loaded");

            profileManager = new ProfileManager(_dialogService, userSettings);
            profileManager.Load();
            Log.Info("ProfileManager loaded");

            thunderStore = new ThunderStore(_dialogService, profileManager);
            thunderStore.Load();
            Log.Info($"ThunderStore loaded");

            mainVeiwModel = new MainViewModel(_dialogService, _settingsWindowViewModel, profileManager, thunderStore);
            mainVeiwModel.SelectedModList = userSettings?.DefaultModList;
            mainVeiwModel.SelectedSortOption = userSettings?.DefaultModListSortOption;
            Log.Info($"MainViewModel loaded");
            Log.Info($"Running V{ProgramData.ProgramVersion.Major}.{ProgramData.ProgramVersion.Minor}.{ProgramData.ProgramVersion.Build}");
        }
    }

}
