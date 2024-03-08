using Avalonia.Controls;
using DX12Editor.ViewModels;

namespace DX12Editor.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
        }

        private void OnMainWindowLoaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded;
            ((MainWindowViewModel)DataContext).Show();
        }
    }
}