using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TechtonicaModLoader.Stores
{
    public interface IProgramData 
    {
        bool IsDebugBuild { get; }
        bool IsRuntime { get; }
        bool RunUnitTests { get; }
        string BepInExID { get; }
        Version ProgramVersion { get; }
        HashSet<string> ModsSeenThisSession { get; }
        HashSet<string> AllowedMods { get; }
        HashSet<string> DisallowedMods { get; }
        IFilePaths FilePaths { get; }

        event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class ProgramDataStore : ObservableObject, IProgramData 
    {
        // Properties

        public bool IsDebugBuild {
            get {
                #if DEBUG
                    return true;
                #else
                    return false;
                #endif
            }
        }
        public bool IsRuntime => Application.Current.MainWindow != null;
        public bool RunUnitTests { get; } = false;
        public string BepInExID { get; } = "b9a5a1bd-81d8-4913-a46e-70ca7734628c";
        public Version ProgramVersion => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(1, 0, 0);
        public IFilePaths FilePaths { get; } = new FilePathsStore();
        public HashSet<string> ModsSeenThisSession { get; set; } = new HashSet<string>();

        public HashSet<string> AllowedMods { get; } = new HashSet<string>() {
            "BepInExPack",
            "Helium",
            "LongStackInserters",
            "Official_BepInEx_ConfigurationManager",
            "UnityAudio",
            "UnityExplorer",
        };

        public HashSet<string> DisallowedMods { get; } = new HashSet<string>() {
            "r2modman",
            "GaleModManager",
        };
    }

    public static class ProgramData 
    {
        // Members

        private static IProgramData? programDataStore;

        // Properties

        private static IProgramData ProgramDataStore {
            get {
                if(programDataStore == null) {
                    Log.Error("programDataStore has not been initialised");
                    programDataStore = new ProgramDataStore();
                }

                return programDataStore;
            }
        }

        public static bool IsDebugBuild => ProgramDataStore.IsDebugBuild;
        public static bool IsRuntime => ProgramDataStore.IsRuntime;
        public static bool RunUnitTests => ProgramDataStore.RunUnitTests;
        
        public static string BepInExID => ProgramDataStore.BepInExID;
        public static Version ProgramVersion => ProgramDataStore.ProgramVersion;
        public static HashSet<string> ModsSeenThisSession => ProgramDataStore.ModsSeenThisSession;
        public static HashSet<string> AllowedMods => ProgramDataStore.AllowedMods;
        public static HashSet<string> DisallowedMods => ProgramDataStore.DisallowedMods;
        public static IFilePaths FilePaths => ProgramDataStore.FilePaths;

        // Events

        public static event PropertyChangedEventHandler? ProgramDataPropertyChanged;

        private static void OnProgramDataStorePropertyChanged(object? sender, PropertyChangedEventArgs e) {
            ProgramDataPropertyChanged?.Invoke(sender, e);
        }

        // Public Functions

        public static void Initialise(IProgramData programDataStore) {
            ProgramData.programDataStore = programDataStore;
            ProgramData.programDataStore.PropertyChanged += OnProgramDataStorePropertyChanged;
        }
    }
}
