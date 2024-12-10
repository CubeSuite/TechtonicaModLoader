using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;
using System.Windows;
using TechtonicaModLoader.MVVM;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Stores.Settings;

namespace TechtonicaModLoader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Members

        private readonly IServiceProvider serviceProvider;
        private readonly ILoggerService logger;
        private readonly IProgramData programData;
        private readonly IUserSettings userSettings;

        private MainViewModel? mainVeiwModel = null;

        // Constructors

        public App()
        {
            serviceProvider = ConfigureServiceProvider();
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            programData = serviceProvider.GetRequiredService<IProgramData>();
            userSettings = serviceProvider.GetRequiredService<IUserSettings>();
       }

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

        private ServiceProvider ConfigureServiceProvider() {
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<IUserSettings, UserSettings>();
            services.AddSingleton<ISettingsFileHandler, SettingsFileHandler>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IProgramData, ProgramData>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IProfileManager, ProfileManager>();
            services.AddSingleton<IThunderStore, ThunderStore>();
            services.AddSingleton<IModFilesManager, ModFilesManager>();
            services.AddSingleton<IDebugUtils, DebugUtils>();
            services.AddTransient<IFileSystem, FileSystem>();

            return services.BuildServiceProvider();
        }

        private void DoStartupProcess() {
            programData.FilePaths.BepInExFolder = $"{userSettings.GameFolder}\\BepInEx";
            programData.FilePaths.CreateFolderStructure();
            programData.FilePaths.GenerateResources();

            mainVeiwModel = new MainViewModel(serviceProvider);
            logger.Info($"MainViewModel loaded");
            logger.Info($"Running V{programData.ProgramVersion.Major}.{programData.ProgramVersion.Minor}.{programData.ProgramVersion.Build}");
        }
    }

}
