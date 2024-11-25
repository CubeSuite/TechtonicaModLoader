using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Services;

namespace TechtonicaModLoader.Stores
{
    public class ModFilesManager
    {
        // Members
        private IDialogService dialogService;

        // Constructors

        public ModFilesManager(IDialogService dialogService) {
            this.dialogService = dialogService;
        }

        // Public Functions

        public bool ProcessZipFile(string zipFileLocation) {
            if (!File.Exists(zipFileLocation)) {
                string error = $"zip file does not exist: '{zipFileLocation}'";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }

            UnzipToTemp(zipFileLocation);

            return true;
        }

        // Private Functions

        private bool UnzipToTemp(string zipFileLocation) {
            string zipName = Path.GetFileNameWithoutExtension(zipFileLocation);
            try {
                Log.Debug($"Unzipping '{zipName}'");
                using (ZipArchive archive = ZipFile.OpenRead(zipFileLocation)) {
                    foreach (ZipArchiveEntry entry in archive.Entries) {
                        string entryFilePath = Path.Combine($"{ProgramData.FilePaths.unzipFolder}\\{zipName}", entry.FullName);
                        Directory.CreateDirectory(Path.GetDirectoryName(entryFilePath));
                        if (!entryFilePath.EndsWith("/")) {
                            entry.ExtractToFile(entryFilePath, true);
                            Log.Debug($"Extracted '{entryFilePath}'");
                        }
                    }
                }

                return true;
            }
            catch (Exception e) {
                string error = $"Errorc occured while unzipping '{zipName}'.zip: {e.Message}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);

                if (!ProgramData.isDebugBuild) {
                    dialogService.ShowErrorMessage("Error Occurred While Unzipping Mod", "Please click the bug report button.");
                    // ToDo: Auto open and populate bug-report View
                }

                return false;
            }
        }
    }
}
