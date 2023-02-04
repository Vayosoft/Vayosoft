using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.MongoDB.Extensions;
using Vayosoft.Persistence.Specifications;

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

        public IQueryable<T> AsQueryable()
            => Collection.AsQueryable();

        public Task<T> FindAsync(object id, CancellationToken cancellationToken = default) =>
            Collection.Find(q => q.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);


        public virtual Task AddAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

        public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), entity, cancellationToken: cancellationToken);

        public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default) =>
            Collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, entity.Id), cancellationToken: cancellationToken);
        public virtual Task DeleteAsync(object id, CancellationToken cancellationToken = default) =>
            Collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, id), cancellationToken: cancellationToken);


        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) =>
            Collection.Find(criteria).FirstOrDefaultAsync(cancellationToken);

        public Task<T> FirstOrDefaultAsync(ILinqSpecification<T> spec, CancellationToken cancellationToken = default) =>
            Collection.AsQueryable().Apply(spec).FirstOrDefaultAsync(cancellationToken);


        public Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> criteria, CancellationToken cancellationToken = default) =>
            Collection.Find(criteria).SingleOrDefaultAsync(cancellationToken);


        public Task<List<T>> ListAsync(ISpecification<T> spec, CancellationToken cancellationToken = default) {
            return Collection.AsQueryable().Apply(spec).ToListAsync(cancellationToken);
        }
        
        public IAsyncEnumerable<T> StreamAsync(ISpecification<T> spec, CancellationToken cancellationToken = default) {
            return Collection.AsQueryable().Apply(spec).ToAsyncEnumerable(cancellationToken);
        }

        public Task<PagedList<T>> PagedListAsync(int page, int pageSize, ISpecification<T> spec, CancellationToken cancellationToken = default) {
            return Collection.AsQueryable().Apply(spec).ToPagedListAsync(page, pageSize, cancellationToken: cancellationToken);
        }

        public Task<IPagedEnumerable<T>> PagedListAsync(Expression<Func<T, bool>> criteria, IPagingModel<T, object> model, CancellationToken cancellationToken = default) =>
            Collection.AggregateByPage(model, Builders<T>.Filter.Where(criteria), cancellationToken);
    }
}
