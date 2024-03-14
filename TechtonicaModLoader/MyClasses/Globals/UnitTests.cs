using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.Modes.Globals
{
    public static partial class UnitTests
    {
        // Objects & Variables
        public enum Test
        {
            exampleTest,
        }

        // Public Functions

        public static async Task<bool> runAll() {
            bool allTestsPassed = true;
            foreach (Test test in Enum.GetValues(typeof(Test))) {
                if (!await runTest(test)) {
                    allTestsPassed = false;
                }
            }

            if (!allTestsPassed) {
                Log.Warning("Not all unit tests passed");
            }

            return allTestsPassed;
        }

        public static async Task<bool> runTest(Test test) {
            switch (test) {
                case Test.exampleTest: return await testExampleTest();
                default:
                    string error = $"Could not find test function for test: '{Enum.GetName(typeof(Test), test)}'";
                    Log.Error(error);
                    DebugTools.DebugCrash(error);
                    return false;
            }
        }

        // Tests
        // Try use partial classes in the same
        // file as the code that is being tested

        private static async Task<bool> testExampleTest() {
            await Task.Delay(0);
            Log.Debug("testExampleTest passed");
            return true;
        }
    }
}
