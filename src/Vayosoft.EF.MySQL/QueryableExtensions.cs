using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.EF.MySQL
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> Evaluate<TEntity>(this IQueryable<TEntity> input, ISpecification<TEntity> spec) where TEntity : class, IEntity
        {
            input = spec.Criteria.Includes.Aggregate(input, (current, include) => current.Include(include));
            return new SpecificationEvaluator<TEntity>().Evaluate(input, spec);
        }
    }
}
