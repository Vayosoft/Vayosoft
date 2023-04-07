using System.Linq.Expressions;

namespace Vayosoft.Persistence.Criterias
{
    public interface ICriteria<T> where T : class
    {
        bool IsSatisfiedBy(T entity);
        Expression<Func<T, bool>> ToExpression();
        IReadOnlyCollection<Expression<Func<T, object>>> Includes { get; }
    }
}
