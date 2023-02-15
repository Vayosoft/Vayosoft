using MongoDB.Driver;

namespace Vayosoft.Persistence.MongoDB
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDbConnection _connection;

        private readonly List<Func<Task>> _commands = new();

        public IClientSessionHandle Session { get; private set; }

        public MongoDbContext(IMongoDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            using (Session = await _connection.StartSessionAsync(cancellationToken: cancellationToken))
            {
                Session.StartTransaction();

                var commandTasks = _commands.Select(c => c());

                await Task.WhenAll(commandTasks);

                await Session.CommitTransactionAsync(cancellationToken);
            }

            return _commands.Count;
        }

        public void AddCommand(Func<Task> func)
        {
            _commands.Add(func);
        }

        public IMongoCollection<T> GetCollection<T>(CollectionName collectionName = null)
        {
            return _connection.Collection<T>(collectionName);
        }

        public void Dispose()
        {
            Session?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
