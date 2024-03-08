using Avalonia.Controls;
using DX12Editor.Views;
using ReactiveUI;
using System.Reactive;

namespace DX12Editor.ViewModels
{
    public class ProjectBrowserDialogViewModel : ViewModelBase
    {
        private UserControl? _borderContent;
        private NewProject _newProject;
        private OpenProject _openProject;

        public ReactiveCommand<Unit, Unit> ProjectOpenButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectCreateButton { get; }

        public ProjectBrowserDialogViewModel()
        {
            ProjectOpenButton = ReactiveCommand.Create(OnProjectOpenButton);
            ProjectCreateButton = ReactiveCommand.Create(OnProjectCreateButton);

            _newProject = new NewProject();
            _openProject = new OpenProject();
            BorderContent = _openProject;
        }

        public UserControl BorderContent
        {
            get => _borderContent;
            set => this.RaiseAndSetIfChanged(ref _borderContent, value);
        }

        private void OnProjectCreateButton()
        {
            BorderContent = _newProject;
        }

        private void OnProjectOpenButton()
        {
            BorderContent = _openProject;
        }
    }
}
