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
            input = spec.Criteria.Includes.Aggregate(input, (current, include) => current.Include(include));
            return new SpecificationEvaluator<TEntity>().Evaluate(input, spec);
        }

        public static IQueryable<TEntity> ByCriteria<TEntity>(this IQueryable<TEntity> input, ICriteria<TEntity> criteria) where TEntity : class, IEntity
        {
            input = criteria.Includes.Aggregate(input, (current, include) => current.Include(include));
            return input.Where(criteria.ToExpression());
        }
    }
}
