using MongoDB.Driver.Linq;
using Vayosoft.Commons.Models;
using Vayosoft.Persistence.Criterias;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.Persistence.MongoDB.Extensions
{
    public static class SpecificationExtensions
    {
        public static IMongoQueryable<T> Apply<T>(this IMongoQueryable<T> input, ISpecification<T> spec)
            where T : class
        {
            var query = input.Where(spec.Criteria.ToExpression());
            if (spec.Sorting != null)
            {
                query = spec.Sorting.SortOrder == SortOrder.Asc
                    ? query.OrderBy(spec.Sorting.Expression)
                    : query.OrderByDescending(spec.Sorting.Expression);
            }

            return query;
        }

        public static IMongoQueryable<T> Apply<T>(this IMongoQueryable<T> input, ICriteria<T> criteria)
            where T : class
        {
            return input.Where(criteria.ToExpression());
        }
    }
}
