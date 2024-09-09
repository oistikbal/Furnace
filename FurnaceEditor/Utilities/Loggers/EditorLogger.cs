using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using Microsoft.Extensions.Logging;

namespace FurnaceEditor.Utilities.Loggers
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
        public LogType LogType { get; }
        public string Message { get; }
        public string CallStack { get; }
        public string LastFrame { get; }

        public LogMessage(LogType type, string msg, string callStack, string lastFrame)
        {
            Time = DateTime.Now;
            LogType = type;
            Message = msg;
            CallStack = callStack;
            LastFrame = lastFrame;
        }
    }


    public class EditorLogger : ILogger
    {
        private readonly ObservableCollection<LogMessage> _logs = new();
        private readonly string _category;

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

            StackTrace stackTrace = new StackTrace(true);

            var frames = stackTrace.GetFrames()
                .Where(frame =>
                {
                    var method = frame.GetMethod();
                    var declaringType = method.DeclaringType;

                    // Skip frames until we find the first non-Logger method
                    if (declaringType == null || declaringType == typeof(EditorLogger) || declaringType.Namespace.StartsWith("Microsoft.Extensions.Logging") || frame.GetFileLineNumber() == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                })
                .Select(frame =>
                {
                    var method = frame.GetMethod();
                    var declaringType = method.DeclaringType;
                    var fileName = frame.GetFileName();
                    var lineNumber = frame.GetFileLineNumber();

                    return new
                    {
                        Frame = frame,
                        Details = $"{declaringType}.{method.Name} @ ({fileName}:{lineNumber})"
                    };
                })
                .ToList();

            string callstack = string.Join("\n", frames.Select(f => f.Details));

            // Get the last frame details if available
            var lastFrame = frames.First();

            // Update log collection on the UI thread
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (IsEnabled(logLevel))
                {
                    _logs.Add(new LogMessage(type, $"[{_category}]: {message}", callstack, lastFrame.Details));
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
            StackTrace stackTrace = new StackTrace(true);

            var frames = stackTrace.GetFrames()
                .Where(frame =>
                {
                    var method = frame.GetMethod();
                    var declaringType = method.DeclaringType;

                    // Skip frames until we find the first non-Logger method
                    if (declaringType == typeof(EditorLogger) || declaringType.Namespace.StartsWith("Microsoft.Extensions.Logging") || frame.GetFileLineNumber() == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                })
                .Select(frame =>
                {
                    var method = frame.GetMethod();
                    var declaringType = method.DeclaringType;
                    var fileName = frame.GetFileName();
                    var lineNumber = frame.GetFileLineNumber();

                    return new
                    {
                        Frame = frame,
                        Details = $"{declaringType}.{method.Name} @ ({fileName}:{lineNumber})"
                    };
                })
                .ToList();

            string callstack = string.Join("\n", frames.Select(f => f.Details));

            // Get the last frame details if available
            var lastFrame = frames.First();


            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _logs.Add(new LogMessage(LogType.Info, message, callstack, lastFrame.Details));
            }));
        }
    }
}
