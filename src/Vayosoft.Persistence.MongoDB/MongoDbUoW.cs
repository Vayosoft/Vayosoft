namespace Vayosoft.Persistence.MongoDB
{
    public class MongoDbUoW : IDocumentUoW
    {
        private readonly IMongoDbContext _context;
        private bool _disposed;

        public MongoDbUoW(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
        {
            return (await _context.SaveChangesAsync(cancellationToken)) > 0;
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
                _context.Dispose();
            }
            _disposed = true;
        }
    }
}
