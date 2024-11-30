using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public class Setting<T> : SettingBase
    {
        // Members
        private readonly Func<T> getValue;
        private readonly Action<T> setValue;

        // Properties

        public T Value {
            get => getValue();
            set => setValue(value);
        }

        // Constructors

        public Setting(string name, string description, string category, bool isVisible, Func<T> getValueFunc, Action<T> setValueFunc) : base(name, description, category, isVisible) {
            getValue = getValueFunc;
            setValue = setValueFunc;
        }
    }
}
