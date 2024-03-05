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

namespace TechtonicaModLoader.MyWindows
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        public WarningWindow(WarningType type, string title, string description, string closeButtonText) {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;

            switch (type) {
                case WarningType.Info:
                    titleLabel.Foreground = Brushes.White;
                    icon1.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\GUI\\Info.svg");
                    icon2.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\GUI\\Info.svg");
                    break;
                case WarningType.Warning:
                    titleLabel.Foreground = Brushes.Yellow;
                    icon1.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\GUI\\Warning.svg");
                    icon2.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\GUI\\Warning.svg");
                    break;
                case WarningType.Error:
                    titleLabel.Foreground = Brushes.Red;
                    icon1.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\GUI\\Error.svg");
                    icon2.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\GUI\\Error.svg");
                    break;
            }

            titleLabel.Content = title;
            descriptionLabel.Text = description;
            closeButton.ButtonText = closeButtonText;
        }

        // Events

        private void OnCloseClicked(object sender, EventArgs e) {
            Close();
        }

        // Return Functions

        private readonly string result = "";
        private string GetResult() { return result; }

        public static string ShowInfo(string title, string description, string closeButtonText) {
            WarningWindow window = new WarningWindow(WarningType.Info, title, description, closeButtonText);
            window.ShowDialog();
            return window.GetResult();
        }

        public static string ShowWarning(string title, string description, string closeButtonText) {
            WarningWindow window = new WarningWindow(WarningType.Warning, title, description, closeButtonText);
            window.ShowDialog();
            return window.GetResult();
        }

        public static string ShowError(string title, string description, string closeButtonText) {
            WarningWindow window = new WarningWindow(WarningType.Error, title, description, closeButtonText);
            window.ShowDialog();
            return window.GetResult();
        }
    }

    public enum WarningType
    {
        Info,
        Warning,
        Error
    }
}
