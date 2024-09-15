using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace FurnaceEditor.Controls
{
    public partial class FloatBox : UserControl
    {
        private bool _isDragging = false;
        private Point _lastMousePosition;
        private static readonly Regex _regex = new Regex("[^0-9]+");
        private float _multiplier;

        public FloatBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(FloatBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(float), typeof(FloatBox), new PropertyMetadata(0.0f));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            _isDragging = true;
            _lastMousePosition = e.GetPosition(this);
            ((Border)sender).CaptureMouse();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = false;
            if (_isDragging)
            {
                Point currentPosition = e.GetPosition(this);
                double deltaX = currentPosition.X - _lastMousePosition.X;


                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control)) _multiplier = 1.0f;
                else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift)) _multiplier = 0.01f;
                else _multiplier = 0.1f;

                Value += (float)deltaX * _multiplier;

                // Update the last mouse position
                _lastMousePosition = currentPosition;
            }
        }

        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            _isDragging = false;
            ((Border)sender).ReleaseMouseCapture();
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            ((TextBox)sender).Focus();
        }


        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _regex.IsMatch(e.Text);
        }
    }

}