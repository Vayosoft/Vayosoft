using System.Linq.Expressions;
using Vayosoft.Commons.Models;
using Vayosoft.Commons.Models.Pagination;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence.Specifications
{
    public interface ISpecification<T> : IPagingModel where T : class
    { 
        ICriteria<T> Criteria { get; }
        Sorting<T> Sorting { get; }
        Expression<Func<T, object>> GroupBy { get; }
    }

    public interface ISpecification<TResult, TSource> : ISpecification<TSource>
        where TResult : class
        where TSource : class
    { }
}
