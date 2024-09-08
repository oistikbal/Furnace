using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace DX12Editor.Utilities.Loggers
{
    public class ConsoleWriter : TextWriter
    {
        private event Action<string> _callback;

        private const string _prefix = "[DEBUG]";

        public ConsoleWriter(Action<string> callback)
        {
            _callback = callback;
        }

        // Override Write methods
        public override void Write(char value)
        {
            _callback.Invoke($"{_prefix}: {value}");
        }

        public override void Write(string value)
        {
            _callback.Invoke($"{_prefix}: {value}");
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _callback.Invoke($"{_prefix}: {new string(buffer, index, count)}");
        }

        public override void WriteLine()
        {
            _callback.Invoke($"{_prefix}:");
        }

        public override void WriteLine(char value)
        {
            _callback.Invoke($"{_prefix}: {value}");
        }

        public override void WriteLine(string value)
        {
            _callback.Invoke($"{_prefix}: {value}");
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            _callback.Invoke($"{_prefix}: {new string(buffer, index, count)}");
        }

        public override void Write(bool value)
        {
            _callback.Invoke($"{_prefix}: {value}");
        }

        public override Encoding Encoding => Encoding.UTF8;
    }


    public enum LogType
    {
        Info = 0x01,
        Warn = 0x02,
        Error = 0x04,
    }

    public class LogMessage
    {
        public DateTime Time { get; }
        public LogType MessageType { get; }
        public string Message { get; }
        public string File { get; }
        public string Caller { get; }
        public int Line { get; }
        public string MetaData => $"{File}: {Caller} ({Line})";

        public LogMessage(LogType type, string msg, string file, string caller, int line)
        {
            Time = DateTime.Now;
            MessageType = type;
            Message = msg;
            File = System.IO.Path.GetFileName(file);
            Caller = caller;
            Line = line;
        }
    }


    public class EditorLogger : ILogger
    {
        private readonly ObservableCollection<LogMessage> _logs = new();
        private readonly string _category;
        public int _messageFilter = (int)(LogType.Info | LogType.Warn | LogType.Error);

        public EditorLogger(string category, ObservableCollection<LogMessage> logs)
        {
            _category = category;
            _logs = logs;
            Trace.Listeners.Add(new TextWriterTraceListener(new ConsoleWriter(TraceCallback), "EditorLogger"));
            Trace.AutoFlush = true;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var type = ConvertLogLevelToMessageType(logLevel);
            var message = formatter(state, exception);

            // Capture caller information
            var callerFilePath = "";
            var callerMemberName = "";
            var callerLineNumber = 0;

            // Use caller information if available
            if (formatter.Method.GetCustomAttributes(typeof(CallerFilePathAttribute), false).Length > 0)
                callerFilePath = formatter.Method.GetCustomAttributes(typeof(CallerFilePathAttribute), false)[0].ToString();
            if (formatter.Method.GetCustomAttributes(typeof(CallerMemberNameAttribute), false).Length > 0)
                callerMemberName = formatter.Method.GetCustomAttributes(typeof(CallerMemberNameAttribute), false)[0].ToString();
            if (formatter.Method.GetCustomAttributes(typeof(CallerLineNumberAttribute), false).Length > 0)
                callerLineNumber = (int)formatter.Method.GetCustomAttributes(typeof(CallerLineNumberAttribute), false)[0];

            // Update log collection on the UI thread
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsEnabled(logLevel))
                {
                    _logs.Add(new LogMessage(type, $"[{_category}]: {message}", callerFilePath, callerMemberName, callerLineNumber));
                }
            }));
        }

        private LogType ConvertLogLevelToMessageType(LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.Information => LogType.Info,
                LogLevel.Warning => LogType.Warn,
                LogLevel.Error => LogType.Error,
                _ => 0
            };
        }

        private void TraceCallback(string message)
        {
            _logs.Add(new LogMessage(LogType.Info, message, "", "", 0));

        }
    }
}
