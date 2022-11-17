using System.Linq.Expressions;

namespace Vayosoft.Persistence.Specifications
{
    public interface ILinqSpecification<T>
        where T : class
    {
        IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }

        IQueryable<T> Apply(IQueryable<T> query);
    }
}
