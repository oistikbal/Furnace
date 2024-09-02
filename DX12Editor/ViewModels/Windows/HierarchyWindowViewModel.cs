using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Controls;
using DX12Editor.ViewModels;
using DX12Editor.ViewModels.Components;
using ReactiveUI;

namespace DX12Editor.ViewModels.Windows
{
    public class HierarchyWindowViewModel : ViewModelBase
    {

        private object _selectedEntity;

        public object SelectedEntity
        {
            get => _selectedEntity;
            set
            {
                Debug.WriteLine("selection");
                this.RaiseAndSetIfChanged(ref _selectedEntity, value);
            }
        }

        private readonly ObservableCollection<Entity> _entities = new();
        public ReadOnlyObservableCollection<Entity> Entities { get; private set; }

        #region Commands
        public ReactiveCommand<Unit, Unit> AddEntityCommand { get; private set; }
        public ReactiveCommand<Entity, Unit> RemoveEntityCommand { get; private set; }
        public ReactiveCommand<object, Unit> RightClickCommand { get; private set; }
        public ReactiveCommand<Entity, Unit> SelectedItemCommand { get; private set; }
        #endregion

        public HierarchyWindowViewModel() 
        {
            AddEntityCommand = ReactiveCommand.Create(AddEntity);
            RightClickCommand = ReactiveCommand.Create<object>(obj => RightClick(obj));
            RemoveEntityCommand = ReactiveCommand.Create<Entity>(entity=> RemoveEntity(entity));
            SelectedItemCommand = ReactiveCommand.Create<Entity>(entity=> MessageBus.Current.SendMessage<Entity>(entity, "SelectedEntity"));
            Entities = new(_entities);
        }

        private void AddEntity()
        {
            _entities.Add(new Entity());
        }

        private void RightClick(object stackPanel)
        {
            
            if (stackPanel is StackPanel)
            {
                var contextMenu = ((StackPanel)stackPanel).ContextMenu;
                contextMenu.DataContext = this;
            }
        }

        private void RemoveEntity(Entity entity) 
        {
            _entities.Remove(entity);
        }
    }
}
