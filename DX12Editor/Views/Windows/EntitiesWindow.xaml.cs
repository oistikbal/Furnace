using System.Windows.Controls;
using DX12Editor.Attributes;
using DX12Editor.ViewModels.Windows;

namespace DX12Editor.Views.Windows
{
    [Window("Entities", typeof(EntitiesWindowViewModel))]
    public partial class EntitiesWindow : UserControl
    {
        public EntitiesWindow()
        {
            InitializeComponent();
        }
    }
}
