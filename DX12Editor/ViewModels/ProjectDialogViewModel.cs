using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using DX12Editor.Models;
using DX12Editor.Views.Project;
using ReactiveUI;

namespace DX12Editor.ViewModels
{
    public class ProjectDialogViewModel : ViewModelBase
    {
        private System.Windows.Controls.UserControl? _borderContent;
        private readonly CreateProject _createProject;
        private readonly RecentProjects _recentProjects;

        private string _location;
        private string _projectName;
        private string _projectLocationMessage;

        public string ProjectName
        {
            get => _projectName;
            set
            {
                this.RaiseAndSetIfChanged(ref _projectName, value);
                UpdateProjectLocation();
            }
        }

        public string Location
        {
            get => _location;
            set
            {
                this.RaiseAndSetIfChanged(ref _location, value);
                UpdateProjectLocation();
            }
        }

        public string ProjectLocationMessage
        {
            get => _projectLocationMessage;
            set => this.RaiseAndSetIfChanged(ref _projectLocationMessage, value);
        }

        public bool IsNextButtonEnabled => IsValidFileName(ProjectName) && IsValidPath(Location);

        public ReactiveCommand<Unit, Unit> ProjectOpenButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectCreateButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectBackButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectLocationButton { get; }
        public ReactiveCommand<Unit, Unit> ProjectCreateNextButton { get; }

        public ProjectDialogViewModel()
        {
            ProjectOpenButton = ReactiveCommand.Create(OnProjectOpenButton);
            ProjectCreateButton = ReactiveCommand.Create(OnProjectCreateButton);
            ProjectBackButton = ReactiveCommand.Create(OnProjectBackButton);
            ProjectLocationButton = ReactiveCommand.Create(OnProjectLocationButton);
            ProjectCreateNextButton = ReactiveCommand.Create(OnProjectCreateNextButton);

            _createProject = new CreateProject();
            _recentProjects = new RecentProjects();
            BorderContent = _recentProjects;

            this.WhenAnyValue(x => x.ProjectName, x => x.Location)
                .Subscribe(_ => UpdateIsNextButtonEnabled());

            UpdateProjectLocation();
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
                    Location = folderBrowserDialog.SelectedPath;
                }
            }
        }

        private void OnProjectBackButton()
        {
            BorderContent = _recentProjects;
        }

        private void OnProjectCreateNextButton()
        {
            Project.CreateProject(Location, ProjectName);
        }

        private bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            // Trim leading and trailing spaces
            fileName = fileName.Trim();

            return !fileName.Contains(" ")
                   && !fileName.Any(ch => Path.GetInvalidFileNameChars().Contains(ch));
        }

        private bool IsValidPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(path) && Directory.Exists(path);
        }

        private void UpdateIsNextButtonEnabled()
        {
            this.RaisePropertyChanged(nameof(IsNextButtonEnabled));
        }

        private void UpdateProjectLocation()
        {
            // Validate the inputs
            if (!IsValidPath(Location) || !IsValidFileName(ProjectName))
            {
                // Handle invalid inputs, perhaps log a message or show an error to the user
                ProjectLocationMessage = "Invalid location or project name.";
                return;
            }

            try
            {
                // Combine path and project name
                var combinedPath = System.IO.Path.Combine(Location, ProjectName);
                ProjectLocationMessage = $"Project will be created in: {combinedPath}";
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                ProjectLocationMessage = $"Error: {ex.Message}";
            }
        }
    }
}
