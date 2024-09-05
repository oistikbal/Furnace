using System.Collections.ObjectModel;
using DX12Editor.Attributes;
using DX12Editor.Views.Inspector;
using ReactiveUI;

namespace DX12Editor.ViewModels.Components
{
    [Inspector<EntityView>]
    public class Entity : ViewModelBase
    {
        private string _name;
        private readonly ObservableCollection<Component> _components = new();
        private static int count = 0;

        public ReadOnlyObservableCollection<Component> Components { get; private set; }
        public string Name
        {
            get
            {
                return _name;
            }
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public Entity()
        {
            Name = $"Game Object ({count})";
            _components.Add(new Transform());
            Components = new(_components);
            count++;
        }
    }
}
