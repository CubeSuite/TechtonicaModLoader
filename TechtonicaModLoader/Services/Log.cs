using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader
{
    public class Log
    {
        // Members

        private static ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private const int paddingSize = 10;
        private bool missingLogPathNotified = false;
        private static readonly object consoleLock = new object();

        // Properties

        private static string LogPath => ProgramData.FilePaths.logFile;
        private static bool IsLogPathSet => !string.IsNullOrEmpty(LogPath);

        private static bool _logDebugToFile = true;
        public static bool LogDebugToFile {
            get => _logDebugToFile || ProgramData.isDebugBuild;
            set => _logDebugToFile = value;
        }

        // Constructors

        public Log()
        {
            if (File.Exists(LogPath)) File.Delete(LogPath);
            StartLoggingThread();
        }

        // Log Functions

        public static void Debug(string message) {
            lock (consoleLock) {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[Debug]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                if (LogDebugToFile) {
                    WriteMessageToFile("Debug", message);
                }
            }
        }

        public static void Info(string message) {
            lock (consoleLock) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[Info]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                WriteMessageToFile("Info", message);
            }
        }

        public static void Warning(string message) {
            lock (consoleLock) {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("[Warning]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                WriteMessageToFile("Warning", message);
            }
        }

        public static void Error(string message) {
            lock (consoleLock) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[Error]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                WriteMessageToFile("Error", message);
            }
        }

        // Public Functions

        public static void WriteQueuedMessages() {
            if (queue.Count == 0) return;
            lock (queue) {
                File.AppendAllLines(LogPath, queue);
                queue.Clear();
            }
        }

        // Private Functions

        private async void StartLoggingThread() {
            if (!IsLogPathSet && !missingLogPathNotified) {
                missingLogPathNotified = true;
                Warning("You have not set the log file path. Logging thread will not be started");
                return;
            }

            await Task.Run(async () => {
                while (true) {
                    WriteQueuedMessages();
                    await Task.Delay(1000);
                }
            });
        }

        private static void WriteMessageToConsole(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"| {message}");
        }

        private static void WriteMessageToFile(string level, string message) {
            level = $"[{level}]".PadRight(paddingSize);
            string line = $"{level}| {message}";
            queue.Enqueue(line);
        }
    }
}
