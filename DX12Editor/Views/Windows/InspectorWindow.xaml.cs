using System.Windows;
using System.Windows.Controls;
using DX12Editor.Attributes;
using DX12Editor.ViewModels.Windows;

namespace DX12Editor.Views.Windows
{
    /// <summary>
    /// Interaction logic for InspectorWindow.xaml
    /// </summary>
    /// 
    [Window("Inspector", typeof(InspectorWindowViewModel))]
    public partial class InspectorWindow : UserControl
    {
        public InspectorWindow()
        {
            InitializeComponent();
        }
    }
}
