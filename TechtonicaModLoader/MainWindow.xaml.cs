using Newtonsoft.Json;
using SharpVectors.Scripting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
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
using TechtonicaModLoader.MyClasses;
using TechtonicaModLoader.MyClasses.Globals;
using TechtonicaModLoader.MyPanels;
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader
{
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        // Objects & Variables
        public static MainWindow current => (MainWindow)Application.Current.MainWindow;
        private static DispatcherTimer autoSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(15) };
        private DispatcherTimer processCheckTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
        private object modsList => modListBorder.Child;

        // Window Events

        private async void OnProgramLoaded(object sender, RoutedEventArgs e) {
            titleLabel.Content = $"{ProgramData.programName} - V{ProgramData.versionText}";
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;

            ProgramData.FilePaths.CreateFolderStructure();
            ProgramData.FilePaths.GenerateResources();

            Log.InitialiseLog();
            Log.Info($"Log initialised at {DateTime.Now}");
            if (ProgramData.isDebugBuild) Log.Warning("This is a debug build");

            await LoadData();
            Log.Info("Data Loaded");
            
            ProgramData.FilePaths.CopyConfigFiles();
            if (!FileStructureUtils.ValidateGameFolder()) {
                Settings.userSettings.findGameFolder.OnClick();

                if (FileStructureUtils.ValidateGameFolder()) {
                    if (!GuiUtils.GetUserConfirmation("Correct Game Folder?",
                                                     $"TML found Techtonica installed in the following location.\n" +
                                                     $"Is this installation you would like to mod?\n{Settings.userSettings.gameFolder.value}")) {
                        SettingsWindow.ShowSettingsWindow();
                    }
                }
            }
                
            if (!FileStructureUtils.ValidateGameFolder()) {
                GuiUtils.ShowInfoMessage("Game Folder Not Found", "TML couldn't find Techtonica's installation location. Please set it in the settings.");
                SettingsWindow.ShowSettingsWindow();
            }

            if (Settings.userSettings.isFirstTimeLaunch.value) {
                await ProfileManager.CreateDefaultProfiles();
            }

            CleanProfiles();

            if(!await ModManager.CheckForUpdates()) {
                sidePanel.Children.Remove(updateAllButton);
            }

            InitialiseProcessCheckTimer();
            InitialiseAutoSaveTimer();
            InitialiseGUI();

            Profile lastProfile = ProfileManager.GetProfileByName(Settings.userSettings.lastProfile.value);
            if(lastProfile != null) {
                Log.Info($"Loading last profile: '{lastProfile.name}'");
            }
            else {
                Log.Info($"Loading default Modded profile");
                lastProfile = ProfileManager.GetProfileByName("Modded");
                ProfileManager.LoadProfile(lastProfile);
            }

            RefreshCurrentModList();

            mainGrid.Visibility = Visibility.Visible;
            launchGameButton.Visibility = Visibility.Visible;
            controlBox.Visibility = Visibility.Visible;
            controlBoxBorder.Visibility = Visibility.Visible;
        }

        private async void OnProgramClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            Settings.userSettings.SetSetting(SettingNames.isFirstTimeLaunch, false, false);
            SaveData();
            await BackupManager.AutoBackup();
            Log.Info("Data saved on program close");
        }

        // Timer Events

        private void OnAutoSaveTimerTick(object sender, EventArgs e) {
            SaveData();
        }

        private void OnProcessCheckTimerTick(object sender, EventArgs e) {
            Process[] techtonicaProcess = Process.GetProcessesByName("Techtonica");
            if (techtonicaProcess.Length != 0) {
                launchGameButton.IsEnabled = false;
                launchGameButton.ButtonText = "Game Running";
                loadingPanel.SetInfo("Game Is Running");
                mainGrid.Visibility = Visibility.Hidden;
                loadingPanel.Visibility = Visibility.Visible;

                if (!WindowSwitcher.gaveFocusSinceLaunch) {
                    WindowSwitcher.FocusGame();
                }
            }
            else {
                if (!launchGameButton.IsEnabled) {
                    string[] configFiles = Directory.GetFiles(ProgramData.FilePaths.bepInExConfigFolder);
                    foreach (string configFile in configFiles) {
                        string newpath = configFile.Replace(ProgramData.FilePaths.bepInExConfigFolder, ProgramData.FilePaths.configsFolder);
                        if (File.Exists(newpath)) File.Delete(newpath);
                        File.Copy(configFile, newpath);
                    }
                }

                launchGameButton.IsEnabled = true;
                launchGameButton.ButtonText = "Launch Game";
                mainGrid.Visibility = Visibility.Visible;
                loadingPanel.Visibility = Visibility.Hidden;
                WindowSwitcher.gaveFocusSinceLaunch = false;
            }
        }

        // UI Events

        private void OnLaunchGameClicked(object sender, EventArgs e) {
            FileStructureUtils.StartGame();
        }

        private async void OnUpdateAllClicked(object sender, EventArgs e) {
            List<Mod> mods = ModManager.GetAllMods();
            mods = mods.Where(mod => mod.updateAvailable).ToList();
            LoadingWindow loader = new LoadingWindow();
            loader.SetProgress("Updating Mods", 1, mods.Count);
            loader.Show();

            await Task.Run(async () => {
                for (int i = 0; i < mods.Count; i++) {
                    Dispatcher.Invoke(delegate () {
                        loader.SetProgress("Updating Mods", i + 1, mods.Count);
                    });

                    Mod mod = mods[i];
                    mod.Uninstall();
                    ModManager.DeleteMod(mod);

                    mod = await ThunderStore.GetMod(mod.id);
                    ModManager.AddMod(mod);
                    await mod.DownloadAndInstall();
                    
                }
            });

            sidePanel.Children.Remove(updateAllButton);
            loader.Close();
            RefreshCurrentModList();
        }

        private async void OnCheckForUpdatesClicked(object sender, EventArgs e) {
            if(await ModManager.CheckForUpdates() && !sidePanel.Children.Contains(updateAllButton)) {
                sidePanel.Children.Insert(0, updateAllButton);
            }
            else if (sidePanel.Children.Contains(updateAllButton)) {
                sidePanel.Children.Remove(updateAllButton);
            }

            RefreshCurrentModList();
        }

        private void OnProfileChanged(object sender, EventArgs e) {
            if (profileBox.SelectedItem == Settings.userSettings.lastProfile.value) return;
            
            searchBar.Input = "";
            ProfileManager.GetActiveProfile().UninstallAll();
            ProfileManager.LoadProfile(ProfileManager.GetProfileByName(profileBox.SelectedItem));
            Settings.userSettings.SetSetting(SettingNames.lastProfile, profileBox.SelectedItem);
            ProfileManager.GetActiveProfile().InstallAll();
            RefreshCurrentModList();
        }

        private void OnModListChanged(object sender, EventArgs e) {
            searchBar.Input = "";
            RefreshCurrentModList();
        }

        private void OnSortOptionChanged(object sender, EventArgs e) {
            ProgramData.currentSortOption = StringUtils.GetModListSortOptionFromName(sortBox.SelectedItem);
            searchBar.Input = "";
            RefreshCurrentModList();
        }

        private void OnSearchBarTextChanged(object sender, EventArgs e) {
            if(modsList is InstalledModsPanel installedModsPanel) {
                installedModsPanel.SearchModsList(searchBar.Input);
            }
            else if (modsList is OnlineModsPanel onlineModsPanel) {
                onlineModsPanel.SearchModsList(searchBar.Input);
            }
            else if (modsList is NewModsPanel newModsPanel) {
                newModsPanel.SearchModsList(searchBar.Input);
            }
        }

        // Public Functions
        
        public void RefreshCurrentModList() {
            ModListSource source = StringUtils.GetModListSourceFromName(modListBox.SelectedItem);
            switch (source) {
                case ModListSource.Installed: modListBorder.Child = new InstalledModsPanel() { Margin = new Thickness(5) }; break;
                case ModListSource.Online: modListBorder.Child = new OnlineModsPanel() { Margin = new Thickness(5) }; break;
                case ModListSource.NewMods: modListBorder.Child = new NewModsPanel() { Margin = new Thickness(5) }; break;
                default:
                    Log.Error($"Could not load mod list for unknown source - {StringUtils.GetModListSourceName(source)}");
                    break;
            }

            loadingPanel.Visibility = Visibility.Hidden;
            mainGrid.Visibility = Visibility.Visible;
        }

        // Private Functions

        private void InitialiseAutoSaveTimer() {
            autoSaveTimer.Tick += OnAutoSaveTimerTick;
            autoSaveTimer.Start();
        }

        private void InitialiseProcessCheckTimer() {
            processCheckTimer.Tick += OnProcessCheckTimerTick;
            processCheckTimer.Start();
        }

        private void InitialiseGUI() {
            profileBox.SetItems(ProfileManager.GetAllProfileNames());

            modListBox.SetItems(StringUtils.GetAllModListSourceNamesForCombo());
            modListBox.SetItem(Settings.userSettings.defaultModList.value);

            sortBox.SetItems(StringUtils.GetAllModListSortOptionNamesForCombo());
            sortBox.SetItem(Settings.userSettings.defaultSortOption.value);
            ProgramData.currentSortOption = StringUtils.GetModListSortOptionFromName(Settings.userSettings.defaultSortOption.value);
        }

        private void CleanProfiles()
        {
            foreach (Profile profile in ProfileManager.GetAllProfiles()) {
                profile.RemoveObsoleteMods();
            }
        }

        // Data Functions

        private void SaveData() {
            if (ProgramData.safeToSave) {
                try {
                    Settings.Save();
                    Log.Debug("Settings saved");
                    ModManager.Save();
                    Log.Debug("ModManager saved");
                    ProfileManager.Save();
                    Log.Debug("ProfileManager saved");
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

        private async Task<string> LoadData() {
            Settings.Load();
            Log.Debug("Settings loaded");
            ModManager.Load();
            Log.Debug("ModManager loaded");
            await ProfileManager.Load();
            Log.Debug("ProfileManager loaded");
            ProgramData.safeToSave = true;
            return "";
        }
    }
}
