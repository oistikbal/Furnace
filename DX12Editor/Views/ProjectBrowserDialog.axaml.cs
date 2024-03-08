using Avalonia.Controls;
using DX12Editor.ViewModels;


namespace DX12Editor.Views
{
    public partial class ProjectBrowserDialog : Window
    {
        private ProjectBrowserDialogViewModel _viewModel;

        public ProjectBrowserDialog()
        {
            InitializeComponent();
            _viewModel = new ProjectBrowserDialogViewModel();
            DataContext = _viewModel;
        }
    }
}