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

namespace FurnaceEditor.Views.Inspector
{
    /// <summary>
    /// Interaction logic for EntityView.xaml
    /// </summary>
    public partial class EntityView : UserControl
    {
        public EntityView()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (binding != null)
                    {
                        binding.UpdateSource();
                    }
                }
            }
            else if (e.Key == Key.Escape)
            {
                TextBox textBox = sender as TextBox;
                if (textBox != null)
                {
                    BindingExpression binding = textBox.GetBindingExpression(TextBox.TextProperty);
                    if (binding != null)
                    {
                        binding.UpdateTarget();
                    }
                    Keyboard.ClearFocus();
                }
            }
        }
    }
}
