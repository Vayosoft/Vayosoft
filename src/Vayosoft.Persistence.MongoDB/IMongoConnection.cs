using MongoDB.Driver;
using Vayosoft.Commons.Entities;

namespace Vayosoft.Persistence.MongoDB
{
    public interface IMongoConnection
    {
        IMongoDatabase Database { get; }
        IClientSessionHandle Session { get; }

        Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken = default);

        IMongoDatabase GetDatabase(string db);

        IMongoCollection<T> Collection<T>(CollectionName collectionName = null) where T : IEntity;
    }
}
