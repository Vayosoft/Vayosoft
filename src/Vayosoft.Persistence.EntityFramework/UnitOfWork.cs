using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Exceptions;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        public UnitOfWork(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<T> GetAsync<T>(object id, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            return await _dataContext.Set<T>()
                       .AsTracking()
                       .SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken) ??
                   throw AggregateNotFoundException.For<T>(id);
        }

        public Task<List<T>> GetAsync<T>(ICriteria<T> criteria, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            return _dataContext.Set<T>()
                .AsTracking()
                .ByCriteria(criteria)
                .ToListAsync(cancellationToken);
        }
        
        public new async ValueTask AddAsync<T>(T entity,
            CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            await _dataContext.AddAsync(entity, cancellationToken);
        }

        public new void Update<T>(T entity)
            where T : class, IAggregateRoot
        {
            _dataContext.Update(entity);
        }

        public new void Remove<T>(T entity)
            where T : class, IAggregateRoot
        {
            _dataContext.Remove(entity);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _dataContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (HandleConcurrency(_dataContext, ex.Entries))
                {
                    await _dataContext.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw;
                }

            }
        }

        protected virtual bool HandleConcurrency(DbContext context, IReadOnlyList<EntityEntry> entries)
        {
            return false;
        }

        public void Dispose()
        {
            
        }
    }
}
