using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Persistence.Extensions;

namespace Vayosoft.Persistence.MongoDB
{
    public class MongoDbUoW : IUnitOfWork
    {
        private bool _disposed;
        private readonly List<Func<CancellationToken, Task>> _commands = new();
        private readonly Dictionary<string, object> _repositories = new();

        protected readonly IMongoDbConnection Connection;
        protected readonly IServiceScope Scope;

        public MongoDbUoW(IMongoDbConnection connection, IServiceProvider serviceProvider)
        {
            Connection = connection;
            Scope = serviceProvider.CreateScope();
        }
        public Task<T> GetAsync<T>(object id, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            return Repository<T>()
                .GetAsync(id, cancellationToken);
        }

        public ValueTask AddAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            AddCommand(cToken =>
                Repository<T>()
                    .AddAsync(entity, cToken));

            return ValueTask.CompletedTask;
        }

        public void Update<T>(T entity) where T : class, IAggregateRoot
        {
            AddCommand(cToken =>
                Repository<T>()
                    .UpdateAsync(entity, cToken));
        }

        public void Remove<T>(T entity) where T : class, IAggregateRoot
        {
            AddCommand(cToken =>
                Repository<T>()
                    .DeleteAsync(entity, cToken));
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            using var session = await Connection.StartSessionAsync(cancellationToken: cancellationToken);

            session.StartTransaction();
            await Task.WhenAll(_commands.Select(c => c(cancellationToken)));
            await session.CommitTransactionAsync(cancellationToken);
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
                Scope?.Dispose();
            }
            _disposed = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddCommand(Func<CancellationToken, Task> func)
        {
            _commands.Add(func);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T Repository<T, TEntity>() where T : IRepository<TEntity> where TEntity : class, IAggregateRoot
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IRepository<T> Repository<T>() where T : class, IAggregateRoot
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
