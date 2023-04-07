namespace Vayosoft.Commons.Models.Pagination
{
    public interface IPagingModel
    {
        int Page { get; }

        int PageSize { get; }

        const int DefaultSize = 30;
    }

    public interface IPagingModel<TEntity, TSortKey> : IPagingModel
        where TEntity : class
    {
        Sorting<TEntity, TSortKey> OrderBy { get; }
    }
}