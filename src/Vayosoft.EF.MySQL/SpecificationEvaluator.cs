using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons.Entities;
using Vayosoft.Specifications;

namespace Vayosoft.EF.MySQL
{
    public static class SpecificationExtensions
    {
        public static IQueryable<TEntity> Evaluate<TEntity>(this IQueryable<TEntity> input, ISpecification<TEntity> spec) where TEntity : class, IEntity
        {
            var query = new SpecificationEvaluator<TEntity>().Evaluate(input, spec);
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
