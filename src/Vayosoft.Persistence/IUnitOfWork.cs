using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<T> GetAsync<T>(object id, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        ValueTask AddAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        void Update<T>(T entity)
            where T : class, IAggregateRoot;

        void Remove<T>(T entity)
            where T : class, IAggregateRoot;

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
