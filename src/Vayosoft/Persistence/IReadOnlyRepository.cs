using System.Linq.Expressions;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Specifications;

namespace Vayosoft.Persistence
{
    public interface IReadOnlyRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        Task<TEntity> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        Task<TResult> FindAsync<TId, TResult>(TId id, CancellationToken cancellationToken = default) where TId : notnull;


        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default);

        Task<TEntity> FirstOrDefaultAsync(ILinqSpecification<TEntity> spec,
            CancellationToken cancellationToken = default);


        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> criteria,
            CancellationToken cancellationToken = default);
        
        Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<TEntity>> PageAsync(ILinqSpecification<TEntity> spec, int page = 1, int pageSize = IPagingModel.DefaultSize,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<TEntity> StreamAsync(ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default);
    }
}
