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

    public partial class ProgramData : ObservableObject, IProgramData 
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
        public IFilePaths FilePaths { get; }
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

        // Constructors

        public ProgramData()
        {
            FilePaths = new FilePathsStore(this);
        }
    }
}
