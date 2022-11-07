namespace Vayosoft.Identity
{
    public interface IUserProvider
    {
        object ProviderId { get; }
    }

    public interface IUserProvider<out TKey> : IUserProvider
    {
        new TKey ProviderId { get; }
    }
}
