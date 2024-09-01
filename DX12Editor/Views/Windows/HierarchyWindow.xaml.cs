using System.Windows.Controls;
using DX12Editor.Attributes;
using DX12Editor.ViewModels.Windows;

namespace DX12Editor.Views.Windows
{
    [Window("Hierarchy", typeof(HierarchyWindowViewModel))]
    public partial class HierarchyWindow : UserControl
    {
        public HierarchyWindow()
        {
            InitializeComponent();
        }
    }
}
