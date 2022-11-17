using MongoDB.Driver.Linq;
using Vayosoft.Commons.Models;
using Vayosoft.Specifications;

namespace Vayosoft.MongoDB.Extensions
{
    public static class SpecificationExtensions
    {
        public static IMongoQueryable<T> Apply<T>(this IMongoQueryable<T> input, ISpecification<T> spec)
            where T : class
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
