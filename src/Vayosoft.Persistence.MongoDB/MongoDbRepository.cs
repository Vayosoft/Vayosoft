using MongoDB.Driver;
using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence.MongoDB
{
    public abstract class MongoDbRepository<T> : IRepository<T> where T : class, IAggregateRoot
    {
        protected readonly IMongoDbContext Context;
        protected readonly IMongoCollection<T> Collection;

        protected MongoDbRepository(IMongoDbContext context)
        {
            Context = context;
            Collection = context.GetCollection<T>(CollectionName.For<T>());
        }

        public virtual Task<T> FindAsync(object id, CancellationToken cancellationToken = default)
        {
            return Collection.Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            Context.AddCommand(() => 
                Collection.InsertOneAsync(entity, cancellationToken: cancellationToken));
            return Task.CompletedTask;
        }

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            Context.AddCommand(() =>
                Collection.ReplaceOneAsync(e => e.Id.Equals(entity.Id), entity, cancellationToken: cancellationToken));
            return Task.CompletedTask;
        }

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            Context.AddCommand(() =>
                Collection.DeleteOneAsync(e => e.Id.Equals(entity.Id), cancellationToken));
            return Task.CompletedTask;
        }
    }
}
