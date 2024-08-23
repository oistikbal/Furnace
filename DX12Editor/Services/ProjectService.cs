using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DX12Editor.Models;

namespace DX12Editor.Services
{
    public class ProjectService
    {
        private Project _project;

        public ProjectService(string path) 
        {
            if (!string.IsNullOrEmpty(path))
            {
                Debug.WriteLine(path);
                _project = Serializers.Serializer.FromFile<Project>(path);
            }
            else 
            {
                throw new ArgumentNullException(nameof(path));
            }
        }
    }
}
