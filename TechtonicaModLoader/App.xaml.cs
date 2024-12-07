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
        private ILoggerService logger;
        private IProgramData programData;
        
        private MainViewModel? mainVeiwModel = null;

        // Constructors

        public App()
        {
            serviceProvider = ConfigureServiceProvider();
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            programData = serviceProvider.GetRequiredService<IProgramData>();
        }

        // Overrides

        protected override void OnStartup(StartupEventArgs e) {
            ConfigureServiceProvider();
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
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddSingleton<IProgramData, ProgramData>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton<IProfileManager, ProfileManager>();
            services.AddSingleton<IThunderStore, ThunderStore>();
            services.AddSingleton<IModFilesManager, ModFilesManager>();
            services.AddSingleton<IDebugUtils, DebugUtils>();

            return services.BuildServiceProvider();
        }

        private void DoStartupProcess() {
            programData.FilePaths.CreateFolderStructure();
            programData.FilePaths.GenerateResources();

            logger = serviceProvider.GetRequiredService<ILoggerService>();
            logger.Info("Logger started");

            mainVeiwModel = new MainViewModel(serviceProvider);
            logger.Info($"MainViewModel loaded");
            logger.Info($"Running V{programData.ProgramVersion.Major}.{programData.ProgramVersion.Minor}.{programData.ProgramVersion.Build}");
        }
    }

}
