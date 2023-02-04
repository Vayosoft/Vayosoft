using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.MongoDB.Extensions;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.MongoDB
{
    public class GenericDao : IDAO
    {
        protected readonly IMongoDbConnection Connection;
        protected readonly IServiceScope Scope;
    
        public GenericDao(IMongoDbConnection connection, IServiceProvider serviceProvider)
        {
            Connection = connection;
            Scope = serviceProvider.CreateScope();
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

        public Task<T> FindAsync<T>(ICriteria<T> criteria,
            CancellationToken cancellationToken = default) where T : class, IEntity
        {
            return IAsyncCursorSourceExtensions.SingleOrDefaultAsync(Set<T>()
                .Apply(criteria), cancellationToken);
        }

        public Task CreateAsync<T>(T entity, CancellationToken cancellationToken = default)
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

        public Task<List<TEntity>> ListAsync<TEntity>(ISpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .Apply(spec)
                .ToListAsync(cancellationToken);
        }

        public IAsyncEnumerable<TEntity> StreamAsync<TEntity>(ISpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .Apply(spec)
                .ToAsyncEnumerable(cancellationToken);
        }

        public Task<PagedList<TEntity>> PagedListAsync<TEntity>(int page, int pageSize, ISpecification<TEntity> spec,
            CancellationToken cancellationToken)
            where TEntity : class, IEntity
        {
            return Set<TEntity>()
                .Apply(spec)
                .ToPagedListAsync(page, pageSize, cancellationToken);
        }
    }
}
