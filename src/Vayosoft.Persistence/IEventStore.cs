using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence
{
    public interface IEventStore<TKey> where TKey : notnull
    {
        Task<IAggregate<TKey>> Load(TKey aggregateId, CancellationToken cancellationToken = default);
        Task SaveAsync(IAggregate<TKey> aggregate, CancellationToken cancellationToken = default);
    }
}
