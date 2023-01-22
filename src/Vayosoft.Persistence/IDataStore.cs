using Vayosoft.Commons.Entities;

namespace Vayosoft.Persistence
{
    public interface IDataStore
    {
        public Task<TEntity> FindAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default) 
            where TEntity : class, IEntity where TId : notnull;

        public Task AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;

        public Task UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) 
            where TEntity : class, IEntity;

        public Task DeleteAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) 
            where TEntity : class, IEntity;
        public Task DeleteAsync<TEntity, TId>(TId id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity where TId : notnull;
    }
}
