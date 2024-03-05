using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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

namespace TechtonicaModLoader.MyControls
{
    /// <summary>
    /// Interaction logic for MyBounceLabel.xaml
    /// </summary>
    public partial class MyBounceLabel : UserControl
    {
        public MyBounceLabel() {
            InitializeComponent();
        }

        public MyBounceLabel(string labelText) {
            InitializeComponent();
            LabelText = labelText;
        }

        // Properties

        #region Label Text Property

        public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register("LabelText", typeof(string), typeof(MyBounceLabel), new PropertyMetadata(""));

        public string LabelText {
            get => (string)GetValue(LabelTextProperty);
            set => SetValue(LabelTextProperty, value);
        }

        #endregion
    }
}
