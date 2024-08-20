using System.Diagnostics;
using System.Reflection;
using System.Windows;
using DX12Editor.ViewModels;
using DX12Editor.ViewModels.Windows;
using DX12Editor.Views.Windows;
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
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            _serviceProvider.GetRequiredService<ConsoleWindowViewModel>();
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            mainWindow.Closed += (object? sender, EventArgs e) => { System.Environment.Exit(0); };
        }

        private void ConfigureServices(IServiceCollection services)
        {
            RegisterWindowsWithAttribute(services, Assembly.GetExecutingAssembly());
            services.AddSingleton<MainWindowViewModel>();
            services.AddTransient<MainWindow>();
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
                    // Register the window type
                    services.AddTransient(windowType);

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
