using System.Reflection;
using System.Windows.Controls;
using AvalonDock;
using AvalonDock.Layout;
using DX12Editor.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace DX12Editor.Views.Windows
{
    public class WindowManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DockingManager _dockingManager;
        private readonly Dictionary<ViewModelBase, LayoutAnchorable> _openWindows = new();

        public WindowManager(IServiceProvider serviceProvider, DockingManager dockingManager)
        {
            _serviceProvider = serviceProvider;
            _dockingManager = dockingManager;
        }

        public void ShowFloatingWindow(Type windowType, double width = 400, double height = 300)
        {
            if (!typeof(UserControl).IsAssignableFrom(windowType))
            {
                throw new ArgumentException($"The type {windowType.Name} must be a UserControl.", nameof(windowType));
            }

            var attribute = windowType.GetCustomAttribute<WindowAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException($"The window type {windowType.Name} does not have a {nameof(WindowAttribute)}.");
            }

            var viewModelType = attribute.ViewModelType;
            var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);

            if (_openWindows.TryGetValue(viewModel, out var existingAnchorable))
            {
                if (existingAnchorable.IsVisible)
                {
                    existingAnchorable.IsActive = true;
                    return;
                }
            }


            var userControl = (UserControl)_serviceProvider.GetRequiredService(windowType);
            userControl.DataContext = viewModel;

            // Create a new LayoutAnchorable and add it to a floating window
            var anchorable = new LayoutAnchorable
            {
                Content = userControl,
                Title = attribute.Name, // Assuming your ViewModel has a DisplayName property
                FloatingWidth = 400,
                FloatingHeight = 300,
            };

            var floatingWindow = _dockingManager.CreateFloatingWindow(anchorable, false);

            // Track the window in the dictionary
            _openWindows[viewModel] = anchorable;

            // Show the floating window
            anchorable.Float();
            anchorable.IsActive = true;

            // Handle the closing event
            anchorable.Closed += (s, e) => _openWindows.Remove(viewModel);
        }
    }
}
