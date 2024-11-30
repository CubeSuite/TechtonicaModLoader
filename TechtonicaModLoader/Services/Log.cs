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
    public interface ILoggerService 
    {
        void Debug(string message, bool shortenPath);
        void Info(string messasge, bool shortenPath);
        void Warning(string message);
        void Error(string message);

        bool LogDebugToFile { get; set; }
    }

    public class LoggerService : ILoggerService
    {
        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private const int paddingSize = 10;
        private bool missingLogPathNotified = false;
        private readonly object consoleLock = new object();

        // Properties

        private string LogPath => ProgramData.FilePaths.LogFile;
        private bool IsLogPathSet => !string.IsNullOrEmpty(LogPath);

        private bool _logDebugToFile = true;
        public bool LogDebugToFile {
            get => _logDebugToFile || ProgramData.IsDebugBuild;
            set => _logDebugToFile = value;
        }

        // Constructors

        public LoggerService() {
            if (File.Exists(LogPath)) File.Delete(LogPath);
            StartLoggingThread();
        }

        // Public Functions

        public void Debug(string message, bool shortenPath) {
            lock (consoleLock) {
                if (shortenPath) message = message.Replace(ProgramData.FilePaths.RootFolder, "");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("[Debug]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                if (LogDebugToFile) {
                    WriteMessageToFile("Debug", message);
                }
            }
        }

        public void Info(string message, bool shortenPath) {
            lock (consoleLock) {
                if (shortenPath) message = message.Replace(ProgramData.FilePaths.RootFolder, "");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[Info]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                WriteMessageToFile("Info", message);
            }
        }

        public void Warning(string message) {
            lock (consoleLock) {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("[Warning]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                WriteMessageToFile("Warning", message);
            }
        }

        public void Error(string message) {
            lock (consoleLock) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[Error]".PadRight(paddingSize));
                WriteMessageToConsole(message);
                WriteMessageToFile("Error", message);
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

        private void WriteQueuedMessages() {
            if (queue.Count == 0) return;
            lock (queue) {
                File.AppendAllLines(LogPath, queue);
                queue.Clear();
            }
        }

        private void WriteMessageToConsole(string message) {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"| {message}");
        }

        private void WriteMessageToFile(string level, string message) {
            level = $"[{level}]".PadRight(paddingSize);
            string line = $"{level}| {message}";
            queue.Enqueue(line);
        }
    }

    public static class Log 
    {
        private static ILoggerService? logger;

        public static bool LogDebugToFile {
            get => logger?.LogDebugToFile ?? false;
            set {
                if (logger != null) logger.LogDebugToFile = value;
            }
        }

        // Public Functions

        public static void Initialise(ILoggerService logger) {
            Log.logger = logger;
        }

        public static void Debug(string message, bool shortenPath = true) {
            logger?.Debug(message, shortenPath);
        }

        public static void Info(string message, bool shortenPath = true) {
            logger?.Info(message, shortenPath);
        }

        public static void Warning(string message) {
            logger?.Warning(message);
        }

        public static void Error(string message) {
            logger?.Error(message);
        }
    }
}
