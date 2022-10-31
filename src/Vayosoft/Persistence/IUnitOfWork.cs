using Vayosoft.Commons.Entities;

namespace Vayosoft.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        void Add<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        void Update<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        void Delete<TEntity>(TEntity entity)
            where TEntity : class, IEntity;

        TEntity Find<TEntity>(object id)
            where TEntity : class, IEntity;

        void Commit();
        Task CommitAsync();
    }
}