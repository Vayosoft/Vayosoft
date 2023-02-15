using MongoDB.Driver;

namespace Vayosoft.Persistence.MongoDB
{
    public interface IMongoDbContext : IDisposable
    {
        void AddCommand(Func<Task> command);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        IMongoCollection<T> GetCollection<T>(CollectionName collectionName = null);
    }
}
