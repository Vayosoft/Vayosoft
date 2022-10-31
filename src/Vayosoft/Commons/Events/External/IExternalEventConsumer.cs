namespace Vayosoft.Commons.Events.External
{
    public interface IExternalEventConsumer
    {
        Task StartAsync(CancellationToken cancellationToken);
    }
}
