namespace Vayosoft.Queries
{
    public interface IQueryBus
    {
        Task<TResponse> Send<TResponse>(IQuery<TResponse> query,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<TResponse> Send<TResponse>(IStreamQuery<TResponse> query,
            CancellationToken cancellationToken = default);
    }
}
