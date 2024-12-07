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

        private readonly IReadOnlyCollection<KeyValuePair<T, string>> options;

        // Properties

        public T Value {
            get => getValue();
            set => setValue(value);
        }

        public IReadOnlyCollection<KeyValuePair<T, string>> Options => options;

        // Constructors

        public EnumSetting(string name, string description, string category, IReadOnlyCollection<KeyValuePair<T, string>> options, Func<T> getValueFunc, Action<T> setValueFunc) : base(name, description, category) {
            getValue = getValueFunc;
            setValue = setValueFunc;
            this.options = options;
        }
    }
}
