namespace TechtonicaModLoader.Stores.Settings.V1
{
    internal class BasicSettingV1<T>
    {
        public T? Value { get; set; }

        public BasicSettingV1(T value) {
            Value = value;
        }

        public BasicSettingV1() {
            Value = default;
        }
    }
}
