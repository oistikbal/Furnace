using System.ComponentModel;
using System.Windows.Data;
using DX12Editor.Utilities.Providers;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DX12Editor.ViewModels.Windows
{
    public class ConsoleWindowViewModel : ViewModelBase
    {
        private readonly ILogger<ConsoleWindowViewModel> _logger;

        private ICollectionView _filteredLogs;

        public ICollectionView FilteredLogs
        {
            get => _filteredLogs;
            set => this.RaiseAndSetIfChanged(ref _filteredLogs, value);
        }

        public ConsoleWindowViewModel(ILogger<ConsoleWindowViewModel> logger, IObservableLoggerProvider loggerProvider)
        {
            FilteredLogs = CollectionViewSource.GetDefaultView(loggerProvider.Logs);
        }
    }
}
