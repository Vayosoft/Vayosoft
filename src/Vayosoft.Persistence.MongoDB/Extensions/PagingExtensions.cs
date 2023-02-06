using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Vayosoft.Commons.Models.Pagination;

namespace Vayosoft.Persistence.MongoDB.Extensions
{
    public static class PagingExtensions
    {
        public static IMongoQueryable<T> Paginate<T>(this IMongoQueryable<T> queryable, IPagingModel pagingModel)
            => Paginate(queryable, pagingModel.Page, pagingModel.PageSize);

        public static IMongoQueryable<T> Paginate<T>(this IMongoQueryable<T> queryable, int page, int pageSize)
            => queryable.Skip((page - 1) * pageSize).Take(pageSize);

        public static Task<PagedList<T>> ToPagedListAsync<T>(this IMongoQueryable<T> queryable,
            IPagingModel pagingModel, CancellationToken cancellationToken = default)
        {
            return queryable.ToPagedListAsync(pagingModel.Page, pagingModel.PageSize, cancellationToken);
        }

        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IMongoQueryable<T> queryable,
            int page = 1, int pageSize = IPagingModel.DefaultSize, CancellationToken cancellationToken = default)
        {
            var list = queryable.Paginate(page, pageSize).ToListAsync(cancellationToken: cancellationToken);
            var count = queryable.CountAsync(cancellationToken: cancellationToken);
            await Task.WhenAll(list, count);

            return From(await list, await count);
        }

        public static IFindFluent<TDocument, TProjection> Paginate<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> query,
            IPagingModel pagingModel) => Paginate(query, pagingModel.Page, pagingModel.PageSize);

        public static IFindFluent<TDocument, TProjection> Paginate<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> query,
            int page, int pageSize) => query.Skip((page - 1) * pageSize).Limit(pageSize);

        public static async Task<PagedList<TProjection>> ToPagedListAsync<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> query,
            int page = 1, int pageSize = IPagingModel.DefaultSize, CancellationToken cancellationToken = default)
        {
            var list = query.Paginate(page, pageSize).ToListAsync(cancellationToken);
            var count = query.CountDocumentsAsync(cancellationToken: cancellationToken);
            await Task.WhenAll(list, count);

            return From(await list, await count);
        }
        public static PagedList<T> From<T>(IEnumerable<T> inner, long totalCount) => 
            new(inner, totalCount);
    }
}
