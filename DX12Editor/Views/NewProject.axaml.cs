using Avalonia.Controls;
using DX12Editor.ViewModels;


namespace DX12Editor.Views
{

    public partial class NewProject : UserControl
    {
        public NewProject()
        {
            InitializeComponent();
            DataContext = new NewProjectViewModel();
        }
    }

}