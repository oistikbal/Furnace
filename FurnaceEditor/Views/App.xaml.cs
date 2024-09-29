using System.Reflection;
using System.Windows;
using FurnaceEditor.Attributes;
using FurnaceEditor.Services;
using FurnaceEditor.Utilities.Providers;
using FurnaceEditor.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace FurnaceEditor.Views
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

        private void OnProjectOpen(string projectPath)
        {
            var services = new ServiceCollection();
            ConfigureServices(services, projectPath);
            _serviceProvider = services.BuildServiceProvider();

            var editorWindow = _serviceProvider.GetRequiredService<EditorWindow>();
            editorWindow.DataContext = _serviceProvider.GetRequiredService<EditorWindowViewModel>();
            editorWindow.Show();
            MainWindow = editorWindow;
            _serviceProvider.GetRequiredService<EngineService>();
            _projectDialog?.Close();
        }

        private void ConfigureServices(IServiceCollection services, string projectPath)
        {
            RegisterWindowsWithAttribute(services, Assembly.GetExecutingAssembly());

            var loggerProvider = new LoggerProvider();
            services.AddSingleton<EditorWindow>();
            services.AddSingleton<EngineService>();
            services.AddSingleton<IObservableLoggerProvider>(loggerProvider);

            services.AddLogging(configure =>
            {
                configure.AddProvider(loggerProvider);
                configure.SetMinimumLevel(LogLevel.Information);
            });
            services.AddSingleton<EditorWindowViewModel>(provider =>
            {
                return new EditorWindowViewModel(provider.GetRequiredService<WindowsService>(),
                    provider.GetRequiredService<ProjectService>(), provider.GetRequiredService<ILogger<EditorWindowViewModel>>());
            });

            services.AddSingleton<WindowsService>(provider =>
            {
                return new WindowsService(new ViewModelProvider(provider), provider.GetRequiredService<EditorWindow>().dockManager);
            });

            services.AddSingleton<ProjectService>(provider =>
            {
                return new ProjectService(projectPath, MessageBus.Current, provider.GetRequiredService<ILogger<ProjectService>>());
            });

            services.AddSingleton<SceneService>(provider =>
            {
                return new SceneService(projectPath);
            });

            services.AddSingleton<UndoService>();
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
