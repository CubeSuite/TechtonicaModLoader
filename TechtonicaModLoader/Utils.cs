using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader
{
    public static class DebugUtils
    {
        public static void CrashIfDebug(string message) {
            if (ProgramData.isDebugBuild) throw new Exception(message);
        }

        public static void Assert(bool condition, string message) {
            if (!condition) {
                Log.Error($"Assert Failed: {message}");
                CrashIfDebug(message);
            }
        }
    }
}
