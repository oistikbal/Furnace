using System.Reactive;
using System.Reflection;
using AvalonDock;
using DX12Editor.Views.Windows;
using ReactiveUI;

namespace DX12Editor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public class WindowItem
        {
            public string Name { get; set; }
            public Type Type { get; set; }
        }

        public List<WindowItem> WindowItems { get; private set; }
        public ReactiveCommand<WindowItem, Unit> OpenWindowCommand { get; private set; }

        private WindowManager _windowManager;
        private IServiceProvider _serviceProvider;

        public void SetDockingManager(DockingManager dockingManager)
        {
            _windowManager = new WindowManager(_serviceProvider, dockingManager);
        }

        public MainWindowViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            WindowItems = new List<WindowItem>();
            OpenWindowCommand = ReactiveCommand.Create<WindowItem>(item => OpenWindow(item));

            LoadWindowItems();
        }

        private void LoadWindowItems()
        {
            var windowTypes = Assembly.GetExecutingAssembly()
                                      .GetTypes()
                                      .Where(t => t.GetCustomAttributes<WindowAttribute>().Any());

            foreach (var type in windowTypes)
            {
                var attribute = type.GetCustomAttribute<WindowAttribute>();
                if (attribute != null)
                {
                    WindowItems.Add(new WindowItem { Name = attribute.Name, Type = type });
                }
            }
        }

        private void OpenWindow(WindowItem windowItem)
        {
            _windowManager.ShowFloatingWindow(windowItem.Type);
        }
    }
}
