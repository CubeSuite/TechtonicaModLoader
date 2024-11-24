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
        private readonly MainViewModel _mainVeiwModel;
        private readonly ProfileManager _profileManager;
        private readonly IDialogService _dialogService;

        // Constructor

        public App() {
            _dialogService = new DialogService();
            _profileManager = new ProfileManager(_dialogService);
            _mainVeiwModel = new MainViewModel(
                _profileManager,
                _dialogService,
                new Thunderstore(_profileManager)
            );
        }

        // Overrides

        protected override void OnStartup(StartupEventArgs e) {
            
            DoStartupProcess();

            _mainVeiwModel.SelectedModList = Settings.UserSettings?.DefaultModList.Value;
            _mainVeiwModel.SelectedSortOption = Settings.UserSettings?.DefaultModListSortOption.Value;

            MainWindow = new MainWindow() {
                DataContext = _mainVeiwModel
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
            Settings.Load(_dialogService);
            Log.Info("Settings loaded");

            _profileManager.Load();
            Log.Info("ProfileManager loaded");
        }
    }

}
