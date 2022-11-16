using Vayosoft.Threading.Channels.Producers;

namespace Vayosoft.Threading.Channels
{
    public class AsyncDefaultChannel<T> : ProducerConsumerChannelBase<T>
    {
        private readonly Func<T, CancellationToken, ValueTask> _consumeAction;

        public AsyncDefaultChannel(
            Func<T, CancellationToken, ValueTask> consumeAction,
            string channelName = null,
            uint startedNumberOfWorkerThreads = 1,
            bool enableTaskManagement = false,
            CancellationToken cancellationToken = default)
            : base(channelName, startedNumberOfWorkerThreads, enableTaskManagement, cancellationToken)
        {
            _consumeAction = consumeAction ?? throw new ArgumentNullException(nameof(consumeAction));
        }

        protected override async ValueTask OnDataReceivedAsync(T item, CancellationToken token)
        {
            try
            {
                await _consumeAction.Invoke(item, token);
            }
            catch (OperationCanceledException) { }
        }
    }
}
