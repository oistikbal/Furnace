using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DX12Editor.Models;
using ReactiveUI;
using Avalonia.Media.Imaging;

namespace DX12Editor.ViewModels
{
    public class NewProjectViewModel : ViewModelBase
    {
        private string _projectName = "New Project";
        private readonly string _templatePath = @"..\..\DX12Editor\ProjectTemplates";
        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        private ProjectTemplate _selectedProjectTemplate;

        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = this.RaiseAndSetIfChanged(ref _projectName, value); }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\DX12Engine";

        public string ProjectPath
        {
            get { return _projectPath; }
            set { _projectPath = this.RaiseAndSetIfChanged(ref _projectPath, value); }
        }

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        public ProjectTemplate SelectedProjectTemplate
        {
            get => _selectedProjectTemplate;
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedProjectTemplate, value);
            }
        }

        public NewProjectViewModel()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", searchOption: SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var templateFile in templateFiles) 
                {
                    var template = Serializers.Serializer.FromFile<ProjectTemplate>(templateFile);

                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), "icon.png"));
                    template.Icon = new Bitmap(template.IconFilePath);

                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), "screenshot.png"));
                    template.Screenshot = new Bitmap(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(templateFile), template.ProjectFile));
                    _projectTemplates.Add(template);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
