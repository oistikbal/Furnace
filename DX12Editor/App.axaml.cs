using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using DX12Editor.ViewModels;
using DX12Editor.Views;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DX12Editor
{
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<Test>();
        }

        public override void OnFrameworkInitializationCompleted()
        {
            BindingPlugins.DataValidators.RemoveAt(0);

            var collection = new ServiceCollection();
            ConfigureServices(collection);
            ServiceProvider = collection.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
                desktop.MainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}