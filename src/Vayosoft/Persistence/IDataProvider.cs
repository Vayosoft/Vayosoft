using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence
{
    public interface IDataProvider
    {
        Task<TEntity> SingleAsync<TEntity>(ICriteria<TEntity> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;
        Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity;
        IAsyncEnumerable<TEntity> StreamAsync<TEntity>(ISpecification<TEntity> spec)
            where TEntity : class, IEntity;
    }
}
