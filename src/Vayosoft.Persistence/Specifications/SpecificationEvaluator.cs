using Vayosoft.Commons.Models;

namespace Vayosoft.Persistence.Specifications
{
    public class SpecificationEvaluator<T> : ISpecificationEvaluator<T> where T : class
    {
        public IQueryable<T> Evaluate(IQueryable<T> input, ISpecification<T> spec)
        {
            var query = input.Where(spec.Criteria.ToExpression());

            if (spec.Sorting != null)
            {
                query = spec.Sorting.SortOrder == SortOrder.Asc
                    ? query.OrderBy(spec.Sorting.Expression)
                    : query.OrderByDescending(spec.Sorting.Expression);
            }

            if (spec.GroupBy != null)
            {
                query = query.GroupBy(spec.GroupBy).SelectMany(x => x);
            }

            return query;
        }
    }
}
