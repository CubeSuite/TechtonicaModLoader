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

namespace TechtonicaModLoader.MyWindows.GetWindows
{
    public partial class GetIntWindow : Window
    {
        public GetIntWindow(string title, int min, int max, int? defaultValue) {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;
            titleLabel.Content = title;

            inputBox.Min = min;
            inputBox.Max = max;
            inputBox.Value = defaultValue == null ? min : (int)defaultValue;
        }

        // Objects & Variables
        public const int canceledInput = -123456;

        // Events

        private void OnConfirmClicked(object sender, EventArgs e) {
            result = inputBox.Value;
            Close();
        }

        private void OnCancelClicked(object sender, EventArgs e) {
            result = canceledInput;
            Close();
        }

        // Return Functions

        private int result;
        private int GetResult() { return result; }
        public static int GetInt(string title, int min, int max, int? defaultValue) {
            GetIntWindow window = new GetIntWindow(title, min, max, defaultValue);
            window.ShowDialog();
            return window.GetResult();
        }
    }
}
