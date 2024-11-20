using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Services.ThunderstoreModels;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Mod
{
    public class ModModel
    {
        // Members
        private ProfileManager _profileManager;

        private readonly string? _id;
        private readonly string? _name;
        private readonly string? _tagline;

        private readonly string? _link;
        private readonly string? _iconLink;

        private readonly bool? _isDownloaded;
        
        // Properties

        public string ID => _id ?? "";
        public string Name => _name ?? "Unknown Mod";
        public string TagLine => _tagline ?? "Description of mod unavailable";

        public string Link => _link ?? "https://thunderstore.io/c/techtonica/";
        public string IconLink => _iconLink ?? ""; // ToDo: default icon link

        public bool IsDownloaded => _isDownloaded ?? false;
        
        public bool IsEnabled => _profileManager.ActiveProfile.IsModEnabled(this);

        // Constructors

        public ModModel(ThunderStoreMod thunderStoreMod, ProfileManager profileManager) {
            //_profileManager = profileManager;
            _profileManager = profileManager.Instance;

            _id = thunderStoreMod.uuid4;
            _name = thunderStoreMod.name;
            _tagline = thunderStoreMod.versions[0].description;

            _link = thunderStoreMod.package_url;
            _iconLink = thunderStoreMod.versions[0].icon;
        }

        // Public Functions

        public void Download() {
            // ToDo: download mod
        }

        // Private Functions
    }

    public struct ModVersion
    {
        public int major;
        public int minor;
        public int build;

        public static ModVersion Parse(string input) {
            try {
                string[] parts = input.Split('.');
                return new ModVersion() {
                    major = int.Parse(parts[0]),
                    minor = int.Parse(parts[1]),
                    build = int.Parse(parts[2]),
                };
            }
            catch (Exception e) {
                string error = $"Error occurred while parsing Version '{input}': {e.Message}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return new ModVersion() {
                    major = 0,
                    minor = 0,
                    build = 0
                };
            }
        }

        public override string ToString() {
            return $"{major}.{minor}.{build}";
        }

        public static bool operator >(ModVersion v1, ModVersion v2) {
            if (v1.major > v2.major) return true;
            if (v1.major < v2.major) return false;

            if (v1.minor > v2.minor) return true;
            if (v1.minor < v2.minor) return false;

            return v1.build > v2.build;
        }

        public static bool operator <(ModVersion v1, ModVersion v2) {
            if (v1.major < v2.major) return true;
            if (v1.major > v2.major) return false;

            if (v1.minor < v2.minor) return true;
            if (v1.minor > v2.minor) return false;

            return v1.build < v2.build;
        }
    }
}
