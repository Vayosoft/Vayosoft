using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Exceptions;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.EntityFramework.Converters;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.EntityFramework
{
    public class DataContext : DbContext, ILinqProvider, IDAO, IUnitOfWork
    {
        public DataContext(DbContextOptions options)
            : base(options) { }

        public TEntity Find<TEntity>(object id)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .AsTracking()
                .SingleOrDefault(x => x.Id == id);
        }

        public Task<TEntity> FindAsync<TEntity>(object id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .AsTracking()
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
        }

        public Task<TEntity> FindAsync<TEntity>(ICriteria<TEntity> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .AsTracking()
                .ByCriteria(criteria)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task CreateAsync<TEntity>(TEntity entity,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            await base.AddAsync(entity, cancellationToken);
            await CommitAsync(cancellationToken);
        }

        public async Task UpdateAsync<TEntity>(TEntity entity, 
            CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            base.Update(entity);
            await CommitAsync(cancellationToken);
        }

        public async Task DeleteAsync<TEntity>(TEntity entity, 
            CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            base.Remove(entity);
            await CommitAsync(cancellationToken);
        }

        public async Task<T> GetAsync<T>(object id, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            return await Set<T>()
                .AsTracking()
                .SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken) ??
                   throw AggregateNotFoundException.For<T>(id);
        }

        public new void Add<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            base.Add(entity);
        }

        public new async ValueTask AddAsync<T>(T entity,
            CancellationToken cancellationToken = default) 
            where T : class, IAggregateRoot
        {
            await base.AddAsync(entity, cancellationToken);
        }

        public new void Update<T>(T entity)
            where T : class, IAggregateRoot
        {
            base.Update(entity);
        }

        public new void Remove<T>(T entity)
            where T : class, IAggregateRoot
        {
            base.Remove(entity);
        }

        public void Delete<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            base.Remove(entity);
        }

        public void Commit()
        {
            SaveChanges();
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (HandleConcurrency(this, ex.Entries))
                {
                    await SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw;
                }

            }
        }

        protected virtual bool HandleConcurrency(DataContext context, IReadOnlyList<EntityEntry> entries)
        {
            return false;
        }


        public Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                //.AsNoTracking()
                .BySpecification(spec)
                .ToListAsync(cancellationToken);
        }

        public IAsyncEnumerable<TEntity> StreamAsync<TEntity>(ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            return Set<TEntity>()
                //.AsNoTracking()
                .BySpecification(spec)
                .AsAsyncEnumerable();
        }

        public async Task<PagedList<TEntity>> PagedListAsync<TEntity>(ISpecification<TEntity> spec, 
            CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            var query = Set<TEntity>()
                //.AsNoTracking()
                .BySpecification(spec);

            var paginate = query.Paginate(spec.Page, spec.PageSize);
            return new PagedList<TEntity>(await paginate.ToArrayAsync(cancellationToken), await query.CountAsync(cancellationToken));
        }

        public IQueryable<TEntity> AsQueryable<TEntity>()
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .AsNoTracking();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var typesToRegister = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(type =>
                    !string.IsNullOrEmpty(type.Namespace)
                    && type.BaseType is {IsGenericType: true} && type.BaseType.GetGenericTypeDefinition() ==
                    typeof(EntityConfigurationMapper<>));

            foreach (var type in typesToRegister)
            {
                dynamic configInstance = Activator.CreateInstance(type)!;
                modelBuilder.ApplyConfiguration(configInstance);
            }

            modelBuilder.UseUtcDateTimeConverter();
        }
    }
}
