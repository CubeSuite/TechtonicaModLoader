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

namespace TechtonicaModLoader.MyControls
{
    /// <summary>
    /// Interaction logic for MyCheckBox.xaml
    /// </summary>
    public partial class MyCheckBox : UserControl
    {
        public MyCheckBox() {
            InitializeComponent();
        }

        // Properties

        #region Is Checked Property

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(MyCheckBox), new PropertyMetadata(false));

        public bool IsChecked {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        #endregion

        #region IsEditable Property

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(MyCheckBox), new PropertyMetadata(true));

        public bool IsEditable {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler IsCheckedChanged;

        // Events

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (!IsEditable) return;
            IsChecked = !IsChecked;
            IsCheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
