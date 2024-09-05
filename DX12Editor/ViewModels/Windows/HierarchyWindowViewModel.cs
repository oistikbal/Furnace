using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Windows.Controls;
using DX12Editor.ViewModels.Components;
using ReactiveUI;

namespace DX12Editor.ViewModels.Windows
{
    public class HierarchyWindowViewModel : ViewModelBase
    {
        private readonly ObservableCollection<Entity> _entities = new();
        public ReadOnlyObservableCollection<Entity> Entities { get; private set; }

        #region Commands
        public ReactiveCommand<Unit, Unit> AddEntityCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> RemoveEntityCommand { get; private set; }
        public ReactiveCommand<(Entity, StackPanel), Unit> RightClickEntityCommand { get; private set; }
        public ReactiveCommand<Entity, Unit> SelectedItemCommand { get; private set; }
        #endregion

        private Entity? _rightClickEntity;

        public HierarchyWindowViewModel()
        {
            AddEntityCommand = ReactiveCommand.Create(AddEntity);
            RightClickEntityCommand = ReactiveCommand.Create<(Entity, StackPanel)>(o => RightClickEntity(o.Item1, o.Item2));
            RemoveEntityCommand = ReactiveCommand.Create(RemoveEntity);
            SelectedItemCommand = ReactiveCommand.Create<Entity>(entity => MessageBus.Current.SendMessage<Entity>(entity, "SelectedEntity"));
            Entities = new(_entities);
        }

        private void AddEntity()
        {
            _entities.Add(new Entity());
        }

        private void RightClickEntity(Entity entity, StackPanel stackPanel)
        {
            Debug.WriteLine(entity.Name);
            var contextMenu = ((StackPanel)stackPanel).ContextMenu.DataContext = this;
            _rightClickEntity = entity;
        }

        private void RemoveEntity()
        {
            Debug.Assert(_rightClickEntity is not null);
            if (_rightClickEntity is not null)
            {
                _entities.Remove(_rightClickEntity);
            }
        }
    }
}
