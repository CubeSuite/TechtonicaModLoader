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
    /// Interaction logic for MyButton.xaml
    /// </summary>
    public partial class MyButton : UserControl
    {
        public MyButton() {
            InitializeComponent();
        }

        // Properties

        #region ButtonText Property

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(MyButton), new PropertyMetadata(""));

        public string ButtonText {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler LeftClicked;
        public event EventHandler RightClicked;

        // Events

        private async void OnMyButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            await Task.Delay(200);
            LeftClicked?.Invoke(this, EventArgs.Empty);
        }

        private async void OnMyButtonMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            await Task.Delay(200);
            RightClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
