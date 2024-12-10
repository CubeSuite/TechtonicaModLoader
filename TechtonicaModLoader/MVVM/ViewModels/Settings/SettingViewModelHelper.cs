using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public class SettingViewModelHelper
    {
        // Properties
        public List<SettingBase> SettingViewModels { get; } = [];

        // Constructors

        public SettingViewModelHelper(IServiceProvider serviceProvider, Action onSettingChanged) {
            IUserSettings userSettings = serviceProvider.GetRequiredService<IUserSettings>();
            IDialogService dialogService = serviceProvider.GetRequiredService<IDialogService>();
            IProgramData programData = serviceProvider.GetRequiredService<IProgramData>();

            // General

            SettingViewModels.Add(new Setting<bool>(
                name: StringResources.LogDebugMessagesName,
                description: StringResources.LogDebugMessagesDescription,
                category: StringResources.CategoryGeneral,
                getValueFunc: () => userSettings.LogDebugMessages,
                setValueFunc: value => userSettings.LogDebugMessages = value
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: StringResources.ShowLogInExplorerButtonName,
                description: StringResources.ShowLogInExplorerButtonDescription,
                category: StringResources.CategoryGeneral,
                buttonText: StringResources.ShowLogInExplorerButtonText,
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
                name: StringResources.GameFolderName,
                description: StringResources.GameFolderDescription,
                category: StringResources.CategoryGameFolder,
                getValueFunc: () => userSettings.GameFolder,
                setValueFunc: value => userSettings.GameFolder = value
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: StringResources.FindGameFolderButtonName,
                description: StringResources.FindGameFolderButtonDescription,
                category: StringResources.CategoryGameFolder,
                buttonText: StringResources.FindGameFolderButtonText,
                onClick: delegate () {
                    // ToDo: Find Game Folder
                }
            ));

            SettingViewModels.Add(new ButtonSetting(
                name: StringResources.BrowseForGameFolderButtonName,
                description: StringResources.BrowseForGameFolderButtonDescription,
                category: StringResources.CategoryGameFolder,
                buttonText: StringResources.BrowseForGameFolderButtonText,
                onClick: delegate () {
                    // ToDo: Move to dialogService
                    OpenFileDialog browser = new() { Filter = "Techtonica.exe|*.exe" };
                    if (browser.ShowDialog() == true) {
                        if (browser.FileName.EndsWith("Techtonica.exe")) {
                            userSettings.GameFolder = Path.GetDirectoryName(browser.FileName) ?? "";
                            onSettingChanged();
                        }
                        else {
                            dialogService.ShowErrorMessage(StringResources.WrongFileTitle, StringResources.WrongFileMessage);
                        }
                    }
                }
            ));

            // Mod List

            SettingViewModels.Add(new EnumSetting<ModListSource>(
                name: StringResources.DefaultModListName,
                description: StringResources.DefaultModListDescription,
                category: StringResources.CategoryModList,
                options: ModListSourceDisplay.Strings,
                getValueFunc: () => userSettings.DefaultModList,
                setValueFunc: value => userSettings.DefaultModList = value
            ));

            SettingViewModels.Add(new EnumSetting<ModListSortOption>(
                name: StringResources.DefaultSortOptionName,
                description: StringResources.DefaultSortOptionDescription,
                category: StringResources.CategoryModList,
                options: ModListSortOptionDisplay.Strings,
                getValueFunc: () => userSettings.DefaultModListSortOption,
                setValueFunc: value => userSettings.DefaultModListSortOption = value
            ));
        }
    }
}
