using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Queries;
using Vayosoft.Queries;

namespace Vayosoft.Persistence.EntityFramework.Queries;

public class SingleQueryHandler<TKey, TEntity, TResult> : IQueryHandler<SingleQuery<TResult>, TResult>
    where TKey : struct, IComparable, IComparable<TKey>, IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
    where TResult : IEntity<TKey>
{
    protected readonly ILinqProvider LinqProvider;

    protected readonly IProjector Projector;

    public SingleQueryHandler(ILinqProvider linqProvider, IProjector projector)
    {
        LinqProvider = linqProvider;
        Projector = projector;
    }

    public virtual Task<TResult> Handle(SingleQuery<TResult> requiest, CancellationToken cancellationToken)
    {
        return Projector.Project<TEntity, TResult>(LinqProvider
                .AsQueryable<TEntity>()
                .Where(x => requiest.Id.Equals(x.Id)))
            .SingleOrDefaultAsync(cancellationToken: cancellationToken);
    }
}
