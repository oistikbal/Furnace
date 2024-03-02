using Avalonia.Controls;

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
            var projectBroweser = new ProjectBrowserDialog();
            projectBroweser.Show();
        }
    }
}