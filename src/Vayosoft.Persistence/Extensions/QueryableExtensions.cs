using System.Linq.Expressions;
using Vayosoft.Commons;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, bool cnd, Expression<Func<T, bool>> expr)
            => cnd
                ? queryable.Where(expr)
                : queryable;

        public static IQueryable<T> BySpecification<T>(this IQueryable<T> queryable, ISpecification<T> spec)
            where T : class, IEntity
            => new SpecificationEvaluator<T>().Evaluate(queryable, spec);

        public static IQueryable<TEntity> ByCriteria<TEntity>(this IQueryable<TEntity> queryable, ICriteria<TEntity> criteria) where TEntity : class, IEntity
        {
            return queryable.Where(criteria.ToExpression());
        }

        public static IQueryable<T> Apply<T>(this IQueryable<T> source, ILinqSpecification<T> spec)
            where T : class
            => spec.Apply(source);

        public static IQueryable<T> ApplyIfPossible<T>(this IQueryable<T> source, object spec)
            where T : class
            => spec is ILinqSpecification<T> specification
                ? specification.Apply(source)
                : source;

        public static IQueryable<TDest> Project<TSource, TDest>(this IQueryable<TSource> source, IProjector projector)
            => projector.Project<TSource, TDest>(source);

        public static TEntity ById<TEntity>(this IQueryable<TEntity> queryable, long id)
            where TEntity : class, IEntity<long>
            => queryable.SingleOrDefault(x => x.Id == id);
    }
}
