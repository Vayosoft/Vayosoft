using System.Linq.Expressions;
using Vayosoft.Commons.Entities;

namespace Vayosoft.Specifications
{
    public interface ICriteriaSpecification<TEntity, TDto> :
        ICriteriaSpecification<TEntity> where TEntity : class, IEntity
    { }

    public interface ICriteriaSpecification<TEntity> where TEntity : class, IEntity
    {
        Expression<Func<TEntity, bool>> Criteria { get; }
    }
}
