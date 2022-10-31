namespace Vayosoft.Utilities
{
    public interface IIdentityGenerator : IIdentityGenerator<Guid> { }

    public interface IIdentityGenerator<out T>
    {
        T New();
    }
}
