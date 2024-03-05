using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Modes;

namespace TechtonicaModLoader
{
    public static class ModManager
    {
        // Objects & Variables

        private static Dictionary<int, Mod> mods = new Dictionary<int, Mod>();

        // Public Functions

        public static void addMod(Mod mod) {
            mod.id = getNewID();
            mods.Add(mod.id, mod);
        }

        public static int getModesCount() {
            return mods.Count;
        }

        // Private Functions

        private static int getNewID() {
            if (getModesCount() == 0) return 0;
            else return mods.Keys.Max() + 1;
        }

        // Data Functions

        public static void saveData() {
            string json = JsonConvert.SerializeObject(mods.Values.ToList());
            File.WriteAllText(ProgramData.FilePaths.modSaveFile, json);
        }

        public static void loadData() {
            if (File.Exists(ProgramData.FilePaths.modSaveFile)) {
                string json = File.ReadAllText(ProgramData.FilePaths.modSaveFile);
                List<Mod> modes = JsonConvert.DeserializeObject<List<Mod>>(json);
                foreach (Mod mod in modes) {
                    addMod(mod);
                }
            }
        }
    }
}
