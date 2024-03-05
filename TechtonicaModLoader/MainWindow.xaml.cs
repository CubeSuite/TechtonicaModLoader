using MyLogger;
using SharpVectors.Scripting;
using System;
using System.Collections.Generic;
using System.Configuration;
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
using System.Windows.Threading;
using TechtonicaModLoader.Modes;
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader
{
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        // ToDo: Rename Mod, ModManager and ProgramData.FilePaths.ModSaveFile
        // ToDo: Set colours in App.xaml
        // ToDo: Place SVG Files For Control Box
        // ToDo: Rename Namespace In MyGlowBorder.xaml
        // ToDo: Set Width and Height in ProgramData.cs

        // Objects & Variables
        public static MainWindow current => (MainWindow)Application.Current.MainWindow;
        private static DispatcherTimer autoSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };

        // Events

        private void OnProgramLoaded(object sender, RoutedEventArgs e) {
            titleLabel.Content = $"{ProgramData.programName} - V{ProgramData.versionText}";
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;

            ProgramData.FilePaths.CreateFolderStructure();
            ProgramData.FilePaths.GenerateResources();

            InitialiseLogger();
            Log.Info($"Log initialised at {DateTime.Now}");
            if (ProgramData.isDebugBuild) Log.Warning("This is a debug build");

            loadData();
            Log.Info("Data Loaded");

            InitialiseAutoSaveTimer();
        }

        private async void OnProgramClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            saveData();
            await BackupManager.AutoBackup();
            Log.Info("Data saved on program close");
        }

        private void OnAutoSaveTimerTick(object sender, EventArgs e) {
            saveData();
        }

        // Public Functions

        // Private Functions

        private void InitialiseLogger() {
            Log.logPath = ProgramData.FilePaths.logFile;
            Log.logDebugToFile = ProgramData.logDebugMessages || ProgramData.isDebugBuild;
        }

        private void InitialiseAutoSaveTimer() {
            autoSaveTimer.Tick += OnAutoSaveTimerTick;
            autoSaveTimer.Start();
        }

        private void saveData() {
            if (ProgramData.safeToSave) {
                try {
                    Settings.Save();
                    Log.Debug("Settings saved");
                    ModManager.saveData();
                    Log.Debug("ModManager saved");
                    Log.Debug("Data saved");
                }
                catch (Exception error) {
                    Log.Error($"Error occurred while trying to save data: ");
                    Log.Error(error.Message);
                    Log.Error(error.StackTrace);
                }
            }
            else {
                Log.Warning("Save skipped");
            }
        }

        private void loadData() {
            Settings.Load();
            Log.Debug("Settings loaded");
            ModManager.loadData();
            Log.Debug("ModManager loaded");
            ProgramData.safeToSave = true;
        }
    }
}
