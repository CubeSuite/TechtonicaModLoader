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
using TechtonicaModLoader.MyClasses;
using TechtonicaModLoader.MyControls;
using TechtonicaModLoader.MyPanels.SettingsPanels;

namespace TechtonicaModLoader.MyWindows
{
    /// <summary>
    /// Interaction logic for ModConfigWindow.xaml
    /// </summary>
    public partial class ModConfigWindow : Window
    {
        public ModConfigWindow() {
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
            LoadCategory(ModConfig.activeConfig.GetCategories().First());
        }

        private void OnRestoreDefaultsClicked(object sender, EventArgs e) {
            if (GuiUtils.GetUserConfirmation("Restore Defaults?", "Are you sure you want to restore the default settings? This cannot be undone.")) {
                ModConfig.activeConfig.RestoreDefaults();
                LoadCategory(currentCategory);
                Log.Debug($"Restored defaults for mod config");
            }
        }

        private void OnCloseClicked(object sender, EventArgs e) {
            Close();
        }

        private void OnCategoryClicked(object sender, MouseButtonEventArgs e) {
            MyBounceLabel clickedLabel = sender as MyBounceLabel;
            LoadCategory(clickedLabel.displayLabel.Text);
        }

        // Private Functions

        private void LoadCategories() {
            doneCategories.Clear();
            foreach(ConfigOption option in ModConfig.activeConfig.options) {
                if (doneCategories.Contains(option.category)) continue;

                string name = option.category;
                int margin = 0;
                if (option.name.Contains("/")) {
                    name = option.name.Split('/').Last();
                    margin = 4 * option.category.Split('/').Length - 1;
                }

                MyBounceLabel label = new MyBounceLabel(name) {
                    Margin = new Thickness(margin, 0, 0, 0)
                };
                label.MouseLeftButtonUp += OnCategoryClicked;
                categoriesPanel.Children.Add(label);
                doneCategories.Add(option.category);
            }
        }

        private void LoadCategory(string category) {
            currentCategory = category;
            settingsPanel.Children.Clear();
            List<ConfigOption> optionsInCategory = ModConfig.activeConfig.GetOptionsInCategory(category);
            foreach (ConfigOption option in optionsInCategory) {
                switch (option.optionType) {
                    case ConfigOptionTypes.stringOption:
                        settingsPanel.Children.Add(new StringSettingPanel((StringConfigOption)option) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case ConfigOptionTypes.keyboardShortcutOption:
                        settingsPanel.Children.Add(new StringSettingPanel((KeyboardShortcutConfigOption)option){
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case ConfigOptionTypes.keycodeOption:
                        settingsPanel.Children.Add(new StringSettingPanel((KeyCodeConfigOption)option){
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case ConfigOptionTypes.intOption:
                        settingsPanel.Children.Add(new IntSettingPanel((IntConfigOption)option) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case ConfigOptionTypes.floatOption:
                        settingsPanel.Children.Add(new StringSettingPanel((FloatConfigOption)option) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case ConfigOptionTypes.doubleOption:
                        settingsPanel.Children.Add(new StringSettingPanel((DoubleConfigOption)option) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    case ConfigOptionTypes.boolOption:
                        settingsPanel.Children.Add(new BoolSettingPanel((BooleanConfigOption)option) {
                            Margin = new Thickness(10, 5, 10, 5)
                        });
                        break;

                    default:
                        Log.Error($"Could not create setting panel for '{option.name}' - unknown type: '{option.optionType}'");
                        break;
                }
            }
        }

        // Return Functions

        public string result;
        private string GetResult() { return result; }
        public static string EditActiveConfig() {
            ModConfigWindow window = new ModConfigWindow();
            window.ShowDialog();
            return window.GetResult();
        }
    }
}
