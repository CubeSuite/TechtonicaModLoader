using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader
{
    public static class Log
    {
        // Objects & Variables
        public static string logPath;
        public static bool logDebugToFile = false;

        private const int paddingSize = 10;
        private static bool isLogPathSet => !string.IsNullOrEmpty(logPath);
        private static bool isFirstLogLine = true;
        private static bool missingLogPathNotified = false;

        // Public Functions

        public static void InitialiseLog() {
            logPath = ProgramData.FilePaths.logFile;
            logDebugToFile = ProgramData.logDebugMessages || ProgramData.isDebugBuild;
            ClearLog();
        }

        public static void ClearLog() {
            if (isLogPathSet) {
                File.Delete(logPath);
            }
            else {
                Error("Could not clear log, you have not set the path");
            }
        }

        public static void Debug(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("[Debug]".PadRight(paddingSize));
            WriteMessageToConsole(message);
            if (logDebugToFile) {
                WriteMessageToFile("Debug", message);
            }
        }

        public static void Info(string message) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[Info]".PadRight(paddingSize));
            WriteMessageToConsole(message);
            WriteMessageToFile("Info", message);
        }

        public static void Warning(string message) {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[Warning]".PadRight(paddingSize));
            WriteMessageToConsole(message);
            WriteMessageToFile("Warning", message);

        }

        public static void Error(string message) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[Error]".PadRight(paddingSize));
            WriteMessageToConsole(message);
            WriteMessageToFile("Error", message);
        }

        // Private Functions

        private static void WriteMessageToConsole(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"| {message}");
        }

        private static void WriteMessageToFile(string level, string message) {
            if (isLogPathSet) {
                level = $"[{level}]".PadRight(paddingSize);
                string line = $"{level}| {message}";
                if (!isFirstLogLine) line = $"{Environment.NewLine}{line}";
                File.AppendAllText(logPath, line);
                isFirstLogLine = false;
            }
            else if (!missingLogPathNotified) {
                missingLogPathNotified = true;
                Warning("You have not set the log file path");
            }
        }
    }
}
