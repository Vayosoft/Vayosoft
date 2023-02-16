using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence.MongoDB
{
    public class MongoDbUoW : IUnitOfWork
    {
        private bool _disposed;
        private readonly Dictionary<string, object> _repositories = new();
        private readonly IMongoDbContext _context;

        protected readonly IServiceScope Scope;

        public MongoDbUoW(IServiceProvider serviceProvider, IMongoDbContext context)
        {
            Scope = serviceProvider.CreateScope();

            _context = context;
        }
        public Task<T> FindAsync<T>(object id, CancellationToken cancellationToken = default) where T : class, IAggregateRoot
        {
            return Repository<T>()
                .FindAsync(id, cancellationToken);
        }

        public ValueTask AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IAggregateRoot
        {
            _context.AddCommand(cToken =>
                Repository<T>().AddAsync(entity, cToken));

            return ValueTask.CompletedTask;
        }

        public void Update<T>(T entity) where T : class, IAggregateRoot
        {
            _context.AddCommand(cToken =>
                Repository<T>().UpdateAsync(entity, cToken));
        }

        public void Delete<T>(T entity) where T : class, IAggregateRoot
        {
            _context.AddCommand(cToken =>
                Repository<T>().DeleteAsync(entity, cToken));
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                Scope.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }

        protected T Repository<T, TEntity>() where T : IRepository<TEntity> where TEntity : class, IAggregateRoot
        {
            var key = typeof(T).Name;
            if (_repositories.TryGetValue(key, out var repo))
            {
                return (T)repo;
            }

            var r = Scope.ServiceProvider.GetRequiredService<T>();
            _repositories.Add(key, r);

            return r;
        }

        protected IRepository<T> Repository<T>() where T : class, IAggregateRoot
        {
            var key = typeof(T).Name;
            if (_repositories.TryGetValue(key, out var repo))
            {
                return (IRepository<T>)repo;
            }

            var r = Scope.ServiceProvider.GetRequiredService<IRepository<T>>();
            _repositories.Add(key, r);

            return r;
        }
    }
}
