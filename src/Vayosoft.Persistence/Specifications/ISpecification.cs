using System.Linq.Expressions;
using Vayosoft.Commons.Models;
using Vayosoft.Persistence.Criterias;

namespace Vayosoft.Persistence.Specifications
{
    public interface ISpecification<T> where T : class
    {
        ICriteria<T> Criteria { get; }
        Sorting<T> Sorting { get; }
    }
}
