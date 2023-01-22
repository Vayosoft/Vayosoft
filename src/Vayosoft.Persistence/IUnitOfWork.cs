using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        TEntity Find<TEntity>(object id)
            where TEntity : class, IEntity;

        Task<TEntity> FindAsync<TEntity>(object id,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        Task<TEntity> FindAsync<TEntity>(ICriteria<TEntity> criteria,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        void Add<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        ValueTask AddAsync<TEntity>(TEntity entity,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        void Update<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        void Commit();

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}