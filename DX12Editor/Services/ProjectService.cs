using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using DX12Editor.Models;
using DX12Editor.Utilities.Serializers;
using ReactiveUI;

namespace DX12Editor.Services
{
    public class ProjectService
    {
        private readonly IMessageBus _messageBus;
        private Project _project;

        public ProjectService(string path, IMessageBus messageBus) 
        {
            Debug.Assert(path != null, "Path cannot be null");
            Debug.Assert(messageBus != null, "MessageBus Cannot be null");

            _messageBus = messageBus;
            _project = Serializer.FromFile<Project>(path);
            _project.Path = System.IO.Path.GetDirectoryName(path);
            InitializeProjectStructure();

            _messageBus.SendMessage(_project, "ProjectLoaded");
        }

        public Project GetProject()
        {
            return _project;
        }

        private void InitializeProjectStructure()
        {
            // Define the folders that should be present in the project
            string[] requiredFolders = new string[]
            {
            System.IO.Path.Combine(_project.Path, "Assets"),
            System.IO.Path.Combine(_project.Path, "Library"),
            System.IO.Path.Combine(_project.Path, "Logs"),
                // Add more folders as needed
            };

            // Check if each folder exists, and create it if it doesn't
            foreach (string folder in requiredFolders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
            }
        }
    }
}
