namespace TechtonicaModLoader.Stores.Settings.V1
{
    internal class SeenModsSettingV1 : BasicSettingV1<string>
    {
        public List<string> GetSeenMods() {
            return Value != null ? new List<string>(Value.Split('|')) : [];
        }
    }
}
