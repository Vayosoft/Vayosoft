using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Exceptions;
using Vayosoft.Persistence;
using Vayosoft.Specifications;


namespace Vayosoft.Linq2db.MySQL
{
    public class DataContext : DataConnection, ILinqProvider, IUnitOfWork
    {
        public DataContext(LinqToDBConnectionOptions<DataContext> options)
            : base(options)
        { }

        public IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class, IEntity {
            return this.GetTable<TEntity>().AsQueryable();
        }

        public IQueryable<TEntity> AsQueryable<TEntity>(ISpecification<TEntity> specification) where TEntity : class, IEntity {
            return new SpecificationEvaluator<TEntity>().Evaluate(AsQueryable<TEntity>(), specification);
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
           this.GetTable<TEntity>().Insert(() => entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            this.GetTable<TEntity>().Update(obj => entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            this.GetTable<TEntity>().Delete(obj => obj.Id == entity.Id);
        }

        public TEntity Find<TEntity>(object id) where TEntity : class, IEntity
        {
            return this.GetTable<TEntity>().SingleOrDefault(x => x.Id == id);
        }

        public TEntity Get<TEntity>(object id) where TEntity : class, IEntity
        {
            var entity = Find<TEntity>(id);
            if (entity == null)
                throw EntityNotFoundException.For<TEntity>(id);

            return entity;
        }

        public Task<TEntity> FindAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class, IEntity
        {
            return this.GetTable<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<TEntity> GetAsync<TEntity>(object id, CancellationToken cancellationToken) where TEntity : class, IEntity
        {
            var entity = await FindAsync<TEntity>(id, cancellationToken);
            if (entity == null)
                throw EntityNotFoundException.For<TEntity>(id);

            return entity;
        }

        public void Commit() => this.CommitTransaction();
        public Task CommitAsync() => this.CommitTransactionAsync();
    }
}
