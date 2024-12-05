
namespace TechtonicaModLoader.MVVM.ViewModels.Settings
{
    public class EnumSetting<T> : SettingBase where T : Enum
    {
        // Members
        private readonly Func<T> getValue;
        private readonly Action<T> setValue;

        // Properties

        public T Value {
            get => getValue();
            set => setValue(value);
        }

        public T[] Options => (T[])Enum.GetValues(typeof(T));

        // Constructors

        public EnumSetting(string name, string description, string category, Func<T> getValueFunc, Action<T> setValueFunc) : base(name, description, category) {
            getValue = getValueFunc;
            setValue = setValueFunc;
        }
    }
}
