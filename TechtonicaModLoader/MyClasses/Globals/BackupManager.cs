using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Modes.Globals;
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader
{
    public static class BackupManager
    {
        // Objects & Variables
        public static string defaultBackupsFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{ProgramData.programName} Backups";

        // Public Functions

        public static void CreateBackup() {
            if (!ValidateBackupFolder()) return;

            string name = GetBackupName();
            string backupFolder = $"{Settings.userSettings.backupsFolder.value}\\{name}";
            Directory.CreateDirectory(backupFolder);
            CopyFolder(ProgramData.FilePaths.dataFolder, $"{backupFolder}\\Data");
        }

        public static List<string> GetAllBackups() {
            List<string> backups = new List<string>();
            if (BackupFolderExists()) {
                string[] backupFolders = Directory.GetDirectories(Settings.userSettings.backupsFolder.value);
                foreach (string backupFolder in backupFolders) {
                    backups.Add(backupFolder.Split('\\').Last());
                }
            }

            return backups;
        }

        public static async Task<string> AutoBackup() {
            if (!ValidateBackupFolder()) return "";

            await Task.Run(() => {
                List<string> backups = GetAllBackups();
                if (GetNumBackups() >= Settings.userSettings.numBackups.value) {
                    DateTime minDateTime = DateTime.Now;
                    foreach (string backup in backups) {
                        DateTime backupDateTime = GetBackupDateTime(backup);
                        if (backupDateTime < minDateTime) {
                            minDateTime = backupDateTime;
                        }
                    }

                    DeleteBackup($"{Settings.userSettings.backupsFolder.value}\\{GetBackupName(minDateTime)}");
                }

                CreateBackup();
            });

            return "";
        }

        public static void DeleteBackup(string name) {
            DeleteFolder(name);
        }

        public static void RestoreBackup(string name) {
            ProgramData.safeToSave = false;
            DeleteFolder(ProgramData.FilePaths.dataFolder);

            string backupFolder = $"{Settings.userSettings.backupsFolder.value}\\{name}";
            CopyFolder($"{backupFolder}\\Data", ProgramData.FilePaths.dataFolder);
            ProgramData.safeToSave = true;
        }

        // Private Functions

        private static bool ValidateBackupFolder() {
            if (string.IsNullOrEmpty(Settings.userSettings.backupsFolder.value)) {
                Settings.userSettings.backupsFolder.value = defaultBackupsFolder;
            }

            try {
                Directory.CreateDirectory(Settings.userSettings.backupsFolder.value);
                return true;
            }
            catch {
                string error = $"Can't create backup, folder doesn't exist: '{Settings.userSettings.backupsFolder.value}'";
                Log.Error(error);
                DebugTools.DebugCrash(error);
                GuiUtils.ShowErrorMessage("Couldn't Create Backup", $"{ProgramData.programName} couldn't back up it's data as the selected folder path doesn't exist.\n" +
                                                                     "Please change the folder path or create the one you've entered");
                return false;
            }
        }

        private static bool BackupFolderExists() {
            return Directory.Exists(Settings.userSettings.backupsFolder.value);
        }

        private static string GetBackupName() {
            return GetBackupName(DateTime.Now);
        }

        private static string GetBackupName(DateTime dateTime) {
            return dateTime.ToString().Replace("/", ",").Replace(":", "-");
        }

        private static DateTime GetBackupDateTime(string name) {
            name = name.Replace(",", "/").Replace("-", ":");
            return DateTime.Parse(name);
        }

        private static int GetNumBackups() {
            if (BackupFolderExists()) {
                return Directory.GetDirectories(Settings.userSettings.backupsFolder.value).Length;
            }
            else {
                return 0;
            }
        }

        private static void CopyFolder(string path, string newPath) {
            Directory.CreateDirectory(newPath);
            string[] folders = Directory.GetDirectories(path);
            string[] files = Directory.GetFiles(path);

            foreach (string file in files) {
                File.Copy(file, file.Replace(path, newPath));
            }

            foreach (string folder in folders) {
                CopyFolder(folder, folder.Replace(path, newPath));
            }
        }

        private static void DeleteFolder(string path) {
            string[] files = Directory.GetFiles(path);
            string[] folders = Directory.GetDirectories(path);
            foreach (string file in files) File.Delete(file);
            foreach (string folder in folders) DeleteBackup(folder);
            Directory.Delete(path);
        }
    }
}