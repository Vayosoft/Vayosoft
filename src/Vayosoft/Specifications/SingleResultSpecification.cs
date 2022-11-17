using System.Linq.Expressions;
using Vayosoft.Commons.Entities;
using Vayosoft.Utilities;

namespace Vayosoft.Specifications
{
    public class CriteriaSpecification<TEntity> : ICriteriaSpecification<TEntity> where TEntity : class, IEntity
    {
        public CriteriaSpecification() : this(criteria => true)
        { }

        public CriteriaSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = Guard.NotNull(criteria);
        }

        public Expression<Func<TEntity, bool>> Criteria { get; protected set; }
    }

    public class CriteriaSpecification<TEntity, TDto> : CriteriaSpecification<TEntity>, ICriteriaSpecification<TEntity, TDto> where TEntity : class, IEntity
    { }
}
