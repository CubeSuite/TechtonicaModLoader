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

namespace TechtonicaModLoader.MyControls
{
    public partial class MyUpDown : UserControl
    {
        public MyUpDown() {
            InitializeComponent();
        }

        // Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(MyUpDown), new PropertyMetadata(0, onValueChanged));

        public int Value {
            get => (int)GetValue(ValueProperty);
            set {
                SetValue(ValueProperty, value);
                onValueChanged(this, new DependencyPropertyChangedEventArgs(ValueProperty, value, value));
            }
        }

        private static void onValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            MyUpDown thisMyUpDown = obj as MyUpDown;
            if (thisMyUpDown.Value < thisMyUpDown.Min) thisMyUpDown.Value = thisMyUpDown.Min;
            if (thisMyUpDown.Value > thisMyUpDown.Max) thisMyUpDown.Value = thisMyUpDown.Max;
            thisMyUpDown.ValueChanged?.Invoke(thisMyUpDown, EventArgs.Empty);
        }

        #endregion

        #region Min Property

        public static readonly DependencyProperty MinProperty = DependencyProperty.Register("Min", typeof(int), typeof(MyUpDown), new PropertyMetadata(int.MinValue));

        public int Min {
            get => (int)GetValue(MinProperty);
            set => SetValue(MinProperty, value);
        }

        #endregion

        #region Max Property

        public static readonly DependencyProperty MaxProperty = DependencyProperty.Register("Max", typeof(int), typeof(MyUpDown), new PropertyMetadata(int.MaxValue));

        public int Max {
            get => (int)GetValue(MaxProperty);
            set => SetValue(MaxProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler ValueChanged;

        // Events

        private void OnUpClicked(object sender, EventArgs e) {
            ChangeValue(1);
        }

        private void OnDownClicked(object sender, EventArgs e) {
            ChangeValue(-1);
        }

        // Private Functions

        private void ChangeValue(int direction) {
            int amount = 1;
            bool shiftDown = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
            bool controlDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (shiftDown && controlDown) {
                amount = 100;
            }
            else if (controlDown) {
                amount = 10;
            }
            else if (shiftDown) {
                amount = 5;
            }

            Value += amount * direction;
        }
    }
}
