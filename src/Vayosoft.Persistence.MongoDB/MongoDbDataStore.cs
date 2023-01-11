using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.MongoDB.Extensions;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.MongoDB
{
    public class MongoDbDataStore : IDataStore, IDataProvider, IDisposable
    {
        private bool _disposed;

        protected readonly IMongoDbConnection Connection;
        protected readonly IServiceScope Scope;
        protected readonly Dictionary<string, object> Repositories = new();

        public MongoDbDataStore(IMongoDbConnection connection, IServiceProvider serviceProvider)
        {
            Connection = connection;
            Scope = serviceProvider.CreateScope();
        }

        protected IRepository<T> Repository<T>() where T : class, IAggregateRoot
        {
            var key = typeof(T).Name;
            if (Repositories.TryGetValue(key, out var repo))
            {
                return (IRepository<T>)repo;
            }

            var r = Scope.ServiceProvider.GetRequiredService<IRepository<T>>();
            Repositories.Add(key, r);

            return r;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected IMongoQueryable<T> Set<T>()
        {
            return Collection<T>().AsQueryable();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IMongoCollection<T> Collection<T>()
        {
            return Connection.Collection<T>(CollectionName.For<T>());
        }

        public Task<T> FindAsync<T, TId>(TId id, CancellationToken cancellationToken = default) 
            where T : class, IEntity
        {
            return Collection<T>().Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        }

        public Task AddAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            return Collection<T>().InsertOneAsync(entity, cancellationToken: cancellationToken);
        }

        public Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            return Collection<T>().ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity,
                cancellationToken: cancellationToken);
        }

        public Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) 
            where T : class, IEntity
        {
            return Collection<T>().DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id),
                cancellationToken: cancellationToken);
        }

        public Task DeleteAsync<T, TId>(TId id, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            return Collection<T>().DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, id),
                cancellationToken: cancellationToken);
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
            }
            _disposed = true;
        }

        public Task<TEntity> SingleAsync<TEntity>(ICriteria<TEntity> criteria,
            CancellationToken cancellationToken = default) 
            where TEntity : class, IEntity
        {
            return IAsyncCursorSourceExtensions.SingleOrDefaultAsync(Set<TEntity>()
                    .Apply(criteria), cancellationToken);
        }

        public Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .Apply(spec)
                .ToListAsync(cancellationToken);
        }

        public IAsyncEnumerable<TEntity> StreamAsync<TEntity>(ISpecification<TEntity> spec,
            CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .Apply(spec)
                .ToAsyncEnumerable(cancellationToken); ;
        }
    }
}
