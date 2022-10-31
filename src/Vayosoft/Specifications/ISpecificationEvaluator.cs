using Vayosoft.Commons.Entities;

namespace Vayosoft.Specifications
{
    public interface ISpecificationEvaluator<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> Evaluate(IQueryable<TEntity> input, ISpecification<TEntity> spec);
    }
}
