using System.Linq.Expressions;
using Vayosoft.Commons.Models;

namespace Vayosoft.Specifications
{
    public interface ISpecification<TEntity> where TEntity : class
    {
        bool IsSatisfiedBy(TEntity entity);
        Expression<Func<TEntity, bool>> ToExpression();
        IReadOnlyCollection<Expression<Func<TEntity, object>>> Includes { get; }
        Sorting<TEntity> Sorting { get; }
    }
}
