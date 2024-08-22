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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Debug.WriteLine(e.Args.Length);
            var services = new ServiceCollection();
            ConfigureServices(services, e);

            _serviceProvider = services.BuildServiceProvider();
            _serviceProvider.GetRequiredService<ConsoleWindowViewModel>();

            _serviceProvider.GetRequiredService<WindowsService>();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.DataContext = _serviceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services, StartupEventArgs e)
        {
            RegisterWindowsWithAttribute(services, Assembly.GetExecutingAssembly());

            services.AddSingleton<MainWindow>(provider =>
            {
                var mainWindow = new MainWindow();
                return mainWindow;
            });

            services.AddSingleton<MainWindowViewModel>(provider =>
            {
                return new MainWindowViewModel(provider.GetRequiredService<WindowsService>());
            });

            services.AddSingleton<WindowsService>(provider =>
            {
                return new WindowsService(new ViewModelProvider(provider), provider.GetRequiredService<MainWindow>().dockManager);
            });

            services.AddSingleton<ProjectService>(provider =>
            {
                return new ProjectService(string.Empty);
            });
        }

        public void RegisterWindowsWithAttribute(IServiceCollection services, Assembly assembly)
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
