namespace Vayosoft.Persistence.Specifications
{
    public interface ILinqSpecification<T>
        where T : class
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
