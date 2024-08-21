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
        private readonly Dictionary<Type, LayoutContent> _openWindows = new();
        private readonly Dictionary<string, string> _layoutDictionary = new();



        public WindowsService(IServiceProvider serviceProvider, DockingManager dockingManager)
        {
            _serviceProvider = serviceProvider;
            _dockingManager = dockingManager;
            LoadLayoutsFromResources();
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

            // Check if the window is already open
            if (_openWindows.TryGetValue(windowType, out var existingAnchorable))
            {
                existingAnchorable.IsActive = true;
                return;
            }

            var viewModel = (ViewModelBase)_serviceProvider.GetRequiredService(attribute.ViewModelType);
            var userControl = (UserControl)_serviceProvider.GetRequiredService(windowType);
            userControl.DataContext = viewModel;

            // Create a new LayoutAnchorable and add it to a floating window
            var anchorable = new LayoutAnchorable
            {
                Content = userControl,
                Title = attribute.Name,
                FloatingWidth = 400,
                FloatingHeight = 300,
            };

            var floatingWindow = _dockingManager.CreateFloatingWindow(anchorable, false);

            // Track the window in the dictionary
            _openWindows[windowType] = anchorable;

            // Show the floating window
            anchorable.Float();
            anchorable.IsActive = true;
            anchorable.CanClose = true;

            // Handle the closing event to update the dictionary
            anchorable.Closed += (s, e) =>
            {
                _openWindows.Remove(windowType);
            };
        }
        public void LoadLayout(string layoutName)
        {
            if (_layoutDictionary.TryGetValue(layoutName, out var resourceName))
            {
                var xmlReader = XmlReader.Create(new StringReader(_layoutDictionary[layoutName]));
                var serializer = new XmlLayoutSerializer(_dockingManager);
                serializer.LayoutSerializationCallback += LayoutSerializationCallback;
                serializer.Deserialize(xmlReader);
                UpdateOpenWindows(_dockingManager.Layout);
            }
            else
            {
                throw new KeyNotFoundException($"Layout with name {layoutName} not found.");
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

        private void UpdateOpenWindows(ILayoutContainer layoutContainer)
        {
            foreach (var child in layoutContainer.Children)
            {
                switch (child)
                {
                    case LayoutAnchorablePane layoutAnchorablePane:
                        foreach (var anchorable in layoutAnchorablePane.Children)
                        {
                            if (anchorable.Content is UserControl userControl)
                            {
                                var windowType = userControl.GetType();
                                _openWindows[windowType] = anchorable;
                                anchorable.CanClose = true;

                                // Attach Closed event to properly track the window closure
                                anchorable.Closed += (s, e) =>
                                {
                                    if (_openWindows.TryGetValue(windowType, out var existingAnchorable))
                                    {
                                        _openWindows.Remove(windowType);
                                    }
                                };
                            }
                        }
                        break;

                    case LayoutDocumentPane layoutDocumentPane:
                        foreach (var document in layoutDocumentPane.Children)
                        {
                            if (document.Content is UserControl userControl)
                            {
                                var windowType = userControl.GetType();
                                _openWindows[windowType] = document;
                                document.CanClose = true;

                                document.Closed += (s, e) =>
                                {
                                    if (_openWindows.TryGetValue(windowType, out var existingAnchorable))
                                    {
                                        _openWindows.Remove(windowType);
                                    }
                                };
                            }
                        }
                        break;
                    case ILayoutContainer subContainer:
                        UpdateOpenWindows(subContainer); // Recursively update sub-containers
                        break;
                }
            }
        }

        private void LayoutSerializationCallback(object sender, LayoutSerializationCallbackEventArgs e)
        {
            var contentId = e.Model.ContentId;

            // Attempt to resolve the window type from ContentId
            var windowType = Type.GetType(contentId);
            if (windowType == null || !typeof(UserControl).IsAssignableFrom(windowType))
            {
                Debug.WriteLine($"Unable to restore window for ContentId: {contentId}");
                e.Cancel = true;
                return;
            }

            var attribute = windowType.GetCustomAttribute<WindowAttribute>();
            if (attribute == null)
            {
                Debug.WriteLine($"Missing {nameof(WindowAttribute)} for window type: {windowType.Name}");
                e.Cancel = true;
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

        private string GetXmlContent(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new FileNotFoundException($"Resource {resourceName} not found.");
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private void LoadLayoutsFromResources()
        {
            _layoutDictionary.Clear();
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames();

            foreach (var resourceName in resourceNames)
            {
                if (resourceName.EndsWith(".xml") && resourceName.Contains("Layouts"))
                {
                    _layoutDictionary[resourceName] = GetXmlContent(resourceName);
                }
            }
        }

    }
}
