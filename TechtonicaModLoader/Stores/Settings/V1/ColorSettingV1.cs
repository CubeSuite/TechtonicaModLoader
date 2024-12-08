using System.Drawing;

namespace TechtonicaModLoader.Stores.Settings.V1
{
    internal class ColorSettingV1 : BasicSettingV1<string>
    {
        public Color GetColor() {
            return !string.IsNullOrEmpty(Value) ? ColorTranslator.FromHtml(Value) : Color.CornflowerBlue;
        }
    }
}
