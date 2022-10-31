namespace Vayosoft.Commons.Events.External
{
    public class NullExternalEventProducer : IExternalEventProducer
    {
        public Task Publish(IExternalEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}
