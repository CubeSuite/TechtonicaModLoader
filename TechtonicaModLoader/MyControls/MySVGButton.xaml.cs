using MyLogger;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for MySVGButton.xaml
    /// </summary>
    public partial class MySVGButton : UserControl
    {
        public MySVGButton() {
            InitializeComponent();
        }

        // Properties

        #region Source Property

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(MySVGButton), new PropertyMetadata("", OnSourceChanged));

        public string Source {
            get => (string)GetValue(SourceProperty);
            set {
                SetValue(SourceProperty, value);
                OnSourceChanged(this, new DependencyPropertyChangedEventArgs(SourceProperty, value, value));
            }
        }

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            MySVGButton thisMySVGButton = obj as MySVGButton;
            string svgPath = $"{ProgramData.FilePaths.resourcesFolder}\\{thisMySVGButton.Source}.svg";
            if (File.Exists(svgPath)) {
                thisMySVGButton.svg.Source = new Uri(svgPath);
            }
            else {
                Log.Error($"Could not set SVG source, file does not exist - '{svgPath}'");
            }
        }

        #endregion

        #region SVG Margin Property

        public static readonly DependencyProperty SVGMarginProperty = DependencyProperty.Register("SVGMargin", typeof(Thickness), typeof(MySVGButton), new PropertyMetadata(new Thickness(0)));

        public Thickness SVGMargin {
            get => (Thickness)GetValue(SVGMarginProperty);
            set => SetValue(SVGMarginProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler LeftClicked;
        public event EventHandler RightClicked;

        // Events

        private async void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            await Task.Delay(200);
            LeftClicked?.Invoke(this, EventArgs.Empty);
        }

        private async void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e) {
            await Task.Delay(200);
            RightClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
