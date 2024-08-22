using System.Windows;
using System.Windows.Controls;
using DX12Editor.Attributes;
using DX12Editor.ViewModels.Windows;

namespace DX12Editor.Views.Windows
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
