using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FurnaceEditor.Controls
{
    public partial class VectorBox : UserControl
    {
        private bool _isDragging = false;
        private Point _lastMousePosition;
        private static readonly Regex _regex = new Regex("[^0-9]+");

        public VectorBox()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(VectorBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(
        "X", typeof(float), typeof(VectorBox), new PropertyMetadata(0.0f));

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(
        "Y", typeof(float), typeof(VectorBox), new PropertyMetadata(0.0f));

        public static readonly DependencyProperty ZProperty = DependencyProperty.Register(
        "Z", typeof(float), typeof(VectorBox), new PropertyMetadata(0.0f));

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public float X
        {
            get { return (float)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        public float Y
        {
            get { return (float)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        public float Z
        {
            get { return (float)GetValue(ZProperty); }
            set { SetValue(ZProperty, value); }
        }
    }
}
