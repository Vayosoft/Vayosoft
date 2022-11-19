using System.Security.Claims;

namespace Vayosoft.Identity.Extensions
{
    public static class PrincipalExtensions
    {
        public static bool HasAnyRole(this ClaimsPrincipal principal, IList<string> roles)
        {
            return roles.Any(principal.IsInRole);
        }
    }
}
