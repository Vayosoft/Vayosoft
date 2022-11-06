namespace Vayosoft.Identity
{
    public interface IProviderable
    {
        object ProviderId { get; }
    }

    public interface IProviderable<out TKey> : IProviderable
    {
        new TKey ProviderId { get; }
    }
}
