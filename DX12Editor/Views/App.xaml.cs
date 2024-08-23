using System.Diagnostics;
using System.Reflection;
using System.Windows;
using DX12Editor.Attributes;
using DX12Editor.Providers;
using DX12Editor.Services;
using DX12Editor.ViewModels;
using DX12Editor.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;

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
            _serviceProvider.GetRequiredService<ProjectService>();
            _serviceProvider.GetRequiredService<WindowsService>();

            var mainWindow = _serviceProvider.GetRequiredService<EditorWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<EditorWindowViewModel>();
            mainWindow.Show();
            _projectDialog?.Close();
        }

        private void ConfigureServices(IServiceCollection services, string path)
        {
            RegisterWindowsWithAttribute(services, Assembly.GetExecutingAssembly());

            services.AddSingleton<EditorWindow>(provider =>
            {
                var mainWindow = new EditorWindow();
                return mainWindow;
            });

            services.AddSingleton<EditorWindowViewModel>(provider =>
            {
                return new EditorWindowViewModel(provider.GetRequiredService<WindowsService>());
            });

            services.AddSingleton<WindowsService>(provider =>
            {
                return new WindowsService(new ViewModelProvider(provider), provider.GetRequiredService<EditorWindow>().dockManager);
            });

            services.AddSingleton<ProjectService>(provider =>
            {
                return new ProjectService(path);
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

        private string GetProjectPathFromArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-projectPath" && i + 1 < args.Length)
                {
                    return args[i + 1]; // The value following -projectPath
                }
            }
            return null;
        }

    }
}
