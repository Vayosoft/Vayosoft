namespace Vayosoft.Persistence.Specifications
{
    public interface ISpecificationEvaluator<T> where T : class
    {
        IQueryable<T> Evaluate(IQueryable<T> input, ISpecification<T> spec);
    }
}
