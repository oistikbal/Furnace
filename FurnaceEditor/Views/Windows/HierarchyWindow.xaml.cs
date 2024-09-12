using System.Windows;
using System.Windows.Controls;
using FurnaceEditor.Attributes;
using FurnaceEditor.ViewModels.Windows;

namespace FurnaceEditor.Views.Windows
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
