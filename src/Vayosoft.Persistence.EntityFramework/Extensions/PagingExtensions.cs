using Microsoft.EntityFrameworkCore;
using Vayosoft.Commons.Models.Pagination;

namespace Vayosoft.Persistence.EntityFramework.Extensions
{
    public static class PagingExtensions
    {
        public static Task<IPagedEnumerable<T>> ToPagedEnumerableAsync<T>(this IQueryable<T> queryable,
            IPagingModel pagingModel, CancellationToken cancellationToken)
            where T : class
            => queryable.ToPagedEnumerableAsync(pagingModel.Page, pagingModel.PageSize, cancellationToken);

        public static async Task<IPagedEnumerable<T>> ToPagedEnumerableAsync<T>(this IQueryable<T> queryable,
            int page, int pageSize, CancellationToken cancellationToken)
            where T : class
        {
            var list = queryable.Paginate(page, pageSize).ToArrayAsync(cancellationToken);
            var count = queryable.CountAsync(cancellationToken: cancellationToken);
            await Task.WhenAll(list, count);

            return From(await list, await count);
        }

        public static IPagedEnumerable<T> From<T>(IEnumerable<T> inner, int totalCount)
            => new PagedList<T>(inner, totalCount);
    }
}
