using System.Collections.ObjectModel;

namespace DX12Editor.ViewModels.Windows
{
    public class Entity
    {
        public string Name { get; set; }
    }

    public class EntitiesWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Entity> Entities { get; set; }

        public EntitiesWindowViewModel()
        {
            Entities = new ObservableCollection<Entity>
            {
                new Entity { Name = "Entity 1" },
                new Entity { Name = "Entity 2" },
                new Entity { Name = "Entity 3" }
            };
        }
    }
}
