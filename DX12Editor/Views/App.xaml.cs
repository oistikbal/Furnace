using System.Diagnostics;
using System.Reflection;
using System.Windows;
using DX12Editor.Attributes;
using DX12Editor.Services;
using DX12Editor.Utilities.Providers;
using DX12Editor.ViewModels;
using DX12Editor.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace DX12Editor.Views
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;
        private ProjectDialog? _projectDialog;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _projectDialog = new ProjectDialog();
            var projectDialogViewModel = new ProjectDialogViewModel();
            _projectDialog.DataContext = projectDialogViewModel;
            projectDialogViewModel.ProjectOpen += OnProjectOpen;
            _projectDialog.Show();
        }

        private void OnProjectOpen(string path)
        {
            var services = new ServiceCollection();
            ConfigureServices(services, path);
            _serviceProvider = services.BuildServiceProvider();

            _serviceProvider.GetRequiredService<ConsoleWindowViewModel>();

            var editorWindow = _serviceProvider.GetRequiredService<EditorWindow>();
            editorWindow.DataContext = _serviceProvider.GetRequiredService<EditorWindowViewModel>();
            editorWindow.Show();
            MainWindow = editorWindow;
            _projectDialog?.Close();
        }

        private void ConfigureServices(IServiceCollection services, string path)
        {
            RegisterWindowsWithAttribute(services, Assembly.GetExecutingAssembly());

            services.AddSingleton<EditorWindow>();

            services.AddSingleton<EditorWindowViewModel>(provider =>
            {
                return new EditorWindowViewModel(provider.GetRequiredService<WindowsService>(), provider.GetRequiredService<ProjectService>());
            });

            services.AddSingleton<WindowsService>(provider =>
            {
                return new WindowsService(new ViewModelProvider(provider), provider.GetRequiredService<EditorWindow>().dockManager);
            });

            services.AddSingleton<ProjectService>(provider =>
            {
                return new ProjectService(path, MessageBus.Current);
            });

            services.AddSingleton<SceneService>(provider =>
            {
                return new SceneService(path);
            });
        }

        private void RegisterWindowsWithAttribute(IServiceCollection services, Assembly assembly)
        {
            var windowTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<WindowAttribute>() != null)
                .ToList();

            foreach (var windowType in windowTypes)
            {
                var attribute = windowType.GetCustomAttribute<WindowAttribute>();
                if (attribute != null)
                {

                    // Ensure the ViewModelType is valid and register it
                    var viewModelType = attribute.ViewModelType;
                    if (viewModelType != null && typeof(ViewModelBase).IsAssignableFrom(viewModelType))
                    {
                        services.AddSingleton(viewModelType);
                    }
                    else
                    {
                        throw new InvalidOperationException($"The ViewModel type for {windowType.Name} must inherit from {nameof(ViewModelBase)}.");
                    }
                }
            }
        }

    }
}
