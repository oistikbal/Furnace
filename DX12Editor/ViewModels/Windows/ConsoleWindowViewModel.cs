using System.ComponentModel;
using System.Diagnostics;
using System.Reactive;
using System.Windows.Data;
using DX12Editor.Utilities.Loggers;
using DX12Editor.Utilities.Providers;
using DX12Editor.ViewModels.Components;
using Microsoft.Extensions.Logging;
using ReactiveUI;

namespace DX12Editor.ViewModels.Windows
{
    public class ConsoleWindowViewModel : ViewModelBase
    {
        private readonly ILogger<ConsoleWindowViewModel> _logger;
        private ICollectionView _filteredLogs;
        private int _messageFilter = (int)(LogType.Info | LogType.Warn | LogType.Error);
        private LogMessage _selectedLog;

        private bool _isInfoChecked;
        private bool _isWarnChecked;
        private bool _isErrorChecked;
        private string _filterText = string.Empty;


        #region Commands
        public ReactiveCommand<Unit, Unit> ToggleInfoCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleWarnCommand { get; }
        public ReactiveCommand<Unit, Unit> ToggleErrorCommand { get; }
        public ReactiveCommand<LogMessage, Unit> SelectedLogCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> ClearCommand { get; private set; }
        #endregion

        #region Properties
        public LogMessage SelectedLog
        {
            get => _selectedLog;
            set => this.RaiseAndSetIfChanged(ref _selectedLog, value);
        }

        public ICollectionView FilteredLogs
        {
            get => _filteredLogs;
            set => this.RaiseAndSetIfChanged(ref _filteredLogs, value);
        }

        public string FilterText
        {
            get => _filterText;
            set
            {
                this.RaiseAndSetIfChanged(ref _filterText, value);
                FilteredLogs.Refresh();
            }
        }

        public bool IsInfoChecked
        {
            get => _isInfoChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isInfoChecked, value))
                {
                    UpdateFilter();
                }
            }
        }

        public bool IsWarnChecked
        {
            get => _isWarnChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isWarnChecked, value))
                {
                    UpdateFilter();
                }
            }
        }

        public bool IsErrorChecked
        {
            get => _isErrorChecked;
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isErrorChecked, value))
                {
                    UpdateFilter();
                }
            }
        }
        #endregion

        // TO-DO: Update Filter and Toggle Buttons
        public ConsoleWindowViewModel(ILogger<ConsoleWindowViewModel> logger, IObservableLoggerProvider loggerProvider)
        {
            _logger = logger;
            FilteredLogs = CollectionViewSource.GetDefaultView(loggerProvider.Logs);
            SelectedLogCommand = ReactiveCommand.Create<LogMessage>(log => { SelectedLog = log; });
            ClearCommand = ReactiveCommand.Create(loggerProvider.Logs.Clear);

            IsInfoChecked = true; 
            IsWarnChecked = true; 
            IsErrorChecked = true;
        }

        private void UpdateFilter()
        {
            // Calculate the combined filter value based on selected log types
            int filter = 0;
            if (IsInfoChecked) filter |= (int)LogType.Info;
            if (IsWarnChecked) filter |= (int)LogType.Warn;
            if (IsErrorChecked) filter |= (int)LogType.Error;


            // Apply the filter to the collection view
            FilteredLogs.Filter = log =>
            {
                var logMessage = (LogMessage)log;

                // Check if the log type matches any of the enabled types
                bool isMessageTypeMatch = ((int)logMessage.LogType & filter) != 0;

                // Check if the log message contains the filter text, if any
                bool isTextMatch = string.IsNullOrEmpty(FilterText) ||
                                    logMessage.Message.IndexOf(FilterText, StringComparison.OrdinalIgnoreCase) >= 0;

                return isMessageTypeMatch && isTextMatch;
            };
            _logger.LogInformation("updatefilter");
        }
    }
}
