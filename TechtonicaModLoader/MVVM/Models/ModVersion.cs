using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Models
{
    public class ModVersion 
    {
        // Properties

        public int Major { get; }
        public int Minor { get; }
        public int Patch { get; }

        // Constructors

        public ModVersion(int major, int minor, int patch) {
            Major = major;
            Minor = minor;
            Patch = patch;
        }

        public static ModVersion? Parse(string input, IServiceProvider serviceProvider) {
            ILoggerService logger = serviceProvider.GetRequiredService<ILoggerService>();
            IDebugUtils debugUtils = serviceProvider.GetRequiredService<IDebugUtils>();

            if (string.IsNullOrEmpty(input) || !input.Contains(".")) {
                string error = $"Can't parse null, empty or invalid string ('{input}') to {nameof(ModVersion)}";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);
                return null;
            }

            string[] parts = input.Split('.');
            if(parts.Count() != 3) {
                string error = $"Split string has invalid number of parts: {parts.Count()}/3";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);
                return null;
            }

            if (!int.TryParse(parts[0], out int major)) {
                string error = $"Couldn't parse major string into int: '{parts[0]}'";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);
                return null;
            }

            if (!int.TryParse(parts[1], out int minor)) {
                string error = $"Couldn't parse minor string into int: '{parts[1]}'";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);
                return null;
            }

            if (!int.TryParse(parts[2], out int patch)) {
                string error = $"Couldn't parse patch string into int: '{parts[2]}'";
                logger.Error(error);
                debugUtils.CrashIfDebug(error);
                return null;
            }

            return new ModVersion(major, minor, patch);
        }

        // Operators

        public static bool operator >(ModVersion v1, ModVersion v2) {
            if (v1.Major > v2.Major) return true;
            if (v1.Major < v2.Major) return false;

            if (v1.Minor > v2.Minor) return true;
            if (v1.Minor < v2.Minor) return false;

            return v1.Patch > v2.Patch;
        }

        public static bool operator <(ModVersion v1, ModVersion v2) {
            if (v1.Major < v2.Major) return true;
            if (v1.Major > v2.Major) return false;

            if (v1.Minor < v2.Minor) return true;
            if (v1.Minor > v2.Minor) return false;

            return v1.Patch < v2.Patch;
        }
    }
}
