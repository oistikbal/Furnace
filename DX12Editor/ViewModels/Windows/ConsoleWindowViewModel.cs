using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using ReactiveUI;

namespace DX12Editor.ViewModels.Windows
{
    public class ConsoleWriter : TextWriter
    {
        private ObservableCollection<string> _logs;

        public ConsoleWriter(ObservableCollection<string> logs)
        {
            _logs = logs;
        }

        // Override Write methods
        public override void Write(char value)
        {
            _logs.Add($"[Editor]: {value}");
        }

        public override void Write(string value)
        {
            _logs.Add($"[Editor]: {value}");
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _logs.Add($"[Editor]: {new string(buffer, index, count)}");
        }

        public override void WriteLine()
        {
            _logs.Add("[Editor]");
        }

        public override void WriteLine(char value)
        {
            _logs.Add($"[Editor]: {value}");
        }

        public override void WriteLine(string value)
        {
            _logs.Add($"[Editor]: {value}");
        }

        public override void WriteLine(char[] buffer, int index, int count)
        {
            _logs.Add($"[Editor]: {new string(buffer, index, count)}");
        }

        public override void Write(bool value)
        {
            _logs.Add($"[Editor]: {value}");
        }

        public override Encoding Encoding => Encoding.UTF8;
    }


    public class ConsoleWindowViewModel : ViewModelBase
    {
        private ObservableCollection<string> _logs;

        public ObservableCollection<string> Logs
        {
            get => _logs;
            private set => this.RaiseAndSetIfChanged(ref _logs, value);
        }

        public ConsoleWindowViewModel()
        {
            _logs = new ObservableCollection<string>();
            Trace.Listeners.Add(new TextWriterTraceListener(new ConsoleWriter(Logs), "Editor"));
            Trace.AutoFlush = true;
        }
    }
}
