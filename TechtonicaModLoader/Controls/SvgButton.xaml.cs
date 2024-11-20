using CommunityToolkit.Mvvm.Input;
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
using TechtonicaModLoader.Stores;

namespace TechtonicaModLoader.Controls
{
    /// <summary>
    /// Interaction logic for SvgButton.xaml
    /// </summary>
    public partial class SvgButton : UserControl
    {
        public SvgButton()
        {
            InitializeComponent();
        }

        // Properties

        #region Source Property

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(SvgButton), new PropertyMetadata("", OnSourceChanged));

        public string Source {
            get => (string)GetValue(SourceProperty);
            set {
                SetValue(SourceProperty, value);
                OnSourceChanged(this, new DependencyPropertyChangedEventArgs(SourceProperty, value, value));
            }
        }

        public static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            ((SvgButton)obj).svg.Source = new Uri($"{ProgramData.FilePaths.resourcesFolder}\\{((SvgButton)obj).Source}.svg");
        }

        #endregion

        #region SvgMargin Property

        public static readonly DependencyProperty SvgMarginProperty = DependencyProperty.Register("SvgMargin", typeof(Thickness), typeof(SvgButton), new PropertyMetadata(new Thickness(2)));

        public Thickness SvgMargin {
            get => (Thickness)GetValue(SvgMarginProperty);
            set => SetValue(SvgMarginProperty, value);
        }

        #endregion

        #region HandleClickCommand Property

        public static readonly DependencyProperty HandleClickCommandProperty = DependencyProperty.Register("HandleClickCommand", typeof(RelayCommand), typeof(SvgButton), new PropertyMetadata(null));

        public RelayCommand HandleClickCommand {
            get => (RelayCommand)GetValue(HandleClickCommandProperty);
            set => SetValue(HandleClickCommandProperty, value);
        }

        #endregion

        // Events

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (HandleClickCommand == null) return;
            HandleClickCommand.Execute(null);
        }
    }
}
