using Microsoft.Extensions.DependencyInjection;
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
        void Debug(string message, bool shortenPath = true);
        void Info(string messasge, bool shortenPath = true);
        void Warning(string message);
        void Error(string message);

        bool LogDebugToFile { get; set; }
    }

    public class LoggerService : ILoggerService
    {
        private const int paddingSize = 10;

        private ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private readonly object consoleLock = new object();
        private readonly StreamWriter logWriter;
        private bool missingLogPathNotified = false;

        private IProgramData programData;

        private bool _logDebugToFile = true;

        // Properties

        private string LogPath => programData.FilePaths.LogFile;
        private bool IsLogPathSet => !string.IsNullOrEmpty(LogPath);

        public bool LogDebugToFile {
            get => _logDebugToFile || programData.IsDebugBuild;
            set => _logDebugToFile = value;
        }

        // Constructors

        public LoggerService(IServiceProvider serviceProvider) {
            programData = serviceProvider.GetRequiredService<IProgramData>();
            
            if (File.Exists(LogPath)) File.Delete(LogPath);
            logWriter = File.CreateText(LogPath);
            StartLoggingThread();
        }

        // Public Functions

        public void Debug(string message, bool shortenPath) {
            lock (consoleLock) {
                if (shortenPath) message = message.Replace(programData.FilePaths.RootFolder, "");
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
                if (shortenPath) message = message.Replace(programData.FilePaths.RootFolder, "");
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
                    await WriteQueuedMessages();
                    await Task.Delay(1000);
                }
            });
        }

        private async Task WriteQueuedMessages() {
            if (queue.IsEmpty) return;
            while (queue.TryDequeue(out string? message)) {
                await logWriter.WriteLineAsync(message);
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
}
