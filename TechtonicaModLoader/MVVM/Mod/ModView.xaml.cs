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
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.MVVM.Mod
{
    /// <summary>
    /// Interaction logic for ModView.xaml
    /// </summary>
    public partial class ModView : UserControl
    {
        public ModView()
        {
            InitializeComponent();
            downloadsSvg.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\ModPanel\\Download.svg");
            ratingSvg.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\ModPanel\\Thumb.svg");
        }
    }
}
