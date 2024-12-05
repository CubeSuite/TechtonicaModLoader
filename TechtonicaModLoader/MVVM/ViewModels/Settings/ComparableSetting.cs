using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public class ComparableSetting<T> : SettingBase where T : IComparable
    {
        // Members
        
        private Func<T> getValue;
        private Action<T> setValue;

        private static readonly Type[] typesToCheck = [typeof(int), typeof(float), typeof(double)];

        // Properties

        public T Min { get; }
        public T Max { get; }

        public T Value {
            get => getValue();
            set {
                if (typesToCheck.Contains(typeof(T))) {
                    if (Min != null && value.CompareTo(Min) < 0) {
                        Log.Warning($"Tried to set setting '{Name}' to '{value}' which is less than min '{Min}'");
                        value = Min;
                    }

                    if (Max != null && value.CompareTo(Max) > 0) {
                        Log.Warning($"Tried to set setting '{Name}' to '{value}' which is greater than max '{Max}'");
                        value = Max;
                    }
                }

                setValue(value);
            }
        }

        // Constructors

        public ComparableSetting(string name, string description, string category, Func<T> getValueFunc, Action<T> setValueFunc, T min, T max) : base(name, description, category) {
            getValue = getValueFunc;
            setValue = setValueFunc;

            Min = min;
            Max = max;
        }
    }
}
