using MyLogger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation.Peers;

namespace TechtonicaModLoader
{
    #region MyEnum

    public enum MyEnum
    {
        Item1,
        Null,
    }

    public static partial class StringUtils
    {
        public static MyEnum GetMyEnumFromName(string name) {
            switch (name) {
                case "Item1": return MyEnum.Item1;

                default:
                    Log.Error($"No member of MyEnum has name '{name}'");
                    return MyEnum.Null;
            }
        }

        public static string GetMyEnumName(MyEnum myEnum) {
            switch (myEnum) {
                case MyEnum.Item1: return "Item1";
                case MyEnum.Null: return "Null";
                default:
                    string defaultName = Enum.GetName(typeof(MyEnum), myEnum);
                    Log.Error($"Name not set for MyEnum member: {defaultName}");
                    return defaultName;
            }
        }

        public static List<string> GetAllMyEnumNames() {
            List<string> allNames = new List<string>();
            foreach (MyEnum myEnum in Enum.GetValues(typeof(MyEnum))) {
                allNames.Add(GetMyEnumName(myEnum));
            }

            return allNames;
        }
    }

    #endregion
}
