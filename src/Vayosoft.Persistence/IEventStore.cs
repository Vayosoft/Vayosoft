using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence
{
    public interface IEventStore<TKey> where TKey : notnull
    {
        Task SaveAsync(IAggregate<TKey> aggregate, CancellationToken cancellationToken = default);
        Task<IAggregate<TKey>> Get(TKey aggregateId, CancellationToken cancellationToken = default);
    }
}
