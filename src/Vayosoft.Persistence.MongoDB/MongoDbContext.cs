using MongoDB.Driver;

namespace Vayosoft.Persistence.MongoDB
{
    public abstract class MongoDbContext : IDisposable
    {
        private bool _disposed;
        protected readonly IMongoDbConnection Connection;

        protected IClientSessionHandle Session { get; private set; }

        protected MongoDbContext(IMongoDbConnection connection)
        {
            Connection = connection;
        }

        public async Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken)
        {
            return Session = await Connection.StartSessionAsync(cancellationToken: cancellationToken);
        }

        public IMongoCollection<T> GetCollection<T>(CollectionName collectionName = null)
        {
            return Connection.Collection<T>(collectionName);
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
                Session?.Dispose();
            }
            _disposed = true;
        }
    }
}
