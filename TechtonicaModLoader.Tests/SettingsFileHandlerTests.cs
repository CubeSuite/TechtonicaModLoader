using Moq;
using System.Drawing;
using System.IO.Abstractions;
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

        private ILoggerService? logger;
        private IProgramData? programData;
        private Mock<IProfileManager>? profileManager;
        private Mock<IFileSystem>? fileSystem;

        private Mock<IServiceProvider> SetupMockServiceProvider() {
            logger = Mock.Of<ILoggerService>();
            programData = Mock.Of<IProgramData>(p => p.FilePaths.SettingsFile == "SomePath");

            profileManager = new Mock<IProfileManager>(MockBehavior.Strict);
            profileManager.Setup(p => p.ProfilesList).Returns([
                new Profile(profileManager.Object, StringResources.ProfileModded, true),
                new Profile(profileManager.Object, StringResources.ProfileDevelopment, true),
                new Profile(profileManager.Object, StringResources.ProfileVanilla, true),
                ]);

            fileSystem = new Mock<IFileSystem>(MockBehavior.Strict);
            fileSystem.Setup(f => f.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>()));

            Mock<IServiceProvider> serviceProvider = new(MockBehavior.Strict);
            serviceProvider.Setup(s => s.GetService(typeof(ILoggerService))).Returns(logger);
            serviceProvider.Setup(s => s.GetService(typeof(IProgramData))).Returns(programData);
            serviceProvider.Setup(s => s.GetService(typeof(IProfileManager))).Returns(profileManager.Object);
            serviceProvider.Setup(s => s.GetService(typeof(IFileSystem))).Returns(fileSystem.Object);

            return serviceProvider;
        }

        [TestMethod]
        public void Load_FileNotExist_ReturnsDefault() {
            Mock<IServiceProvider> serviceProvider = SetupMockServiceProvider();
            Assert.IsNotNull(fileSystem);
            fileSystem.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(false);

            SettingsFileHandler settingsFileHandler = new(serviceProvider.Object);
            SettingsData? settingsData = settingsFileHandler.Load();

            Assert.IsNotNull(settingsData);
            Assert.AreEqual(settingsData, new SettingsData());
            fileSystem.Verify(f => f.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void Load_FileEmpty_ReturnsDefault() {
            Mock<IServiceProvider> serviceProvider = SetupMockServiceProvider();
            Assert.IsNotNull(fileSystem);
            fileSystem.Setup(f => f.File.Exists(It.IsAny<string>())).Returns(true);
            fileSystem.Setup(f => f.File.ReadAllText(It.IsAny<string>())).Returns(string.Empty);

            SettingsFileHandler settingsFileHandler = new(serviceProvider.Object);
            SettingsData? settingsData = settingsFileHandler.Load();

            Assert.IsNotNull(settingsData);
            Assert.AreEqual(settingsData, new SettingsData());
            fileSystem.Verify(f => f.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ParseSettingsJson_V1Schema_DoesNotThrow() {
            Mock<IServiceProvider> serviceProvider = SetupMockServiceProvider();

            SettingsFileHandler settingsFileHandler = new(serviceProvider.Object);
            SettingsData? settingsData = settingsFileHandler.ParseSettingsJson(ref v1Schema);

            Assert.IsNotNull(settingsData);
            Assert.IsTrue(settingsData == convertedV1Schema);
            Assert.IsNotNull(fileSystem);
            fileSystem.Verify(f => f.File.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void ParseSettingsJson_BadJson_LogErrorReturnDefault() {
            Mock<IServiceProvider> serviceProvider = SetupMockServiceProvider();

            SettingsFileHandler settingsFileHandler = new(serviceProvider.Object);
            string badJson = "{";
            SettingsData? settingsData = settingsFileHandler.ParseSettingsJson(ref badJson);

            Assert.IsNull(settingsData);
        }
    }
}
