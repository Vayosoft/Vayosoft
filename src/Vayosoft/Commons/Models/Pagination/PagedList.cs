using System.Collections;

namespace Vayosoft.Commons.Models.Pagination
{
    public class PagedList<T> : IPagedEnumerable<T>
    {
        private readonly IEnumerable<T> _inner;

        public PagedList(IEnumerable<T> inner, long totalCount)
        {
            _inner = inner;

            TotalCount = totalCount;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public long TotalCount { get; }
    }
}
