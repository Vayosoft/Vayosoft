using System.Linq.Expressions;
using Vayosoft.Commons.Entities;

namespace Vayosoft.Persistence
{
    public static class LinqExtensions
    {
        public static IQueryable<T> Where<T>(this ILinqProvider linqProvider, Expression<Func<T, bool>> expr)
            where T : class, IEntity
            => linqProvider.AsQueryable<T>().Where(expr);

        public static IQueryable<T> WhereIf<T>(this ILinqProvider linqProvider, bool cnd, Expression<Func<T, bool>> expr)
            where T : class, IEntity
            => linqProvider.AsQueryable<T>().WhereIf(cnd, expr);

        public static TEntity ById<TEntity>(this ILinqProvider linqProvider, long id)
            where TEntity : class, IEntity<long>
            => linqProvider.AsQueryable<TEntity>().ById(id);
    }
}
