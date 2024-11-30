using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.Models
{
    public struct ModVersion
    {
        public int _major;
        public int _minor;
        public int _patch;

        public ModVersion(int major, int minor, int patch) {
            _major = major;
            _minor = minor;
            _patch = patch;
        }

        public static ModVersion Parse(string input) {
            try {
                string[] parts = input.Split('.');
                return new ModVersion() {
                    _major = int.Parse(parts[0]),
                    _minor = int.Parse(parts[1]),
                    _patch = int.Parse(parts[2]),
                };
            }
            catch (Exception e) {
                string error = $"Error occurred while parsing Version '{input}': {e.Message}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return new ModVersion() {
                    _major = 0,
                    _minor = 0,
                    _patch = 0
                };
            }
        }

        public override string ToString() {
            return $"{_major}.{_minor}.{_patch}";
        }

        public static bool operator >(ModVersion v1, ModVersion v2) {
            if (v1._major > v2._major) return true;
            if (v1._major < v2._major) return false;

            if (v1._minor > v2._minor) return true;
            if (v1._minor < v2._minor) return false;

            return v1._patch > v2._patch;
        }

        public static bool operator <(ModVersion v1, ModVersion v2) {
            if (v1._major < v2._major) return true;
            if (v1._major > v2._major) return false;

            if (v1._minor < v2._minor) return true;
            if (v1._minor > v2._minor) return false;

            return v1._patch < v2._patch;
        }
    }
}
