using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence.MongoDB
{
    public class GenericRepository<T> : IRepository<T> where T : class, IAggregateRoot
    {
        protected readonly IMongoDbConnection Connection;

        protected readonly IMongoCollection<T> Collection;

        public GenericRepository(IMongoDbConnection connection)
        {
            Connection = connection;
            Collection = connection.Collection<T>(CollectionName.For<T>());
        }

        protected IMongoQueryable<TEntity> Set<TEntity>() => 
            Connection.Collection<TEntity>(CollectionName.For<TEntity>()).AsQueryable();

        protected virtual IQueryable<T> AsQueryable()
            => Collection.AsQueryable();

        public virtual Task<T> FindAsync(object id, CancellationToken cancellationToken = default) =>
            Collection.Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);

        public Task<T> FindAsync(ICriteria<T> criteria, CancellationToken cancellationToken = default)
        {
            return Collection.Find(criteria.ToExpression()).SingleOrDefaultAsync(cancellationToken);
        }

        public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), cancellationToken: cancellationToken);
        public virtual Task DeleteAsync(object id, CancellationToken cancellationToken = default) =>
            Collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, id), cancellationToken: cancellationToken);
    }
}
