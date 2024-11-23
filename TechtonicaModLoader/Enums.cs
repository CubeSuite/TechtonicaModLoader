using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace TechtonicaModLoader
{
    public enum ModListSource
    {
        [Description("All")]
        All,

        [Description("New")]
        New,

        [Description("Downloaded")]
        Downloaded,

        [Description("Not Downloaded")]
        NotDownloaded,

        [Description("Enabled")]
        Enabled,

        [Description("Disabled")]
        Disabled
    }

    public enum OfflineModListSource
    {
        [Description("Downloaded")]
        Downloaded,

        [Description("Enabled")]
        Enabled,

        [Description("Disabled")]
        Disabled,

    }

    public enum ModListSortOption
    {
        [Description("Last Updated")]
        LastUpdated,

        [Description("Newest")]
        Newest,

        [Description("Alphabetical")]
        Alphabetical,

        [Description("Downloads")]
        Downloads,

        [Description("Popularity")]
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
