namespace Vayosoft.Web.Model
{
    public class ListResponse<T>
    {
        public IReadOnlyCollection<T> Items { get; }

        public ListResponse(IEnumerable<T> items)
        {
            Items = items as IReadOnlyCollection<T> ?? Array.AsReadOnly(items.ToArray());
        }
    }
}
