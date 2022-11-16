using Vayosoft.Commons.Entities;
using Vayosoft.Specifications;

namespace Vayosoft.Persistence
{
    public interface ILinqProvider
    {
        IQueryable<TEntity> AsQueryable<TEntity>(ISpecification<TEntity> specification = default)
            where TEntity : class, IEntity;
    }
}
