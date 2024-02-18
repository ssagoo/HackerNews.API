using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace HackerNews.API.Application.Dispatchers
{
    public interface IDispatcher : IDisposable
    {
        int CurrentQueueDepth { get; }
        bool IsProcessing { get; }
        event DispatcherEventCallback HandleEvent;
        void Start(CancellationToken startupToken);
        void Stop();
        bool Enqueue(IDispatchEvent dispatchEvent);
    }

    /// <summary>
    /// A concrete Dispatcher which supports processing incoming events in a 'first in first out' ordered manner using a single thread and blocking collection
    /// </summary>
    public class SingleThreadedDispatcher : IDispatcher
    {
        private readonly BlockingCollection<IDispatchEvent> _eventQueue;
        private readonly int _maxShutdownWaitTime;

        private Task _processingTask;
        private CancellationToken _startupToken;
        private long _isProcessing;
        private readonly ILogger<SingleThreadedDispatcher> _logger;

        public SingleThreadedDispatcher(ILogger<SingleThreadedDispatcher> logger)
        {
            _logger = logger;
            _eventQueue = new BlockingCollection<IDispatchEvent>();
            _maxShutdownWaitTime = (int)TimeSpan.FromSeconds(5).TotalMilliseconds; // TODO: Add to app config
        }

        public int CurrentQueueDepth => _eventQueue.Count;

        public bool IsProcessing => Interlocked.Read(ref _isProcessing) == 1;

        public event DispatcherEventCallback HandleEvent;

        public void Start(CancellationToken startupToken)
        {
            if (IsProcessing) return;

            _startupToken = startupToken;
            _processingTask = Task.Factory.StartNew(StartProcessing, startupToken, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            Dispose();
        }

        private void StartProcessing(object state)
        {
            if (Interlocked.CompareExchange(ref _isProcessing, 1, 0) == 1)
            {
                _logger.LogWarning("Cannot start processing again as its already running");
                return;
            }

            try
            {
                foreach (var dispatchEvent in _eventQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        HandleEvent?.Invoke(this, new DispatcherEventArgs(dispatchEvent, _startupToken));
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"Failed to handle event '{dispatchEvent.Event.EventId}', reason: {e.Message}");
                        dispatchEvent.SetError(e);
                    }
                }
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }
        }

        public bool Enqueue(IDispatchEvent dispatchEvent)
        {
            if (_eventQueue.IsAddingCompleted) return false;

            _eventQueue.Add(dispatchEvent, _startupToken);
            return true;
        }

        public void Dispose()
        {
            if (!IsProcessing) return;

            _eventQueue.CompleteAdding();
            _processingTask.Wait(_maxShutdownWaitTime);
        }
    }
}