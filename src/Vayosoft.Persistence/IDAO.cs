using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence
{
    public interface IDAO
    {
        Task<TEntity> FindAsync<TEntity>(ICriteria<TEntity> criteria,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        
        Task CreateAsync<TEntity>(TEntity entity,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        Task UpdateAsync<TEntity>(TEntity entity,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        Task DeleteAsync<TEntity>(TEntity entity,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;


        Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        Task<PagedList<TEntity>> PagedListAsync<TEntity>(int page, int pageSize, ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        IAsyncEnumerable<TEntity> StreamAsync<TEntity>(ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity;
    }
}
