using System.Windows;
using TechtonicaModLoader.MVVM.Settings.ViewModels;
using TechtonicaModLoader.Services;

namespace TechtonicaModLoader.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SettingsWindowViewModel _settingsWindowViewModel;

        public SettingsWindow(SettingsWindowViewModel settingsWindowViewModel, IDialogService dialogService)
        {
            _settingsWindowViewModel = settingsWindowViewModel;

            Owner = Application.Current.MainWindow;
            InitializeComponent();

            _settingsWindowViewModel.CloseButtonClicked += OnCloseButtonClicked;
            DataContext = _settingsWindowViewModel;
        }

        private void OnCloseButtonClicked() {
            Close();
        }
    }
}
