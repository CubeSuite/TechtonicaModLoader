using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Security.Permissions;
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
    public partial class MyComboBox : UserControl
    {
        public MyComboBox() {
            InitializeComponent();
        }

        // Properties

        #region Selected Item Alignment Property

        public static readonly DependencyProperty SelectedItemAlignmentProperty = DependencyProperty.Register("SelectedItemAlignment", typeof(HorizontalAlignment), typeof(MyComboBox), new PropertyMetadata(HorizontalAlignment.Left));

        public HorizontalAlignment SelectedItemAlignment {
            get => (HorizontalAlignment)GetValue(SelectedItemAlignmentProperty);
            set => SetValue(SelectedItemAlignmentProperty, value);
        }

        #endregion

        #region Selected Item Property

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(MyComboBox), new PropertyMetadata(""));

        public string SelectedItem {
            get => (string)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion

        #region Items Property

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(string), typeof(MyComboBox), new PropertyMetadata(""));

        public string Items {
            get => (string)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        #endregion

        #region Show Items Property

        public static readonly DependencyProperty ShowItemsProperty = DependencyProperty.Register("ShowItems", typeof(bool), typeof(MyComboBox), new PropertyMetadata(false));

        public bool ShowItems {
            get => (bool)GetValue(ShowItemsProperty);
            set => SetValue(ShowItemsProperty, value);
        }

        #endregion

        #region Searchable Property

        public static readonly DependencyProperty SearchableProperty = DependencyProperty.Register("Searchable", typeof(bool), typeof(MyComboBox), new PropertyMetadata(true));

        public bool Searchable {
            get => (bool)GetValue(SearchableProperty);
            set => SetValue(SearchableProperty, value);
        }

        #endregion

        // Objects & Variables

        public List<string> itemsAsList => Items.Split('|').ToList();
        private string lastItem;

        // Custom Events

        public event EventHandler SelectedItemChanged;

        // Events

        private void OnControlLoaded(object sender, RoutedEventArgs e) {
            LoadItems();
        }

        private void OnMyComboBoxLeftClicked(object sender, MouseButtonEventArgs e) {
            ShowItems = !ShowItems;
            if (Searchable && ShowItems) {
                SelectedItem = "";
                LoadItems();
            }
            else if (string.IsNullOrEmpty(SelectedItem) && !ShowItems) {
                SetItem(string.IsNullOrEmpty(lastItem) ? itemsAsList[0] : lastItem);
                SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void OnInputBoxGotFocus(object sender, KeyboardFocusChangedEventArgs e) {
            ShowItems = !ShowItems;
            SelectedItem = "";
            LoadItems();
        }

        private void OnItemsListMouseLeave(object sender, MouseEventArgs e) {
            ShowItems = false;
            if (Searchable) {
                if (!itemsAsList.Contains(displayLabel.Text)) {
                    SetItem(string.IsNullOrEmpty(lastItem) ? itemsAsList[0] : lastItem);
                    SelectedItemChanged?.Invoke(this, EventArgs.Empty);
                }

                Keyboard.ClearFocus();
            }
        }

        private void OnItemClicked(object sender, MouseButtonEventArgs e) {
            ShowItems = false;
            MyBounceLabel clickedLabel = sender as MyBounceLabel;
            SetItem(clickedLabel.LabelText);
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnInputBoxPreviewKeyUp(object sender, KeyEventArgs e) {
            LoadItems();
            if (itemsAsList.Contains(displayLabel.Text)) {
                SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            }

            if (e.Key == Key.Enter || e.Key == Key.Escape) {
                ShowItems = false;
                Keyboard.ClearFocus();
            }
        }

        // Public Functions

        public void AddItem(string item) {
            if (!string.IsNullOrEmpty(Items)) {
                item = "|" + item;
            }
            else {
                SelectedItem = item;
            }

            Items += item;
            LoadItems();
        }

        public void SetItem(int index) {
            if (index < 0 || index >= Items.Length) {
                Log.Error($"Cannot set Item to index {index} - out of range");
                return;
            }

            SetItem(itemsAsList[index]);
        }

        public void SetItem(string item) {
            if (!Items.Contains(item)) {
                Log.Warning($"Item '{item}' is not in list of Items: '{string.Join(", ", Items)}'");
            }

            SelectedItem = item;
            lastItem = item;
            displayLabel.Text = item;
        }

        public void SetItems(string items) {
            SetItems(items.Split('|').ToList());
        }

        public void SetItems(List<string> items) {
            if (items.Count == 0) {
                Log.Error($"Cannot set MyComboBox.Items to empty list");
                return;
            }

            Items = string.Join("|", items);
            LoadItems();
            SetItem(0);
        }

        // Private Functions

        private void LoadItems() {
            if (string.IsNullOrEmpty(Items)) return;

            itemsList.Children.Clear();
            if (!Searchable && (string.IsNullOrEmpty(SelectedItem) || !itemsAsList.Contains(SelectedItem))) {
                SetItem(0);
            }

            foreach (string item in itemsAsList) {
                if (Searchable && !item.ToLower().Contains(displayLabel.Text.ToLower())) {
                    continue;
                }

                AddItemLabel(item);
            }
        }

        private void AddItemLabel(string item) {
            MyBounceLabel label = new MyBounceLabel() {
                LabelText = item,
                Margin = new Thickness(2, 1, 2, 1)
            };
            label.MouseLeftButtonUp += OnItemClicked;
            itemsList.Children.Add(label);
        }
    }
}
