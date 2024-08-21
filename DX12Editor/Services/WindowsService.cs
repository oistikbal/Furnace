using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Controls;
using System.Xml;
using AvalonDock;
using AvalonDock.Layout;
using AvalonDock.Layout.Serialization;
using DX12Editor.ViewModels;
using DX12Editor.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace DX12Editor.Services
{
    public class WindowsService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DockingManager _dockingManager;
        private readonly Dictionary<Type, LayoutAnchorable> _openWindows = new();

        public WindowsService(IServiceProvider serviceProvider, DockingManager dockingManager)
        {
            _serviceProvider = serviceProvider;
            _dockingManager = dockingManager;
        }

        public void ShowFloatingWindow(Type windowType)
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

            if (_openWindows.TryGetValue(windowType, out var existingAnchorable))
            {
                if (existingAnchorable.IsVisible)
                {
                    existingAnchorable.IsActive = true;
                    return;
                }
            }


            var viewModelType = attribute.ViewModelType;
            var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);
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
            _openWindows[windowType] = anchorable;

            // Show the floating window
            anchorable.Float();
            anchorable.IsActive = true;

            // Handle the closing event
            anchorable.Closed += (s, e) => _openWindows.Remove(windowType);
        }

        private void UpdateContentId(ILayoutContainer layoutContainer)
        {
            // Traverse the layout container recursively
            foreach (var child in layoutContainer.Children)
            {
                switch (child)
                {
                    case LayoutAnchorablePane layoutAnchorablePane:
                        foreach (var anchorable in layoutAnchorablePane.Children)
                        {
                            if (anchorable.Content is UserControl userControl)
                            {
                                anchorable.ContentId = userControl.GetType().AssemblyQualifiedName;
                            }
                        }
                        break;

                    case LayoutDocumentPane layoutDocumentPane:
                        foreach (var document in layoutDocumentPane.Children)
                        {
                            if (document.Content is UserControl userControl)
                            {
                                document.ContentId = userControl.GetType().AssemblyQualifiedName;
                            }
                        }
                        break;

                    case ILayoutContainer subContainer:
                        UpdateContentId(subContainer); // Recursively update sub-containers
                        break;
                }
            }
        }

        public void Save()
        {
            UpdateContentId(_dockingManager.Layout);
            var sl = new XmlLayoutSerializer(_dockingManager);
            using var fs = XmlWriter.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Default.xml"), new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "\t"
            });
            sl.Serialize(fs);
        }

        public void Load()
        {
            var sl = new XmlLayoutSerializer(_dockingManager);
            sl.LayoutSerializationCallback += LayoutSerializationCallback;
            sl.Deserialize(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Default.xml"));
            _dockingManager.UpdateLayout();
        }

        private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            var contentId = e.Model.ContentId;

            // Attempt to resolve the window type from ContentId
            var windowType = Type.GetType(contentId);
            Debug.WriteLine(windowType);
            if (windowType == null || !typeof(UserControl).IsAssignableFrom(windowType))
            {
                Debug.WriteLine($"Unable to restore window for ContentId: {contentId}");
                return;
            }

            var attribute = windowType.GetCustomAttribute<WindowAttribute>();
            if (attribute == null)
            {
                Debug.WriteLine($"Missing {nameof(WindowAttribute)} for window type: {windowType.Name}");
                return;
            }

            // Check if the window is already open
            if (_openWindows.TryGetValue(windowType, out var existingAnchorable))
            {
                e.Content = existingAnchorable.Content;
                return;
            }

            // Resolve the ViewModel
            var viewModelType = attribute.ViewModelType;
            var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(viewModelType);

            // Create a new UserControl and set the DataContext
            var userControl = (UserControl)_serviceProvider.GetRequiredService(windowType);
            userControl.DataContext = viewModel;

            var anchorable = new LayoutAnchorable
            {
                Content = userControl,
                Title = attribute.Name,
                FloatingWidth = 400,
                FloatingHeight = 300,
                ContentId = windowType.FullName // Ensure ContentId is the fully qualified type name
            };

            // Track the window in the dictionary
            _openWindows[windowType] = anchorable;

            e.Content = userControl;
        }
    }
}
