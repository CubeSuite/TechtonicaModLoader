using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public class SettingViewModelHelper
    {
        // Members
        public const string defaultCategory = "General";
        
        // Properties
        public List<SettingBase> SettingViewModels { get; } = [];

        // Constructors

        public SettingViewModelHelper(IServiceProvider serviceProvider, Action onSettingChanged) {
            IUserSettings userSettings = serviceProvider.GetRequiredService<IUserSettings>();
            IDialogService dialogService = serviceProvider.GetRequiredService<IDialogService>();
            IProgramData programData = serviceProvider.GetRequiredService<IProgramData>();

            // General

            SettingViewModels.Add(new Setting<bool>(
                name: "Log Debug Messages",
                description: "Whether debug messages should be logged to file. Enable to gather info for a bug report.",
                category: defaultCategory,
                getValueFunc: () => userSettings.LogDebugMessages,
                setValueFunc: value => userSettings.LogDebugMessages = value
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: "Show Log In Explorer",
                description: "Opens the folder that contains Techtonica Mod Loader's log file.",
                category: defaultCategory,
                buttonText: "Show In Explorer",
                onClick: delegate () {
                    Process.Start(new ProcessStartInfo() {
                        FileName = programData.FilePaths.LogsFolder,
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
                onClick: delegate () {
                    // ToDo: Find Game Folder
                }
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: "Browse For Game Folder",
                description: "Manually browse for Techtonica's installation location.",
                category: "Game Folder",
                buttonText: "Browse",
                onClick: delegate () {
                    // ToDo: Move to dialogService
                    OpenFileDialog browser = new() { Filter = "Techtonica.exe|*.exe" };
                    if (browser.ShowDialog() == true) {
                        if (browser.FileName.EndsWith("Techtonica.exe")) {
                            userSettings.GameFolder = Path.GetDirectoryName(browser.FileName) ?? "";
                            onSettingChanged();
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
