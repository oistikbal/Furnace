using System.Windows;
using System.Windows.Controls;
using FurnaceEditor.Attributes;
using FurnaceEditor.ViewModels.Windows;

namespace FurnaceEditor.Views.Windows
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
