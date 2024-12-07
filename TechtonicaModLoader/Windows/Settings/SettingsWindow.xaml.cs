﻿using System;
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
        public SettingsWindow(IServiceProvider serviceProvider) {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            DataContext = new SettingsWindowViewModel(serviceProvider);
        }

        private void OnCloseClicked(object sender, EventArgs e) {
            Close();
        }
    }
}
