using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.EF.MySQL
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> BySpecification<TEntity>(this IQueryable<TEntity> input, ISpecification<TEntity> spec) where TEntity : class, IEntity
        {
            return new SpecificationEvaluator<TEntity>()
                .Evaluate(input.UseIncludes(spec.Criteria), spec);
        }

        public static IQueryable<TEntity> ByCriteria<TEntity>(this IQueryable<TEntity> input, ICriteria<TEntity> criteria) where TEntity : class, IEntity
        {
            return input.UseIncludes(criteria).Where(criteria.ToExpression());
        }

        private static IQueryable<TEntity> UseIncludes<TEntity>(this IQueryable<TEntity> input, ICriteria<TEntity> criteria) where TEntity : class, IEntity
        {
            return !criteria.Includes.Any()
                ? input
                : criteria.Includes.Aggregate(input.AsSplitQuery(),
                    (current, include) => current.Include(include));
        }
    }
}
