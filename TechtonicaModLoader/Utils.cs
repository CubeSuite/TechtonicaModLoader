using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader
{
    public interface IDebugUtils
    {
        void Assert(bool condition, string message);
        void CrashIfDebug(string message);
    }

    public class DebugUtils : IDebugUtils
    {
        // Members

        private ILoggerService logger;
        private IProgramData programData;

        // Constructors

        public DebugUtils(IServiceProvider serviceProvider) {
            logger = serviceProvider.GetRequiredService<ILoggerService>();
            programData = serviceProvider.GetRequiredService<IProgramData>();
        }

        // Public Functions

        public void CrashIfDebug(string message) {
            if (programData.IsDebugBuild) throw new Exception(message);
        }

        public void Assert(bool condition, string message) {
            if (!condition) {
                logger.Error($"Assert Failed: {message}");
                CrashIfDebug(message);
            }
        }
    }
}
