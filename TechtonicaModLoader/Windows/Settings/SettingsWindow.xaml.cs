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
using TechtonicaModLoader.Services;
using TechtonicaModLoader.Stores;
using TechtonicaModLoader.Windows.Settings;

namespace TechtonicaModLoader.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(UserSettings userSettings, IDialogService dialogService)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();

            SettingsWindowViewModel viewModel = new SettingsWindowViewModel(userSettings, dialogService);
            viewModel.CloseButtonClicked += OnCloseButtonClicked;
            DataContext = viewModel;
        }

        private void OnCloseButtonClicked() {
            Close();
        }
    }
}
