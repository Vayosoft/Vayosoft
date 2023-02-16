using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence.MongoDB
{
    public interface IDocumentUoW : IDisposable
    {
        Task<T> FindAsync<T>(object id, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        ValueTask AddAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot;

        void Update<T>(T entity)
            where T : class, IAggregateRoot;

        void Delete<T>(T entity)
            where T : class, IAggregateRoot;

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
