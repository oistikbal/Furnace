using System.Reactive;
using System.Windows.Controls;
using System.Windows.Forms;
using DX12Editor.Models;
using DX12Editor.Services;
using DX12Editor.Views.Project;
using ReactiveUI;

namespace DX12Editor.ViewModels
{
    public class ProjectDialogViewModel : ViewModelBase
    {
        private System.Windows.Controls.UserControl? _borderContent;
        private readonly CreateProject _createProject;
        private readonly RecentProjects _recentProjects;

        public ReactiveCommand<Unit, Unit> ProjectOpenButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectCreateButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectBackButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectLocationButton { get; }

        public ProjectDialogViewModel()
        {
            ProjectOpenButton = ReactiveCommand.Create(OnProjectOpenButton);
            ProjectCreateButton = ReactiveCommand.Create(OnProjectCreateButton);
            ProjectBackButton = ReactiveCommand.Create(OnProjectBackButton);
            ProjectLocationButton = ReactiveCommand.Create(OnProjectLocationButton);

            _createProject = new CreateProject();
            _recentProjects = new RecentProjects();
            BorderContent = _recentProjects;
        }

        public System.Windows.Controls.UserControl BorderContent
        {
            get => _borderContent;
            set => this.RaiseAndSetIfChanged(ref _borderContent, value);
        }

        private void OnProjectCreateButton()
        {
            BorderContent = _createProject;
        }

        private void OnProjectOpenButton()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a project",
                Filter = $"{Project.Extenion} (*{Project.Extenion})|*{Project.Extenion}"
            };

            switch (openFileDialog.ShowDialog()) 
            {
                case DialogResult.OK:
                    break;
                default:
                    break;
            }
        }

        private void OnProjectLocationButton() 
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.ShowNewFolderButton = true; // Optionally allow users to create new folders

                if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //FolderPathTextBox.Text = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void OnProjectBackButton()
        {
            BorderContent = _recentProjects;
        }

    }
}
