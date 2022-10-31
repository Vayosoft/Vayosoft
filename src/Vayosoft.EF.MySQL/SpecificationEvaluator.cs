using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons.Entities;
using Vayosoft.Specifications;

namespace Vayosoft.EF.MySQL
{
    public class SpecificationEvaluator<TEntity> : Specifications.SpecificationEvaluator<TEntity> where TEntity : class, IEntity
    {
        public new IQueryable<TEntity> Evaluate(IQueryable<TEntity> input, ISpecification<TEntity> spec)
        {
            var query = base.Evaluate(input, spec);
            query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
            query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            return query;
        }
    }
}
