using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TechtonicaModLoader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            string crashLogText = $"Log Content:\n\n{File.ReadAllText(ProgramData.FilePaths.logFile)}\n\n";
            crashLogText += $"Error:\n\n{e.Exception.Message}\n\n";
            crashLogText += $"Stack Trace:\n\n{e.Exception.StackTrace}";
            File.WriteAllText($"{ProgramData.FilePaths.crashReportsFolder}\\CrashReport.log", crashLogText);

            GuiUtils.ShowErrorMessage($"Sorry, TML Has Crashed", "A folder will open containing a 'CrashReport.log' file, please send it to @Equinox on the Techtonica Discord sever.");
            Process.Start(ProgramData.FilePaths.crashReportsFolder);
        }
    }
}
