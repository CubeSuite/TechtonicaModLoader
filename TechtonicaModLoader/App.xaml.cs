using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using TechtonicaModLoader.MVVM;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Windows.Settings;

namespace TechtonicaModLoader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Members

        private IServiceProvider serviceProvider;

        private UserSettings? userSettings;
        private ILoggerService? logger = null;
        private IProgramData? programData = null;
        private IDialogService? dialogService;
        private IProfileManager? profileManager;
        private IThunderStore? thunderStore;
        private IModFilesManager? modFilesManager;
        
        private MainViewModel? mainVeiwModel = null;
        private SettingsWindowViewModel? settingsWindowViewModel;

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

        //private void ConfigureServiceProvider() {
        //    ServiceCollection services = new ServiceCollection();

        //    services.AddSingleton<IUserSettings, UserSettings>();
        //    services.AddSingleton<ILoggerService, Log>();
        //}

        private void DoStartupProcess() {
            programData = new ProgramDataStore();
            ProgramData.Initialise(programData);

            logger = new LoggerService();
            Log.Initialise(logger);
            Log.Info("Logger started");
            
            ProgramData.FilePaths.CreateFolderStructure();
            Log.Info("Created folder structure");
            ProgramData.FilePaths.GenerateResources();
            Log.Info("Generated resources");

            dialogService = new DialogService();

            userSettings = new UserSettings();
            userSettings.Load();
            Log.Info("Settings loaded");

            settingsWindowViewModel = new SettingsWindowViewModel(userSettings, dialogService);
            Log.Info("Created SettingsWindowViewModel");

            profileManager = new ProfileManager(dialogService, userSettings);
            profileManager.Load();
            Log.Info("ProfileManager loaded");

            modFilesManager = new ModFilesManager(dialogService, profileManager);
            Log.Info("ModFilesManager loaded");

            thunderStore = new ThunderStore(dialogService, profileManager, modFilesManager, settingsWindowViewModel);
            thunderStore.Load();
            Log.Info($"ThunderStore loaded");

            mainVeiwModel = new MainViewModel(dialogService, settingsWindowViewModel, profileManager, thunderStore, modFilesManager);
            mainVeiwModel.SelectedModList = userSettings?.DefaultModList;
            mainVeiwModel.SelectedSortOption = userSettings?.DefaultModListSortOption;
            Log.Info($"MainViewModel loaded");
            Log.Info($"Running V{ProgramData.ProgramVersion.Major}.{ProgramData.ProgramVersion.Minor}.{ProgramData.ProgramVersion.Build}");
        }
    }

}
