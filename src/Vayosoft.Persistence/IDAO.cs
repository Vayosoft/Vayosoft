using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence
{
    public interface IDAO
    {
        Task<T> FindAsync<T>(ICriteria<T> criteria,
            CancellationToken cancellationToken = default) where T : class, IEntity;

        
        Task CreateAsync<T>(T entity,
            CancellationToken cancellationToken = default) where T : class, IEntity;

        Task UpdateAsync<T>(T entity,
            CancellationToken cancellationToken = default) where T : class, IEntity;

        Task DeleteAsync<T>(T entity,
            CancellationToken cancellationToken = default) where T : class, IEntity;


        Task<List<T>> ListAsync<T>(ISpecification<T> specification,
            CancellationToken cancellationToken = default) where T : class, IEntity;

        Task<PagedList<T>> PagedListAsync<T>(int page, int pageSize, ISpecification<T> specification,
            CancellationToken cancellationToken = default) where T : class, IEntity;

        IAsyncEnumerable<T> StreamAsync<T>(ISpecification<T> specification,
            CancellationToken cancellationToken = default) where T : class, IEntity;
    }
}
