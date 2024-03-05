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
    public partial class GetStringWindow : Window
    {
        public GetStringWindow(string title, string hint) {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;
            titleLabel.Content = title;
            inputBox.Hint = hint;
        }

        // Objects & Variables
        public const string canceledInput = "UserCanceledInput";

        // Events

        private void OnInputBoxEnterPressed(object sender, EventArgs e) {
            result = inputBox.Input;
            Close();
        }

        private void OnConfirmClicked(object sender, EventArgs e) {
            result = inputBox.Input;
            Close();
        }

        private void OnCancelClicked(object sender, EventArgs e) {
            result = canceledInput;
            Close();
        }

        // Return Functions

        private string result;
        private string GetResult() { return result; }
        public static string GetString(string title, string hint) {
            GetStringWindow window = new GetStringWindow(title, hint);
            window.ShowDialog();
            return window.GetResult();
        }
    }
}
