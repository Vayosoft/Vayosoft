using MongoDB.Driver;

namespace Vayosoft.Persistence.MongoDB
{
    public interface IMongoDbConnection
    {
        IMongoDatabase Database { get; }

        Task<IClientSessionHandle> StartSessionAsync(CancellationToken cancellationToken = default);

        IMongoDatabase GetDatabase(string db);

        IMongoCollection<T> Collection<T>(CollectionName collectionName = null);
    }
}
