using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace TechtonicaModLoader
{
    public enum ModListSource
    {
        All,
        New,
        Downloaded,
        NotDownloaded,
        Enabled,
        Disabled
    }

    public enum OfflineModListSource
    {
        Downloaded,
        Enabled,
        Disabled,
    }

    public enum ModListSortOption
    {
        LastUpdated,
        Newest,
        Alphabetical,
        Downloads,
        Popularity,

    }

    public class EnumDescriptionConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) return null;

            FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo != null) {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0) return attributes[0].Description;
            }

            return value.ToString();
        }

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) return null;

            foreach (FieldInfo field in targetType.GetFields()) {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0 && attributes[0].Description == value.ToString()) {
                    return Enum.Parse(targetType, field.Name);
                }
            }

            return Enum.Parse(targetType, value?.ToString() ?? "");
        }
    }
}
