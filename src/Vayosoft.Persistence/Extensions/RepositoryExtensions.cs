using MediatR;
using Vayosoft.Commons.Aggregates;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Exceptions;

namespace Vayosoft.Persistence.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task<T> GetAsync<T, TId>(this IRepository<T> repository, TId id, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            var entity = await repository.FindAsync(id, cancellationToken);

            return entity ?? throw EntityNotFoundException.For<T>(id);
        }

        public static async Task<Unit> GetAndUpdateAsync<T, TId>(this IRepository<T> repository, TId id, Action<T> action, CancellationToken cancellationToken = default)
            where T : class, IAggregateRoot
        {
            var entity = await repository.GetAsync(id, cancellationToken);
            action(entity);
            await repository.UpdateAsync(entity, cancellationToken);

            return Unit.Value;
        }

        public static async Task<T> GetAsync<T, TId>(this IUnitOfWork unitOfWork, TId id, CancellationToken cancellationToken = default)
            where T : class, IEntity
        {
            var entity = await unitOfWork.FindAsync<T>(id, cancellationToken);
            if (entity == null)
                throw EntityNotFoundException.For<T>(id);

            return entity;
        }
    }
}
