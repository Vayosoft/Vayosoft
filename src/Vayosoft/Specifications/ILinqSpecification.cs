namespace Vayosoft.Specifications
{
    public interface ILinqSpecification<T>
        where T : class
    {
        IQueryable<T> Apply(IQueryable<T> query);
    }
}
