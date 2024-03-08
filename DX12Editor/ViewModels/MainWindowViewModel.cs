using DX12Editor.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace DX12Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private IServiceProvider _serviceProvider;
        private ProjectBrowserDialog _dialog;

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _dialog = new ProjectBrowserDialog();
        }

        public void Show()
        {
            _dialog.Show();
        }
    }
}
