using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Models;

namespace Vayosoft.Specifications
{
    public class SpecificationEvaluator<TEntity> : ISpecificationEvaluator<TEntity> where TEntity : class, IEntity
    {
        public IQueryable<TEntity> Evaluate(IQueryable<TEntity> input, ISpecification<TEntity> spec)
        {
            var query = input.Where(spec.ToExpression());
            if (spec.Sorting != null)
            {
                query = spec.Sorting.SortOrder == SortOrder.Asc
                    ? query.OrderBy(spec.Sorting.Expression)
                    : query.OrderByDescending(spec.Sorting.Expression);
            }

            return query;
        }
    }
}
