using Vayosoft.Commons.Entities;
using Vayosoft.Specifications;

namespace Vayosoft.Persistence
{
    public interface ILinqProvider
    {
        IQueryable<TEntity> AsQueryable<TEntity>()
            where TEntity : class, IEntity;

        IQueryable<TEntity> AsQueryable<TEntity>(ISpecification<TEntity> specification)
            where TEntity : class, IEntity;
    }
}
