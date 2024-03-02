using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DX12Editor.ViewModels;
using System.Diagnostics;


namespace DX12Editor.Views
{
    public partial class ProjectBrowserDialog : Window
    {
        private ProjectBrowserDialogViewModel _viewModel;
        private NewProject _newProject;
        private OpenProject _openProject;

        public ProjectBrowserDialog()
        {
            InitializeComponent();
            _viewModel = new ProjectBrowserDialogViewModel();
            _newProject = new NewProject();
            _openProject = new OpenProject();
            DataContext = _viewModel;
        }

        private void OnProjectToggleButtonClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender == openProjectButton)
            {
                _viewModel.BorderContent = _openProject;
            }
            else if (sender == createProjectButton)
            {
                _viewModel.BorderContent = _newProject;
            }

        }
    }
}