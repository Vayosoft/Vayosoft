using System.Linq.Expressions;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence
{
    public interface IReadOnlyRepository<TAggregateRoot> where TAggregateRoot : class, IAggregateRoot
    {
        Task<TResult> FindAsync<TId, TResult>(TId id, 
            CancellationToken cancellationToken = default) where TId : notnull;

        Task<TAggregateRoot> FirstOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> criteria,
            CancellationToken cancellationToken = default);

        Task<TAggregateRoot> FirstOrDefaultAsync(ILinqSpecification<TAggregateRoot> spec,
            CancellationToken cancellationToken = default);

        Task<TAggregateRoot> SingleOrDefaultAsync(Expression<Func<TAggregateRoot, bool>> criteria,
            CancellationToken cancellationToken = default);

        Task<List<TAggregateRoot>> ListAsync(ISpecification<TAggregateRoot> spec,
            CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<TAggregateRoot>> PageAsync(IPagingSpecification<TAggregateRoot> spec,
            CancellationToken cancellationToken = default);

        Task<IPagedEnumerable<TAggregateRoot>> PageAsync(Expression<Func<TAggregateRoot, bool>> criteria, IPagingModel<TAggregateRoot, object> model,
            CancellationToken cancellationToken = default);

        IAsyncEnumerable <TAggregateRoot> StreamAsync(ISpecification<TAggregateRoot> spec,
            CancellationToken cancellationToken = default);
    }
}
