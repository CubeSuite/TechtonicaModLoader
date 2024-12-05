using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    internal class SettingViewModelHelper 
    {
        // Properties
        public List<SettingBase> SettingViewModels { get; } = [];

        public string DefaultCategory { get; }

        internal SettingViewModelHelper(UserSettings userSettings, IDialogService dialogService, Action settingChanged, string defaultCategory) {

            DefaultCategory = defaultCategory;

            // General

            SettingViewModels.Add(new Setting<bool>(
                name: "Log Debug Messages",
                description: "Whether debug messages should be logged to file. Enable to gather info for a bug report.",
                category: DefaultCategory,
                getValueFunc: () => userSettings.LogDebugMessages,
                setValueFunc: value => userSettings.LogDebugMessages = value
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: "Show Log In Explorer",
                description: "Opens the folder that contains Techtonica Mod Loader's log file.",
                category: DefaultCategory,
                buttonText: "Show In Explorer",
                userSettings: userSettings,
                onClick: delegate (UserSettings settings) {
                    Process.Start(new ProcessStartInfo() {
                        FileName = ProgramData.FilePaths.LogsFolder,
                        UseShellExecute = true,
                        Verb = "open"
                    });
                }
            ));

            // Game Folder

            SettingViewModels.Add(new Setting<string>(
                name: "Game Folder",
                description: "Techtonica's Installation Location.",
                category: "Game Folder",
                getValueFunc: () => userSettings.GameFolder,
                setValueFunc: value => userSettings.GameFolder = value
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: "Find Game Folder",
                description: "Have Techtonica Mod Loader search for your Techtonica installation folder.",
                category: "Game Folder",
                buttonText: "Find",
                userSettings: userSettings,
                onClick: delegate (UserSettings settings) {
                    // ToDo: Find Game Folder
                }
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: "Browse For Game Folder",
                description: "Manually browse for Techtonica's installation location.",
                category: "Game Folder",
                buttonText: "Browse",
                userSettings: userSettings,
                onClick: delegate (UserSettings settings) {

                    //TODO: ***** This needs to be refactored to remove OpenFileDialog from the ViewModel ******
                    //   How?  IDialogService seems like an interesting place to add a method for this.   May want to think about making any new method resuable for other scenarios.
                    //   I set up the code here for refactoring, regardless of how its done.  

                    OpenFileDialog browser = new() { Filter = "Techtonica.exe|*.exe" };
                    if (browser.ShowDialog() == true) {
                        if (browser.FileName.EndsWith("Techtonica.exe")) {
                            userSettings.GameFolder = Path.GetDirectoryName(browser.FileName) ?? "";
                            settingChanged();
                        }
                        else {
                            dialogService.ShowErrorMessage("Wrong File Selected", "You need to select the file 'Techtonica.exe'");
                        }
                    }
                }
            ));

            // Mod List

            SettingViewModels.Add(new EnumSetting<ModListSource>(
                name: "Default Mod List",
                description: "The mod list that is displayed when you open Techtonica Mod Loader.",
                category: "Mod List",
                getValueFunc: () => userSettings.DefaultModList,
                setValueFunc: value => userSettings.DefaultModList = value
            ));

            SettingViewModels.Add(new EnumSetting<ModListSortOption>(
                name: "Default Sort Option",
                description: "The default sort option to apply to the mod list.",
                category: "Mod List",
                getValueFunc: () => userSettings.DefaultModListSortOption,
                setValueFunc: value => userSettings.DefaultModListSortOption = value
            ));
        }
    }
}
