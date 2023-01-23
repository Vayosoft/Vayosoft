using Vayosoft.Commons.Models.Pagination;

namespace Vayosoft.Web.Model
{
    public class PagedResponse<T>
    {
        public IReadOnlyCollection<T> Items { get; }

        public long TotalItems { get; }

        public long TotalPages { get; }

        public PagedResponse(IPagedEnumerable<T> items, long pageSize)
        {
            Items = Array.AsReadOnly(items.ToArray());

            TotalItems = items.TotalCount;

            TotalPages = (long)Math.Ceiling((double)items.TotalCount / (pageSize > 0 ? pageSize : 1));
        }
    }
}
