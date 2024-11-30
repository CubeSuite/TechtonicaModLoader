using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public class EnumSetting<T> : SettingBase where T : Enum
    {
        // Members
        private Func<T> getValue;
        private Action<T> setValue;

        // Properties

        public T Value {
            get => getValue();
            set => setValue(value);
        }

        public T[] Options => (T[])Enum.GetValues(typeof(T));

        // Constructors

        public EnumSetting(string name, string description, string category, bool isVisible, Func<T> getValueFunc, Action<T> setValueFunc) : base(name, description, category, isVisible) {
            getValue = getValueFunc;
            setValue = setValueFunc;
        }
    }
}
