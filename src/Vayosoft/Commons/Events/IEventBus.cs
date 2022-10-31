namespace Vayosoft.Commons.Events
{
    public interface IEventBus
    {
        Task Publish(IEvent @event, CancellationToken cancellationToken = default);
        Task Publish(IEvent[] events, CancellationToken cancellationToken = default);
    }
}
