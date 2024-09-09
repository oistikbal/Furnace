using System.Windows;
using System.Windows.Controls;
using FurnaceEditor.Attributes;
using FurnaceEditor.ViewModels.Windows;

namespace FurnaceEditor.Views.Windows
{
    [Window("Console", typeof(ConsoleWindowViewModel))]
    public partial class ConsoleWindow : UserControl
    {
        public ConsoleWindow()
        {
            InitializeComponent();
        }
    }
}
