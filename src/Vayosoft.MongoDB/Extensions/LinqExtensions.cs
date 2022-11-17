using MongoDB.Driver.Linq;
using Vayosoft.Persistence.Specifications;

namespace Vayosoft.MongoDB.Extensions
{
    public static class LinqExtensions
    {
        public static IMongoQueryable<T> Apply<T>(this IMongoQueryable<T> source, ILinqSpecification<T> spec)
            where T : class
            => (IMongoQueryable<T>)spec.Apply(source);
    }
}
