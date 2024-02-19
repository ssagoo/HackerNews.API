using HackerNews.API.Application.Dispatchers;

namespace HackerNews.API.Application.Processors
{
    public interface IProcessor
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop();
    }

    /// <summary>
    /// A simple base processor to encapsulate registering an interest in a Dispatcher to receive and process requests
    /// </summary>
    public abstract class BaseProcessor : IProcessor
    {
        private readonly IDispatcher _dispatcher;

        protected BaseProcessor(IDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
            dispatcher.HandleEvent += OnHandleEvent;
        }

        private void OnHandleEvent(object sender, IDispatcherEventArgs eventArgs)
        {
            var response = Handle(eventArgs.DispatchEvent.Event, eventArgs.CancellationToken, eventArgs.DispatchEvent.Request).Result;

            eventArgs.DispatchEvent.SetResponse(response);
        }

        internal abstract Task<object> Handle(IEvent @event, CancellationToken cancellationToken, object request);

        public virtual Task Start(CancellationToken cancellationToken)
        {
            _dispatcher.Start(cancellationToken);
            return Task.CompletedTask;
        }

        public virtual Task Stop()
        {
            _dispatcher.Stop();
            return Task.CompletedTask;
        }
    }
}