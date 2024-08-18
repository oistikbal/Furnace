using System.Windows;
using System.Windows.Controls;
using DX12Editor.ViewModels.Windows;

namespace DX12Editor.Views.Windows
{
    [Window("Scene", typeof(SceneWindowViewModel))]
    public partial class SceneWindow : UserControl
    {
        public SceneWindow()
        {
            InitializeComponent();
        }
    }
}
