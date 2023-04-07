using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Exceptions;

namespace Vayosoft.Persistence.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<T> GetAsync<T>(this IRepository<T> repository, object id, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot =>
            await repository.FindAsync(id, cancellationToken) ?? throw AggregateNotFoundException.For<T>(id);

        public static async Task GetAndUpdateAsync<T>(this IRepository<T> repository, object id, Action<T> action, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            var entity = await repository.GetAsync(id, cancellationToken);
            action(entity);
            await repository.UpdateAsync(entity, cancellationToken);
        }
    }
}
