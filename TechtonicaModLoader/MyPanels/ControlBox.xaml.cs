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
using TechtonicaModLoader.MyWindows;

namespace TechtonicaModLoader.MyPanels
{
    /// <summary>
    /// Interaction logic for ControlBox.xaml
    /// </summary>
    public partial class ControlBox : UserControl
    {
        public ControlBox() {
            InitializeComponent();
        }

        // Events

        private void OnSettingsClicked(object sender, EventArgs e) {
            SettingsWindow.ShowSettingsWindow();
        }

        private void OnMoveButtonMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MainWindow.current.DragMove();
        }

        private void OnMinimiseClicked(object sender, EventArgs e) {
            MainWindow.current.WindowState = WindowState.Minimized;
        }

        private void OnCloseClicked(object sender, EventArgs e) {
            Application.Current.Shutdown();
        }

        // Public Functions

        public void RefreshIcons() {
            settingsButton.Source = "ControlBox/Settings";
            moveButton.Source = "ControlBox/Move";
            minimsieButton.Source = "ControlBox/Minimise";
            closeButton.Source = "ControlBox/Close";
        }
    }
}
