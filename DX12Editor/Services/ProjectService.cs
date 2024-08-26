using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DX12Editor.Models;
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
            _project = Serializers.Serializer.FromFile<Project>(path);
            _messageBus.SendMessage(_project, "ProjectLoaded");
        }

        public Project GetProject()
        {
            return _project;
        }
    }
}
