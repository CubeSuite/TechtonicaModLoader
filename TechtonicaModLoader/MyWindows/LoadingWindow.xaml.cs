using MyLogger;
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
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        public LoadingWindow() {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;
        }

        // Public Functions

        public void SetProgress(string info, int done, int max) {
            Dispatcher.Invoke(delegate () {
                infoLabel.Text = info;
                doneLabel.Text = done.ToString();
                maxLabel.Text = max.ToString();

                double ratio = done / (double)max;
                double rightMargin = (outerBorder.ActualWidth - 5.0) * (1.0 - ratio);
                if (rightMargin > outerBorder.ActualWidth) rightMargin = outerBorder.ActualWidth;
                Log.Debug("Right Margin: " + rightMargin.ToString());
                progressBar.Margin = new Thickness(5, 5, rightMargin, 5);
            });
        }
    }
}
