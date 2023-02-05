using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence
{
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        Task<T> FindAsync(object id,
            CancellationToken cancellationToken = default);
        Task<T> FindAsync(ICriteria<T> criteria,
            CancellationToken cancellationToken = default);

        Task AddAsync(T entity,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(object id,
            CancellationToken cancellationToken = default);


        Task<List<T>> ListAsync(ISpecification<T> specification,
            CancellationToken cancellationToken = default);

        Task<PagedList<T>> PagedListAsync(ISpecification<T> specification,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable<T> StreamAsync(ISpecification<T> specification,
            CancellationToken cancellationToken = default);
    }
}
