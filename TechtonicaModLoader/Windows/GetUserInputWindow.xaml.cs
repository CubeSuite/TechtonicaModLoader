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
using System.Windows.Shapes;

namespace TechtonicaModLoader.Windows
{
    /// <summary>
    /// Interaction logic for GetUserInputWindow.xaml
    /// </summary>
    public partial class GetUserInputWindow : Window
    {
        public GetUserInputWindow()
        {
            if(Application.Current.MainWindow is MainWindow mainWindow) {
                Owner = mainWindow;
            }

            InitializeComponent();
        }

        // Properties

        #region ButtonCount Property

        public static readonly DependencyProperty ButtonCountProperty = DependencyProperty.Register("ButtonCount", typeof(int), typeof(GetUserInputWindow), new PropertyMetadata(2));

        public int ButtonCount {
            get => (int)GetValue(ButtonCountProperty);
            set => SetValue(ButtonCountProperty, value);
        }

        #endregion

        #region LeftButtonText Property

        public static readonly DependencyProperty LeftButtonTextProperty = DependencyProperty.Register("LeftButtonText", typeof(string), typeof(GetUserInputWindow), new PropertyMetadata("Confirm"));

        public string LeftButtonText {
            get => (string)GetValue(LeftButtonTextProperty);
            set => SetValue(LeftButtonTextProperty, value);
        }

        #endregion

        #region RightButtonText Property

        public static readonly DependencyProperty RightButtonTextProperty = DependencyProperty.Register("RightButtonText", typeof(string), typeof(GetUserInputWindow), new PropertyMetadata("Cancel"));

        public string RightButtonText {
            get => (string)GetValue(RightButtonTextProperty);
            set => SetValue(RightButtonTextProperty, value);
        }

        #endregion

        // Events

        private void OnConfirmClicked(object sender, RoutedEventArgs e) {
            DialogResult = true;
            Close();
        }

        private void OnCancelClicked(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
