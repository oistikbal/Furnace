using System.Windows;
using System.Windows.Controls;
using FurnaceEditor.Attributes;
using FurnaceEditor.ViewModels.Windows;

namespace FurnaceEditor.Views.Windows
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
