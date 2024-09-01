using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DX12Editor.Models;
using DX12Editor.Utilities.Serializers;

namespace DX12Editor.Services
{
    public class SceneService
    {
        private string _lastSceneFilePath;
        private Scene _activeScene;
        private string _activeScenePath;
        private readonly string _projectPath;

        public SceneService(string projectPath)
        {
            _lastSceneFilePath = Path.Combine(Path.Combine(Path.GetDirectoryName(projectPath), "Library"), "LastScene.txt");
            _projectPath = projectPath;
            

            if (!File.Exists(_lastSceneFilePath))
            {
                _activeScene = new Scene();
            }
            else
            {
                _activeScenePath = File.ReadAllText(_lastSceneFilePath);
                _activeScene = Serializer.FromFile<Scene>(_activeScenePath);
            }
        }

        public void OpenScene(string path)
        {
            _activeScenePath = File.ReadAllText(_lastSceneFilePath);
            _activeScene = Serializer.FromFile<Scene>(_activeScenePath);
        }

        public void SaveScene()
        {

        }
    }
}
