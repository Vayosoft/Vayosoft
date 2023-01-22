using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence
{
    public interface IRepository<TAggregateRoot> : IReadOnlyRepository<TAggregateRoot> 
        where TAggregateRoot : class, IAggregateRoot
    {
        Task<TAggregateRoot> FindAsync<TId>(TId id,
            CancellationToken cancellationToken = default) where TId : notnull;

        Task AddAsync(TAggregateRoot entity,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(TAggregateRoot entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(TAggregateRoot entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync<TId>(TId id,
            CancellationToken cancellationToken = default) where TId : notnull;
    }
}
