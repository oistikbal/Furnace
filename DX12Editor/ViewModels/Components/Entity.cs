using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using DX12Editor.Attributes;
using DX12Editor.Views.Inspector;
using ReactiveUI;

namespace DX12Editor.ViewModels.Components
{
    [DataContract]
    [Inspector<EntityView>]
    public class Entity : ViewModelBase
    {


        private bool _isEnabled;
        private string _name;
        private static int count = 0;

        private readonly ObservableCollection<Component> _components = new();

        public bool IsEnabled
        {
            get => _isEnabled;
            set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
        }

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
            IsEnabled = true;
            count++;
        }
    }
}
