using System.Linq.Expressions;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence
{
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        Task<T> FindAsync<TId>(TId id,
            CancellationToken cancellationToken = default) where TId : notnull;


        Task AddAsync(T entity,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync<TId>(TId id,
            CancellationToken cancellationToken = default) where TId : notnull;

        Task<TResult> FindAsync<TId, TResult>(TId id,
            CancellationToken cancellationToken = default) where TId : notnull;

        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria,
            CancellationToken cancellationToken = default);

        Task<T> FirstOrDefaultAsync(ILinqSpecification<T> spec,
            CancellationToken cancellationToken = default);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> criteria,
            CancellationToken cancellationToken = default);


        Task<List<T>> ListAsync(ISpecification<T> spec,
            CancellationToken cancellationToken = default);

        Task<PagedList<T>> PagedListAsync(int page, int pageSize, ISpecification<T> spec,
            CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<T>> PagedListAsync(Expression<Func<T, bool>> criteria, IPagingModel<T, object> model,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<T> StreamAsync(ISpecification<T> spec,
            CancellationToken cancellationToken = default);
    }
}
