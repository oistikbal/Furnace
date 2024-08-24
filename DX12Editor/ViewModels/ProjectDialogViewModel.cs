using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Forms;
using System.Xml.Linq;
using DX12Editor.Models;
using DX12Editor.Views.Project;
using ReactiveUI;

namespace DX12Editor.ViewModels
{
    public class ProjectDialogViewModel : ViewModelBase
    {
        private System.Windows.Controls.UserControl? _borderContent;
        private readonly CreateProject _createProject;
        private readonly Views.Project.RecentProjects _recentProjects;

        private string _location;
        private string _projectName;
        private string _projectLocationMessage;
        private RecentProject _selectedRecentProject;
        private ObservableCollection<RecentProject> _recentProjectList;

        public RecentProject SelectedRecentProject
        {
            get => _selectedRecentProject;
            set
            {
                OnProjectSelected(value);  // Handle the item selection
                this.RaiseAndSetIfChanged(ref _selectedRecentProject, value);            
            }
        }

        public ObservableCollection<RecentProject> RecentProjects
        {
            get => _recentProjectList;
            set => this.RaiseAndSetIfChanged(ref _recentProjectList, value);
        }

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

        public event Action<string> ProjectOpen;

        public ProjectDialogViewModel()
        {
            ProjectOpenButton = ReactiveCommand.Create(OnProjectOpenButton);
            ProjectCreateButton = ReactiveCommand.Create(OnProjectCreateButton);
            ProjectBackButton = ReactiveCommand.Create(OnProjectBackButton);
            ProjectLocationButton = ReactiveCommand.Create(OnProjectLocationButton);
            ProjectCreateNextButton = ReactiveCommand.Create(OnProjectCreateNextButton);

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RecentProjects.xml");

            if (!File.Exists(filePath))
            {
                RecentProjects = new ObservableCollection<RecentProject>();
                Serializers.Serializer.ToFile<Models.RecentProjects>(new Models.RecentProjects(), filePath);
            }
            else
            {
                // If the file exists, read the data from the file
                RecentProjects = Serializers.Serializer.FromFile<ObservableCollection<RecentProject>>(filePath);
            }

            _createProject = new();
            _recentProjects = new();
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
                    AddOrUpdateRecentProject(openFileDialog.FileName);
                    ProjectOpen?.Invoke(openFileDialog.FileName);
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
            AddOrUpdateRecentProject($"{Path.Combine(Path.Combine(Location, ProjectName), $"{ProjectName}{Project.Extenion}")}");
            ProjectOpen?.Invoke($"{Path.Combine(Path.Combine(Location, ProjectName), $"{ProjectName}{Project.Extenion}")}");
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

        private void AddOrUpdateRecentProject(string projectPath)
        {
            var existingProject = RecentProjects.FirstOrDefault(p => p.Path == projectPath);

            if (existingProject != null)
            {
                // Update the timestamp if the project already exists
                existingProject.LastOpened = DateTime.Now;
            }
            else
            {
                // Add a new project if it doesn't exist
                var newProject = new RecentProject
                {
                    Name = Path.GetFileNameWithoutExtension(projectPath),
                    Path = projectPath,
                    LastOpened = DateTime.Now
                };

                RecentProjects.Add(newProject);
            }

            // Save the updated list to the XML file
            SaveRecentProjects();
        }

        private void SaveRecentProjects()
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RecentProjects.xml");
            Serializers.Serializer.ToFile(RecentProjects, filePath);
        }

        private void OnProjectSelected(RecentProject project)
        {
            Debug.WriteLine(project.Path);
            ProjectOpen?.Invoke(project.Path);
        }
    }
}
