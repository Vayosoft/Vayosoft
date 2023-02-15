namespace Vayosoft.Persistence.MongoDB
{
    public class MongoDbUoW : IDocumentUoW
    {
        private readonly IMongoDbContext _context;

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
            _context.Dispose();
        }
    }
}
