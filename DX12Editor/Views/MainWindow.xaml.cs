using AakStudio.Shell.UI.Controls;
using DX12Editor.ViewModels;

namespace DX12Editor.Views
{
    public partial class MainWindow : CustomChromeWindow
    {
        public MainWindow(MainWindowViewModel viewModel, IServiceProvider serviceProvider)
        {
            InitializeComponent();
            viewModel.SetDockingManager(dockManager);
            DataContext = viewModel;
        }
    }
}