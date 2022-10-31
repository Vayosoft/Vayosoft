namespace Vayosoft.Specifications
{
    public interface IAggregateSpecification<in T>
    {
        bool IsSatisfiedBy(T o);
    }
}