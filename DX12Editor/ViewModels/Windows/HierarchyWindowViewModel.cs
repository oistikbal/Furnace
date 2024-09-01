using System.Collections.ObjectModel;

namespace DX12Editor.ViewModels.Windows
{
    public class Scene
    {
        public string SceneName { get; set; }
        public ObservableCollection<Entity> Entities { get; set; }
    }

    public class Entity
    {
        public string Name { get; set; }
    }

    public class HierarchyWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Scene> Scenes { get; set; }

        public HierarchyWindowViewModel()
        {
            Scenes = new ObservableCollection<Scene>
            {
                new Scene
                {
                    SceneName = "Scene 1",
                    Entities = new ObservableCollection<Entity>
                    {
                        new Entity { Name = "Entity 1" },
                        new Entity { Name = "Entity 2" }
                    }
                },
                new Scene
                {
                    SceneName = "Scene 2",
                    Entities = new ObservableCollection<Entity>
                    {
                        new Entity { Name = "Entity 3" }
                    }
                },
                new Scene
                {
                    SceneName = "Scene 3",
                    Entities = new ObservableCollection<Entity>()
                }
            };
        }
    }
}
