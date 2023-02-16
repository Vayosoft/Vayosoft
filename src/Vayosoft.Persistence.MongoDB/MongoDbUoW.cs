using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence.MongoDB
{
    public sealed class MongoDbUoW : ContextBase, IDocumentUoW
    {
        private readonly IMongoDbContext _context;

        public MongoDbUoW(IServiceProvider serviceProvider, IMongoDbContext context) : base(serviceProvider)
        {
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
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _context.Dispose();
        }
    }
}
