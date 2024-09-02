using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace DX12Editor.ViewModels.Components
{
    public class Entity : ViewModelBase
    {
        private string _name;
        private readonly ObservableCollection<Component> _components = new();
        private static int count = 0;
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
            count++;
        }
    }
}
