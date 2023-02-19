using Vayosoft.Commons.Aggregates;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence
{
    public interface IRepository<T> where T : class, IAggregateRoot
    {
        Task<T> FindAsync(object id,
            CancellationToken cancellationToken = default);

        Task<List<T>> FindAsync(ICriteria<T> criteria,
            CancellationToken cancellationToken = default);

        Task AddAsync(T entity,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(T entity,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(T entity,
            CancellationToken cancellationToken = default);
    }
}
