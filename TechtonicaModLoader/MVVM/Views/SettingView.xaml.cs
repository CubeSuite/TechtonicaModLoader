using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TechtonicaModLoader.MVVM.ViewModels.Settings;

namespace TechtonicaModLoader.MVVM.Views
{
    /// <summary>
    /// Interaction logic for SettingView.xaml
    /// </summary>
    public partial class SettingView : UserControl
    {
        public SettingView()
        {
            InitializeComponent();
        }
    }

    public class SettingTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? BoolTemplate { get; set; }
        public DataTemplate? StringTemplate { get; set; }
        public DataTemplate? ButtonTemplate { get; set; }
        public DataTemplate? EnumTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container) {
            if (item is Setting<bool>) return BoolTemplate;
            if (item is Setting<string>) return StringTemplate;
            if (item is ButtonSetting) return ButtonTemplate;
            if (item is EnumSetting<ModListSource>) return EnumTemplate;
            if (item is EnumSetting<ModListSortOption>) return EnumTemplate;

            if(item != null) {
                throw new Exception($"Could not get template for unknown setting type: {item}");
            }

            return base.SelectTemplate(item, container);
        }
    }
}
