using Moq;
using System.Drawing;
using TechtonicaModLoader.MVVM.Models;
using TechtonicaModLoader.Resources;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Stores.Settings;

namespace TechtonicaModLoader.Tests
{
    [TestClass]
    public sealed class SettingsFileHandlerTests
    {
        private string v1Schema = @"{
              ""logDebugMessages"": {
                ""value"": true
              },
              ""showLog"": {},
              ""gameFolder"": {
                ""value"": ""D:\\steam\\steamapps\\common\\Techtonica""
              },
              ""findGameFolder"": {},
              ""browseForGameFolder"": {},
              ""defaultSortOption"": {
                ""value"": ""Last Updated""
              },
              ""defaultModList"": {
                ""value"": ""New Mods""
              },
              ""backupsFolder"": {
                ""value"": ""C:\\Users\\james\\Documents\\Techtonica Mod Loader Backups""
              },
              ""browseForBackupsFolder"": {},
              ""numBackups"": {
                ""value"": 5
              },
              ""openBackups"": {},
              ""restoreDefaultTheme"": {},
              ""dimBackground"": {
                ""value"": ""#FF161626""
              },
              ""normalBackground"": {
                ""value"": ""#FF303040""
              },
              ""brightBackground"": {
                ""value"": ""#FF3A3A58""
              },
              ""uiBackground"": {
                ""value"": ""#FF444462""
              },
              ""accentColour"": {
                ""value"": ""#FFFFA500""
              },
              ""textColour"": {
                ""value"": ""#FFFFFFFF""
              },
              ""isFirstTimeLaunch"": {
                ""value"": false
              },
              ""lastProfile"": {
                ""value"": ""Modded""
              },
              ""seenMods"": {
                ""value"": ""42f76853-d2a4-4520-949b-13a02fdbbbcb|3ab8e328-cff6-498e-8e85-c226e8ba94c5|ea219b83-70f4-494d-adcc-650c102525f9""
              }
            }";

        private readonly SettingsData convertedV1Schema = new() {
            LogDebugMessages = true,
            GameFolder = "D:\\steam\\steamapps\\common\\Techtonica",
            DefaultModListSortOption = ModListSortOption.LastUpdated,
            DefaultModList = ModListSource.New,
            BackupsFolder = "C:\\Users\\james\\Documents\\Techtonica Mod Loader Backups",
            NumBackups = 5,
            DimBackground = ColorTranslator.FromHtml("#FF161626"),
            NormalBackground = ColorTranslator.FromHtml("#FF303040"),
            BrightBackground = ColorTranslator.FromHtml("#FF3A3A58"),
            UiBackground = ColorTranslator.FromHtml("#FF444462"),
            AccentColour = ColorTranslator.FromHtml("#FFFFA500"),
            TextColour = ColorTranslator.FromHtml("#FFFFFFFF"),
            IsFirstTimeLaunch = false,
            ActiveProfileID = 0,
            SeenMods = ["42f76853-d2a4-4520-949b-13a02fdbbbcb", "3ab8e328-cff6-498e-8e85-c226e8ba94c5", "ea219b83-70f4-494d-adcc-650c102525f9"],
        };

        private (Mock<ILoggerService>, Mock<IServiceProvider>) SetupV1Schema() {
            Mock<ILoggerService> logger = new(MockBehavior.Strict);

            IProgramData programData = Mock.Of<IProgramData>(p => p.FilePaths.SettingsFile == "SomePath");

            Mock<IProfileManager> profileManager = new(MockBehavior.Strict);
            profileManager.Setup(p => p.ProfilesList).Returns([
                new Profile(profileManager.Object, StringResources.ProfileModded, true),
                new Profile(profileManager.Object, StringResources.ProfileDevelopment, true),
                new Profile(profileManager.Object, StringResources.ProfileVanilla, true),
                ]);

            Mock<IServiceProvider> serviceProvider = new(MockBehavior.Strict);
            serviceProvider.Setup(s => s.GetService(typeof(ILoggerService))).Returns(logger.Object);
            serviceProvider.Setup(s => s.GetService(typeof(IProgramData))).Returns(programData);
            serviceProvider.Setup(s => s.GetService(typeof(IProfileManager))).Returns(profileManager.Object);

            return (logger, serviceProvider);
        }

        [TestMethod]
        public void ParseSettingsJson_EmptyJson_LogErrorReturnDefault() {
            (Mock<ILoggerService> logger, Mock<IServiceProvider> serviceProvider) = SetupV1Schema();
            logger.Setup(l => l.Error(It.IsAny<string>()));

            SettingsFileHandler settingsFileHandler = new(serviceProvider.Object);
            string empty = string.Empty;
            SettingsData? settingsData = settingsFileHandler.ParseSettingsJson(ref empty);

            Assert.IsNull(settingsData);
        }

        [TestMethod]
        public void ParseSettingsJson_V1Schema_DoesNotThrow() {
            (Mock<ILoggerService> logger, Mock<IServiceProvider> serviceProvider) = SetupV1Schema();
            logger.Setup(l => l.Error(It.IsAny<string>()));
            logger.Setup(l => l.Warning(It.IsAny<string>()));
            logger.Setup(l => l.Info(It.IsAny<string>(), true));

            SettingsFileHandler settingsFileHandler = new(serviceProvider.Object);
            SettingsData? settingsData = settingsFileHandler.ParseSettingsJson(ref v1Schema);

            Assert.IsNotNull(settingsData);
            Assert.IsTrue(settingsData == convertedV1Schema);
        }

        [TestMethod]
        public void ParseSettingsJson_BadJson_LogErrorReturnDefault() {
            (Mock<ILoggerService> logger, Mock<IServiceProvider> serviceProvider) = SetupV1Schema();
            logger.Setup(l => l.Error(It.IsAny<string>()));
            logger.Setup(l => l.Debug(It.IsAny<string>(), true));

            SettingsFileHandler settingsFileHandler = new(serviceProvider.Object);
            string badJson = "{";

            SettingsData? settingsData = settingsFileHandler.ParseSettingsJson(ref badJson);
            Assert.IsNull(settingsData);
        }
    }
}
