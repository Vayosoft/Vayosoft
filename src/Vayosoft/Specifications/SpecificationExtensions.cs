using Vayosoft.Commons.Entities;

namespace Vayosoft.Specifications
{
    public static class SpecificationExtensions
    {
        public static IQueryable<TEntity> Evaluate<TEntity>(this IQueryable<TEntity> input, ISpecification<TEntity> spec) where TEntity : class, IEntity
        {
            return new SpecificationEvaluator<TEntity>().Evaluate(input, spec);
        }
    }
}
