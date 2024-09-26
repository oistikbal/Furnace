using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FurnaceEditor.Controls
{
    [TemplatePart(Name = "_border", Type = typeof(Border))]
    [TemplatePart(Name = "_textBox", Type = typeof(TextBox))]
    public partial class IntControl : Control
    {
        private bool _isDragging = false;
        private Point _lastMousePosition;
        private static readonly Regex _regex = new Regex("[^0-9]+");

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(IntControl), new PropertyMetadata(0));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("_border") is Border border)
            {
                ;
                border.PreviewMouseLeftButtonDown += Border_MouseLeftButtonDown;
                border.MouseLeftButtonUp += Border_MouseLeftButtonUp;
                border.MouseMove += Border_MouseMove;
            }

            if (GetTemplateChild("_textBox") is TextBox textBox)
            {
                textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
            }
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

                Value += (int)deltaX;

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
