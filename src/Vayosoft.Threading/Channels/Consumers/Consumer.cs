using System.Threading.Channels;

namespace Vayosoft.Threading.Channels.Consumers
{
    public class ConsumerAsync<T> : ConsumerBase<T>
    {
        private readonly Func<T, CancellationToken, ValueTask> _consumeAction;


        public ConsumerAsync(ChannelReader<T> channelReader, string workerName, Func<T, CancellationToken, ValueTask> consumeAction, CancellationToken globalCancellationToken)
            : base(channelReader, workerName, globalCancellationToken)
        {
            _consumeAction = consumeAction ?? throw new ArgumentNullException(nameof(consumeAction));
        }

        public override ValueTask OnDataReceivedAsync(T item, CancellationToken token, string _)
        {
            return _consumeAction.Invoke(item, token);
        }
    }
}
