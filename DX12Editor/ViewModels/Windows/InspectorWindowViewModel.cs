using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using DX12Editor.ViewModels.Components;
using DX12Editor.Views.Inspector;
using ReactiveUI;

namespace DX12Editor.ViewModels.Windows
{
    public class InspectorWindowViewModel : ViewModelBase
    {
        private UserControl _content;
        private UserControl _entityView;

        public UserControl Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }


        public InspectorWindowViewModel() 
        {
            _entityView = new EntityView();
            Content = _entityView;
            MessageBus.Current.Listen<Entity>("SelectedEntity").Subscribe(e => {_entityView.DataContext = e; });
        }

    }
}
