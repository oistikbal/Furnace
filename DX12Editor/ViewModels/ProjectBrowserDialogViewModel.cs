using Avalonia.Controls;
using DX12Editor.Views;
using ReactiveUI;
using System.Globalization;

namespace DX12Editor.ViewModels
{
    public class ProjectBrowserDialogViewModel : ViewModelBase
    {
        private UserControl _borderContent;

        public UserControl BorderContent
        {
            get => _borderContent;
            set => this.RaiseAndSetIfChanged(ref _borderContent, value);
        }
    }
}
