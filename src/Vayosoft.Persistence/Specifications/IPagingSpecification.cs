using Vayosoft.Commons.Models.Pagination;

namespace Vayosoft.Persistence.Specifications
{
    public interface IPagingSpecification<T> : IPagingModel, ILinqSpecification<T> where T : class
    { }
}
