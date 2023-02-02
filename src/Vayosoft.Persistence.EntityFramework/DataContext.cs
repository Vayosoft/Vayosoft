using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.EntityFramework.Converters;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.EntityFramework
{
    public class DataContext : DbContext, ILinqProvider, IDataProvider, IUnitOfWork
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


        public new void Add<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            base.Add(entity);
        }

        public new async ValueTask AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            await base.AddAsync(entity, cancellationToken);
        }

        public new void Update<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            base.Update(entity);
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


        public Task<TEntity> SingleAsync<TEntity>(ICriteria<TEntity> criteria, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                //.AsNoTracking()
                .ByCriteria(criteria)
                .SingleOrDefaultAsync(cancellationToken);
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

        public async Task<IPagedEnumerable<TEntity>> PageAsync<TEntity>(int page, int pageSize, ISpecification<TEntity> spec, 
            CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            var query = Set<TEntity>()
                //.AsNoTracking()
                .BySpecification(spec);

            var paginate = query.Paginate(page, pageSize);
            return new PagedCollection<TEntity>(await paginate.ToArrayAsync(cancellationToken), await query.CountAsync(cancellationToken));
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
