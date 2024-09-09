using Microsoft.Extensions.Logging;
using FurnaceEditor.Utilities.Loggers;
using System.Collections.ObjectModel;

namespace FurnaceEditor.Utilities.Providers
{
    public interface IObservableLoggerProvider
    {
        ObservableCollection<LogMessage> Logs { get; }
    }

    class LoggerProvider : ILoggerProvider, IObservableLoggerProvider
    {
        public ObservableCollection<LogMessage> Logs { get; }

        public LoggerProvider()
        {
            Logs = new();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new EditorLogger(categoryName, Logs);
        }

        public void Dispose()
        {
        }
    }
}
