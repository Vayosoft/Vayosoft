using System.Linq.Expressions;
using Vayosoft.Commons.Entities;
using Vayosoft.Specifications;

namespace Vayosoft.Persistence
{
    public interface IDataProvider
    {
        Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;
        Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;
        IAsyncEnumerable<TEntity> StreamAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity;
    }
}
