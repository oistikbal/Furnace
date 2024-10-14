using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using Google.FlatBuffers;
using Microsoft.Extensions.Logging;

namespace FurnaceEditor.Services
{
    internal class EngineService
    {
        private const string MmfName = "FurnaceEngine";
        private const int FileSize = 1024 * 1024;
        private const int MessageSize = 256;
        private const int HeaderSize = 2; // sender_id and package_id (1 byte each)
        private bool _isRunning = true;
        private ConcurrentQueue<FlatBufferBuilder> _messageQueue = new ConcurrentQueue<FlatBufferBuilder>();
        private Process? _engineProcess;
        private readonly ILogger _logger;
        private MemoryMappedFile _mmf;
        private readonly CancellationTokenSource _cancellationToken;

        public event Action<Furnace.Buffers.MessageWrapper> OnMessageReceived;
        public EngineService(ILogger<EngineService> logger)
        {
            _logger = logger;

            try
            {
                _mmf = MemoryMappedFile.CreateOrOpen(MmfName, FileSize);
                _cancellationToken = new CancellationTokenSource();
                Thread writerThread = new Thread(Write);
                Thread readerThread = new Thread(Read);
                writerThread.IsBackground = true;
                readerThread.IsBackground = true;
                writerThread.Start();
                readerThread.Start();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            OnMessageReceived += EngineService_OnMessageReceived;
#if DEBUG
            if (!Debugger.IsAttached)
            {
                ProcessStartInfo info = new ProcessStartInfo()
                {
                    FileName = "FurnaceEngine.exe",
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                };

                _engineProcess = Process.Start(info);
            }
#endif
            _logger.LogInformation("EngineService initialized.");
        }

        private void EngineService_OnMessageReceived(Furnace.Buffers.MessageWrapper obj)
        {
            if (obj.MessageType == Furnace.Buffers.AnyMessage.Log)
            {
                var log = obj.MessageAsLog();
                switch (log.LogType)
                {
                    case Furnace.Buffers.LogType.Info:
                        _logger.LogInformation(log.Text);
                        break;
                    case Furnace.Buffers.LogType.Warn:
                        _logger.LogWarning(log.Text);
                        break;
                    case Furnace.Buffers.LogType.Error:
                        _logger.LogError(log.Text);
                        break;
                }
            }
        }

        ~EngineService()
        {
            _cancellationToken.Cancel();
            _mmf.Dispose();
            _engineProcess?.Kill();
        }

        public void Send(FlatBufferBuilder builder)
        {
            _messageQueue.Enqueue(builder);
        }

        private void Write()
        {
            using (var accessor = _mmf.CreateViewAccessor(0, MessageSize))
            {
                byte senderId = 2; // C# sender ID
                byte packageId = 0;

                while (!_cancellationToken.IsCancellationRequested)
                {
                    if (_messageQueue.TryDequeue(out FlatBufferBuilder builder))
                    {
                        var buffer = builder.SizedByteArray();
                        accessor.Write(0, senderId);
                        accessor.Write(1, packageId);
                        accessor.WriteArray(HeaderSize, buffer, 0, buffer.Length);
                        packageId++;

                        Thread.Sleep(10);
                    }
                }
            }
        }

        private void Read()
        {
            using (var accessor = _mmf.CreateViewAccessor(0, MessageSize))
            {
                byte lastPackageId = 255;

                while (!_cancellationToken.IsCancellationRequested)
                {
                    byte senderId = accessor.ReadByte(0);
                    byte packageId = accessor.ReadByte(1);

                    if (packageId != lastPackageId && senderId != 2)
                    {
                        byte[] buffer = new byte[MessageSize];
                        accessor.ReadArray(HeaderSize, buffer, 0, buffer.Length);

                        var bb = new ByteBuffer(buffer);
                        var messageWrapper = Furnace.Buffers.MessageWrapper.GetRootAsMessageWrapper(bb);

                        OnMessageReceived?.Invoke(messageWrapper);
                        lastPackageId = packageId;
                    }

                    Thread.Sleep(10);
                }
            }
        }
    }

}
