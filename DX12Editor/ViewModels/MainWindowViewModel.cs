using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reflection;
using System.Xml;
using AvalonDock;
using AvalonDock.Layout.Serialization;
using DX12Editor.Services;
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
        public ReactiveCommand<Unit, Unit> SaveLayout { get; private set; }
        public ReactiveCommand<Unit, Unit> LoadLayout { get; private set; }

        private WindowsService _windowsService;
        private IServiceProvider _serviceProvider;
        private DockingManager _dockingManager;

        public string SceneName { get => "SampleScene"; }
        public string ProjectName { get => "NewProject"; }

        public void SetDockingManager(DockingManager dockingManager)
        {
            _dockingManager = dockingManager;
            _windowsService = new WindowsService(_serviceProvider, dockingManager);
            SaveLayout = ReactiveCommand.Create(_windowsService.Save);
            LoadLayout = ReactiveCommand.Create(_windowsService.Load);
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
            _windowsService.ShowFloatingWindow(windowItem.Type);
        }
    }
}
