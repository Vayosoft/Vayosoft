using System.Security.Principal;
using Vayosoft.Identity.Security;

namespace Vayosoft.Identity
{
    public interface IUserContext
    {
        IPrincipal User { get; }
        IUserSession Session { get; }

        Task<bool> LoadContextAsync();

        bool HasRole(string role);
        bool HasAnyRole(IEnumerable<string> roles);
        bool HasPermission(string objName, SecurityPermissions requiredPermissions);

        bool IsSupervisor { get; }
        bool IsAdministrator { get; }
    }
}
