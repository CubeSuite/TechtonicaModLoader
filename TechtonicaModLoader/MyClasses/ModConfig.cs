using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MyClasses
{
    public class ModConfig
    {
        // Objects & Variables
        public static ModConfig activeConfig;

        public string filePath;
        public List<ConfigOption> options = new List<ConfigOption>();

        // Public Functions

        public List<string> GetCategories() {
            List<string> categories = new List<string>();
            foreach (ConfigOption option in options) {
                if (!categories.Contains(option.category)) {
                    categories.Add(option.category);
                }
            }

            categories.Sort();
            Log.Debug($"Mod config categories: {string.Join(", ", categories)}");
            return categories;
        }

        public List<ConfigOption> GetOptionsInCategory(string category) {
            Log.Debug($"Getting mod config options in category '{category}'");
            return options.Where(option => option.category == category).ToList();
        }

        public void UpdateSetting(string name, string value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    if(option is StringConfigOption stringOption) {
                        stringOption.value = value;
                        SaveToFile();
                        Log.Debug($"Set mod config StringSetting '{name}' to '{value}'");
                    }
                    else if (option is KeyboardShortcutConfigOption shortcutOption) {
                        shortcutOption.value = value;
                        SaveToFile();
                        Log.Debug($"Set mod config KeyboardShortcutSetting '{name}' to '{value}'");
                    }
                    else if (option is KeyCodeConfigOption keycodeOption) {
                        keycodeOption.value = value;
                        SaveToFile();
                        Log.Debug($"Set mod config KeyCodeSetting '{name}' to '{value}'");
                    }
                    else {
                        Log.Error($"Cannot Update mod config entry '{name}' - unknown type '{option.optionType}'");
                    }
                    return;
                }
            }

            Log.Error($"Could not find mod config StringSetting with name '{name}'");
        }

        public void UpdateSetting(string name, int value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    IntConfigOption intOption = option as IntConfigOption;
                    intOption.value = value;
                    SaveToFile();
                    Log.Debug($"Set mod config IntSetting '{name}' to '{value}'");
                    return;
                }
            }

            Log.Error($"Could not find mod config IntSetting with name '{name}'");
        }

        public void UpdateSetting(string name, float value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    FloatConfigOption floatOption = option as FloatConfigOption;
                    floatOption.value = value;
                    SaveToFile();
                    Log.Debug($"Set mod config FloatSetting '{name}' to '{value}'");
                    return;
                }
            }

            Log.Error($"Could not find mod config FloatSetting with name '{name}'");
        }

        public void UpdateSetting(string name, double value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    DoubleConfigOption doubleOption = option as DoubleConfigOption;
                    doubleOption.value = value;
                    SaveToFile();
                    Log.Debug($"Set mod config DoubleSetting '{name}' to '{value}'");
                    return;
                }
            }

            Log.Error($"Could not find mod config DoubleSetting with name '{name}'");
        }

        public void UpdateSetting(string name, bool value) {
            foreach (ConfigOption option in options) {
                if (option.name == name) {
                    BooleanConfigOption boolOption = option as BooleanConfigOption;
                    boolOption.value = value;
                    SaveToFile();
                    Log.Debug($"Set mod config BoolSetting '{name}' to '{value}'");
                    return;
                }
            }

            Log.Error($"Could not find mod config BoolSetting with name '{name}'");
        }

        public void UpdateSetting(ConfigOption setting) {
            foreach(ConfigOption option in options) {
                if (option.name != setting.name) continue;

                if (setting is StringConfigOption stringOption) UpdateSetting(setting.name, stringOption.value);
                else if (setting is KeyboardShortcutConfigOption shortcutOption) UpdateSetting(setting.name, shortcutOption.value);
                else if (setting is KeyCodeConfigOption keycodeOption) UpdateSetting(setting.name, keycodeOption.value);
                else if (setting is IntConfigOption intOption) UpdateSetting(setting.name, intOption.value);
                else if (setting is FloatConfigOption floatOption) UpdateSetting(setting.name, floatOption.value);
                else if (setting is DoubleConfigOption doubleOption) UpdateSetting(setting.name, doubleOption.value);
                else if (setting is BooleanConfigOption boolOption) UpdateSetting(setting.name, boolOption.value);
                else Log.Error($"Cannot update mod config option '{setting.name}' - unknown type: '{setting.optionType}'");
            }
        }

        public bool HasSetting(string name) {
            return options.Select(option => option.name).Contains(name);
        }

        public void RestoreDefaults() {
            Log.Debug($"Restoring mod config defaults");
            foreach(ConfigOption option in options) {
                option.RestoreDefault();
            }

            SaveToFile();
        }

        // Save And Load Functions

        public static ModConfig FromFile(string filename) {
            Log.Debug($"Reading config file from '{filename}'");
            ModConfig config = new ModConfig() { filePath = filename };
            string[] lines = File.ReadAllLines(filename);

            bool startedConfigFile = false;
            string latestCategory = "";
            for (int i = 0; i < lines.Length; i++) {
                string thisLine = lines[i];
                if (thisLine.StartsWith("[")) {
                    startedConfigFile = true;
                    latestCategory = thisLine.Replace("[", "").Replace("]", "");
                    Log.Debug($"Found category '{latestCategory}'");
                }

                if (thisLine.StartsWith("#") && startedConfigFile) {
                    int offset = thisLine.StartsWith("##") ? 1 : 0;
                    string optionType = lines[i + offset].Split(new string[] { ": " }, StringSplitOptions.None).Last();
                    List<string> optionLines = new List<string>() { };
                    for (int j = i; j < lines.Count(); j++) {
                        if (string.IsNullOrEmpty(lines[j]) || string.IsNullOrWhiteSpace(lines[j]) || j == lines.Count() - 1) {
                            for (int k = i; k < j; k++) {
                                optionLines.Add(lines[k]);
                                i = k;
                            }

                            break;
                        }
                    }

                    switch (optionType) {
                        case "String": config.options.Add(new StringConfigOption(optionLines) { category = latestCategory }); break;
                        case ConfigOptionTypes.keyboardShortcutOption: config.options.Add(new KeyboardShortcutConfigOption(optionLines) { category = latestCategory }); break;
                        case ConfigOptionTypes.keycodeOption: config.options.Add(new KeyCodeConfigOption(optionLines) { category = latestCategory }); break;
                        case "Int32": config.options.Add(new IntConfigOption(optionLines) { category = latestCategory }); break;
                        case "Single": config.options.Add(new FloatConfigOption(optionLines) { category = latestCategory }); break;
                        case "Double": config.options.Add(new DoubleConfigOption(optionLines) { category = latestCategory }); break;
                        case "Boolean": config.options.Add(new BooleanConfigOption(optionLines) { category = latestCategory }); break;
                        default:
                            Log.Error($"Cannot add config option for unknown type: '{optionType}'");
                            break;
                    }
                }
            }

            return config;
        }

        public void SaveToFile() {
            List<string> fileLines = new List<string>() {
                $"## Settings file was created by Techtonica Mod Loader V{ProgramData.versionText}",
                ""
            };

            foreach (string category in GetCategories()) {
                fileLines.Add($"[{category}]");
                foreach (ConfigOption option in GetOptionsInCategory(category)) {
                    fileLines.Add("");
                    if (!string.IsNullOrEmpty(option.description)) {
                        fileLines.Add($"## {option.description}");
                    }

                    fileLines.AddRange(option.ToLines());
                }

                fileLines.Add("");
            }

            File.WriteAllLines(filePath, fileLines);
        }
    }

    public class ConfigOption
    {
        public string name;
        public string description;
        public string category;
        public string optionType;

        public string GetDescription() {
            if (!string.IsNullOrEmpty(description)) return description;
            else return "No description available.";
        }

        public virtual void RestoreDefault(){}
       
        public virtual List<string> ToLines() {
            string error = "ConfigOption.ToLine() has not been overridden";
            Log.Error(error);
            DebugUtils.CrashIfDebug(error);
            return new List<string>();
        }
    }

    public static class ConfigOptionTypes 
    {
        public const string stringOption = "String";
        public const string keyboardShortcutOption = "KeyboardShortcut";
        public const string keycodeOption = "KeyCode";
        public const string intOption = "Int32";
        public const string floatOption = "Single";
        public const string doubleOption = "Double";
        public const string boolOption = "Boolean";
    }

    public class StringConfigOption : ConfigOption
    {
        public string value;
        public string defaultValue;

        public StringConfigOption() { optionType = ConfigOptionTypes.stringOption; }
        public StringConfigOption(List<string> fileLines) {
            optionType = "String";
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = line.Split(new string[] { ": " }, StringSplitOptions.None).Last();
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = line.Split(new string[] { " = " }, StringSplitOptions.None).Last();
                }
            }
        }

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public override List<string> ToLines() {
            return new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
                $"{name} = {value}"
            };
        }
    }

    public class KeyboardShortcutConfigOption : ConfigOption {
        public string value;
        public string defaultValue;

        public KeyboardShortcutConfigOption() { optionType = ConfigOptionTypes.keyboardShortcutOption; }
        public KeyboardShortcutConfigOption(List<string> fileLines) {
            optionType = ConfigOptionTypes.keyboardShortcutOption;
            foreach(string line in fileLines) {
                if(line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = line.Split(new string[] { ": " }, StringSplitOptions.None).Last();
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = line.Split(new string[] { " = " }, StringSplitOptions.None).Last();
                }
            }
        }

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public override List<string> ToLines() {
            return new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
                $"{name} = {value}",
            };
        }
    }

    public class KeyCodeConfigOption : ConfigOption
    {
        public string value;
        public string defaultValue;

        public KeyCodeConfigOption() { optionType = ConfigOptionTypes.keycodeOption; }
        public KeyCodeConfigOption(List<string> fileLines) {
            optionType = ConfigOptionTypes.keycodeOption;
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = line.Split(new string[] { ": " }, StringSplitOptions.None).Last();
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = line.Split(new string[] { " = " }, StringSplitOptions.None).Last();
                }
            }
        }

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public override List<string> ToLines() {
            return new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
                $"{name} = {value}"
            };
        }
    }

    public class IntConfigOption : ConfigOption
    {
        public int value;
        public int defaultValue;
        public int min = int.MinValue;
        public int max = int.MaxValue;

        public IntConfigOption(List<string> fileLines) {
            optionType = ConfigOptionTypes.intOption;
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = int.Parse(line.Split(new string[] { ": " }, StringSplitOptions.None).Last());
                }
                else if (line.StartsWith("# Accept")) {
                    ExtractMinMaxValues(line, out min, out max);
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = int.Parse(line.Split(new string[] { " = " }, StringSplitOptions.None).Last());
                }
            }
        }

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public override List<string> ToLines() {
            List<string> lines = new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
            };

            if (min != int.MinValue && max != int.MaxValue) {
                lines.Add($"# Acceptable value range: From {min} to {max}");
            }

            lines.Add($"{name} = {value}");
            return lines;
        }

        bool ExtractMinMaxValues(string input, out int minValue, out int maxValue) {
            string pattern = @"From (-?\d+) to (-?\d+)";
            Match match = Regex.Match(input, pattern);

            if (match.Success) {
                if (int.TryParse(match.Groups[1].Value, out int min) && int.TryParse(match.Groups[2].Value, out int max)) {
                    if (min <= max) {
                        minValue = min;
                        maxValue = max;
                        return true;
                    }
                }
            }

            minValue = int.MinValue;
            maxValue = int.MaxValue;
            return false;
        }
    }

    public class FloatConfigOption : ConfigOption
    {
        public float value;
        public float defaultValue;
        public float min = float.MinValue;
        public float max = float.MaxValue;

        public FloatConfigOption(List<string> fileLines) {
            optionType = ConfigOptionTypes.floatOption;
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = float.Parse(line.Split(new string[] { ": " }, StringSplitOptions.None).Last());
                }
                else if (line.StartsWith("# Accept")) {
                    ExtractMinMaxValues(line, out min, out max);
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = float.Parse(line.Split(new string[] { " = " }, StringSplitOptions.None).Last());
                }
            }
        }

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public override List<string> ToLines() {
            List<string> lines = new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
            };

            if (min != float.MinValue && max != float.MaxValue) {
                lines.Add($"# Acceptable value range: From {min} to {max}");
            }

            lines.Add($"{name} = {value}");
            return lines;
        }

        bool ExtractMinMaxValues(string input, out float minValue, out float maxValue) {
            string pattern = @"From (-?\d+(\.\d+)?) to (-?\d+(\.\d+)?)";
            Match match = Regex.Match(input, pattern);

            if (match.Success) {
                if (float.TryParse(match.Groups[1].Value, out float min) && float.TryParse(match.Groups[2].Value, out float max)) {
                    if (min <= max) {
                        minValue = min;
                        maxValue = max;
                        return true;
                    }
                }
            }

            minValue = float.MinValue;
            maxValue = float.MaxValue;
            return false;
        }
    }

    public class DoubleConfigOption : ConfigOption
    {
        public double value;
        public double defaultValue;
        public double min = double.MinValue;
        public double max = double.MaxValue;

        public DoubleConfigOption(List<string> fileLines) {
            optionType = ConfigOptionTypes.doubleOption;
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = double.Parse(line.Split(new string[] { ": " }, StringSplitOptions.None).Last());
                }
                else if (line.StartsWith("# Accept")) {
                    ExtractMinMaxValues(line, out min, out max);
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = double.Parse(line.Split(new string[] { " = " }, StringSplitOptions.None).Last());
                }
            }
        }

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public override List<string> ToLines() {
            List<string> lines = new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue}",
            };

            if (min != double.MinValue && max != double.MaxValue) {
                lines.Add($"# Acceptable value range: From {min} to {max}");
            }

            lines.Add($"{name} = {value}");
            return lines;
        }

        bool ExtractMinMaxValues(string input, out double minValue, out double maxValue) {
            string pattern = @"From (-?\d+(\.\d+)?) to (-?\d+(\.\d+)?)";
            Match match = Regex.Match(input, pattern);

            if (match.Success) {
                if (double.TryParse(match.Groups[1].Value, out double min) && double.TryParse(match.Groups[2].Value, out double max)) {
                    if (min <= max) {
                        minValue = min;
                        maxValue = max;
                        return true;
                    }
                }
            }

            minValue = double.MinValue;
            maxValue = double.MaxValue;
            return false;
        }
    }

    public class BooleanConfigOption : ConfigOption
    {
        public bool value;
        public bool defaultValue;

        public BooleanConfigOption() { optionType = ConfigOptionTypes.boolOption; }
        public BooleanConfigOption(List<string> fileLines) {
            optionType = ConfigOptionTypes.boolOption;
            foreach (string line in fileLines) {
                if (line.StartsWith("## ")) {
                    description = line.Replace("## ", "");
                }
                else if (line.StartsWith("# Default")) {
                    defaultValue = line.Split(new string[] { ": " }, StringSplitOptions.None).Last() == "true";
                }
                else if (!line.StartsWith("# Setting type")) {
                    name = line.Split(new string[] { " = " }, StringSplitOptions.None).First();
                    value = line.Split(new string[] { " = " }, StringSplitOptions.None).Last() == "true";
                }
            }
        }

        public override void RestoreDefault() {
            value = defaultValue;
        }

        public override List<string> ToLines() {
            return new List<string>() {
                $"# Setting type: {optionType}",
                $"# Default value: {defaultValue.ToString().ToLower()}",
                $"{name} = {value.ToString().ToLower()}"
            };
        }
    }
}
