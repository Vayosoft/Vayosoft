using Vayosoft.Commons.Entities;

namespace Vayosoft.Persistence
{
    public interface ILinqProvider
    {
        IQueryable<TEntity> AsQueryable<TEntity>()
            where TEntity : class, IEntity;
    }
}
