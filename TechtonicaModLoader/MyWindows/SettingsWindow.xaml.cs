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
using TechtonicaModLoader.MyControls;
using TechtonicaModLoader.MyPanels.SettingsPanels;

namespace TechtonicaModLoader.MyWindows
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow() {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            Width = ProgramData.programWidth;
            Height = ProgramData.programHeight;
        }

        // Objects & Variables
        private List<string> doneCategories = new List<string>();
        private string currentCategory;

        // Events

        private void OnWindowLoaded(object sender, RoutedEventArgs e) {
            LoadCategories();
            LoadCategory(Settings.defaultCategory);
            Settings.SettingsChanged += OnSettingChanged;
        }

        private void OnRestoreDefaultsClicked(object sender, EventArgs e) {
            if (GuiUtils.GetUserConfirmation("Restore Defaults?", "Are you sure you want to restore the default settings? This cannot be undone.")) {
                Settings.userSettings.RestoreDefaults();
                Settings.LoadTheme();
                LoadCategory(currentCategory);
            }
        }

        private void OnCloseClicked(object sender, EventArgs e) {
            if (FileStructureUtils.ValidateGameFolder()) {
                Close();
            }
            else {
                GuiUtils.ShowErrorMessage("Invalid Game Folder", "Please check that your Game Folder setting is set correctly before exiting.");
            }
        }

        private void OnCategoryClicked(object sender, MouseButtonEventArgs e) {
            MyBounceLabel clickedLabel = sender as MyBounceLabel;
            LoadCategory(clickedLabel.displayLabel.Text);
        }

        private void OnSettingChanged(object sender, SettingChangedEventArgs e) {
            Dispatcher.Invoke(delegate () {
                closeButton.Source = "ControlBox/Close";
            });

            if (!e.changeFromGUI) LoadCategory(currentCategory);
        }

        // Private Functions

        private void LoadCategories() {
            doneCategories.Clear();
            foreach (Setting setting in Settings.userSettings.GetAllSettings()) {
                if (doneCategories.Contains(setting.category)) continue;

                string name = setting.category;
                int margin = 0;
                if (setting.category.Contains("/")) {
                    name = setting.category.Split('/').Last();
                    margin = 4 * setting.category.Split('/').Length - 1;
                }

                MyBounceLabel label = new MyBounceLabel(name) {
                    Margin = new Thickness(margin, 0, 0, 0)
                };
                label.MouseLeftButtonUp += OnCategoryClicked;
                categoriesPanel.Children.Add(label);
                doneCategories.Add(setting.category);
            }
        }

        private void LoadCategory(string category) {
            currentCategory = category;
            settingsPanel.Children.Clear();
            List<Setting> settings = Settings.userSettings.GetSettingsInCategory(category);
            foreach (Setting setting in settings) {
                if(setting.isHidden) continue;
                switch (setting.type) {
                    case "String":
                        settingsPanel.Children.Add(new StringSettingPanel((StringSetting)setting) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case "Combo":
                        settingsPanel.Children.Add(new ComboSettingPanel((ComboSetting)setting) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case "Int":
                        settingsPanel.Children.Add(new IntSettingPanel((IntSetting)setting) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case "Bool":
                        settingsPanel.Children.Add(new BoolSettingPanel((BoolSetting)setting) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case "Button":
                        settingsPanel.Children.Add(new ButtonSettingPanel((ButtonSetting)setting) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case "Colour":
                        settingsPanel.Children.Add(new ColourSettingPanel((ColourSetting)setting) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    default:
                        Log.Error($"Could not create setting panel for '{setting.name}' - unknown type: '{setting.type}'");
                        break;
                }
            }
        }

        // Return Functions

        public string result;
        private string GetResult() { return result; }
        public static string ShowSettingsWindow() {
            SettingsWindow window = new SettingsWindow();
            window.ShowDialog();
            return window.GetResult();
        }
    }
}
