using Vayosoft.Commons.Entities;

namespace Vayosoft.Persistence
{
    public interface IDataStore
    {
        public Task<T> FindAsync<T, TId>(TId id, CancellationToken cancellationToken = default) 
            where T : class, IEntity where TId : notnull;

        public Task AddAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IEntity;

        public Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) 
            where T : class, IEntity;

        public Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) 
            where T : class, IEntity;
        public Task DeleteAsync<T, TId>(TId id, CancellationToken cancellationToken = default)
            where T : class, IEntity where TId : notnull;
    }
}
