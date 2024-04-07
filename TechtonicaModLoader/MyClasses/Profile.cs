using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TechtonicaModLoader.Modes;

namespace TechtonicaModLoader.MyClasses
{
    public class Profile
    {
        // Objects & Variables
        public int id = -1;
        public string name;
        public Dictionary<string, bool> mods = new Dictionary<string, bool>();

        // Public Functions

        public void AddMod(string id) {
            if (!HasMod(id)) {
                mods.Add(id, true);
                Log.Debug($"Added mod with id '{id}' to profile '{name}'");
            }
            else {
                Log.Warning($"Skipped adding mod with id '{id}' to '{name}' profile");
            }
        }

        public bool HasMod(string id) {
            return mods.ContainsKey(id);
        }

        public void ToggleMod(string id) {
            if (HasMod(id)) {
                mods[id] = !mods[id];
                Log.Debug($"Set mod with id '{id}' to {IsModEnabled(id)} in profile '{name}'");
            }
            else {
                Log.Error($"Cannot toggle mod with id '{id}' - not in profile '{name}'");
            }
        }

        public bool IsModEnabled(string id) {
            if (HasMod(id)) {
                return mods[id];
            }
            else {
                Log.Error($"Could not get enabled state of mod with id '{id}' - not in profile '{name}'");
                return false;
            }
        }

        public void SortMods(ModListSortOption sortOption) {
            Log.Debug($"Sorting profile '{name}' with ModListSortOption '{StringUtils.GetModListSortOptionName(sortOption)}'");

            Dictionary<string, bool> tempEnabledStates = new Dictionary<string, bool>();
            foreach(KeyValuePair<string, bool> pair in mods) {
                tempEnabledStates.Add(pair.Key, pair.Value);
            }

            List<Mod> allMods = GetMods();
            allMods = ModManager.SortModList(allMods, sortOption);

            mods.Clear();

            if(name != "Vanilla") {
                mods.Add(ProgramData.bepInExID, true);
            }

            foreach (Mod mod in allMods) {
                if (mod.id != ProgramData.bepInExID) {
                    mods.Add(mod.id, tempEnabledStates[mod.id]);
                }
            }
        }

        public List<Mod> GetMods() {
            List<Mod> allMods = new List<Mod>();
            foreach(string id in mods.Keys) {
                Mod mod = ModManager.GetMod(id);
                if (mod != null) allMods.Add(mod);
            }

            return allMods;
        }

        public void InstallAll() {
            foreach(Mod mod in GetMods()) {
                if (IsModEnabled(mod)) {
                    mod.Install();
                }
            }
        }

        public void UninstallAll() {
            foreach (Mod mod in GetMods()) {
                if (IsModEnabled(mod)) {
                    mod.Uninstall();
                }
            }
        }

        #region Overloads

        // Public Functions

        public bool HasMod(Mod mod) {
            return HasMod(mod.id);
        }

        public void ToggleMod(Mod mod) {
            ToggleMod(mod.id);
        }

        public bool IsModEnabled(Mod mod) {
            return IsModEnabled(mod.id);
        }

        #endregion
    }
}
