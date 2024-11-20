using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TechtonicaModLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        // Properties

        public static MainWindow current => (MainWindow)Application.Current.MainWindow;

        private void OnDragMoveBarMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            current.DragMove();
        }
    }
}