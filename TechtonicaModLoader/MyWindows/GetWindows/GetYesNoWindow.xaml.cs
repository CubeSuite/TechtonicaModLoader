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
    /// <summary>
    /// Interaction logic for GetYesNoWindow.xaml
    /// </summary>
    public partial class GetYesNoWindow : Window
    {
        public GetYesNoWindow(string title, string description) {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;
            titleLabel.Content = title;
            descriptionLabel.Text = description;
        }

        // Events

        private void OnYesClicked(object sender, EventArgs e) {
            result = true;
            Close();
        }

        private void OnNoClicked(object sender, EventArgs e) {
            result = false;
            Close();
        }

        // Return Functions

        bool result;
        public bool GetResult() { return result; }
        public static bool GetYesNo(string title, string description) {
            GetYesNoWindow window = new GetYesNoWindow(title, description);
            window.ShowDialog();
            return window.GetResult();
        }
    }
}
