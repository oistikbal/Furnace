using System.Reflection;
using System.Windows.Controls;
using FurnaceEditor.Attributes;
using FurnaceEditor.ViewModels.Components;
using ReactiveUI;

namespace FurnaceEditor.ViewModels.Windows
{
    public class InspectorWindowViewModel : ViewModelBase
    {
        private readonly Dictionary<Type, UserControl> _views = new();
        private UserControl _content;

        public UserControl Content
        {
            get => _content;
            set => this.RaiseAndSetIfChanged(ref _content, value);
        }


        public InspectorWindowViewModel()
        {
            RegisterViews();
            MessageBus.Current.Listen<Entity>("SelectedEntity").Subscribe(e =>
            {
                if (e is not null)
                {
                    Content = _views[e.GetType()];
                    _views[e.GetType()].DataContext = e;
                }
                else
                {
                    Content = null;
                }
            });
        }

        private void RegisterViews()
        {
            var views = Assembly.GetCallingAssembly().GetTypes()
                        .Where(t => t.GetCustomAttributes(typeof(InspectorAttribute<>)).Any());

            foreach (var view in views)
            {
                var attribute = view.GetCustomAttributes(false)
                                    .FirstOrDefault(a => a.GetType().IsGenericType &&
                                                         a.GetType().GetGenericTypeDefinition() == typeof(InspectorAttribute<>));

                var viewType = ((dynamic)attribute).ContentType;

                var viewInstance = Activator.CreateInstance(viewType) as UserControl;
                _views[view] = viewInstance;
            }
        }

    }
}
