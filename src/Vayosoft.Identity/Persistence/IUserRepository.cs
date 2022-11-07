
namespace Vayosoft.Identity.Persistence
{
    public interface IUserRepository<T> : IUserStore<T>, IUserRoleStore where T : IUser
    { }
}
