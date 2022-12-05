using System.Security.Principal;
using Vayosoft.Identity.Security;

namespace Vayosoft.Identity.Tokens
{
    public interface ITokenService<out TPrincipal> where TPrincipal : IPrincipal
    {
        public string GenerateToken(IUser user, IEnumerable<SecurityRoleEntity> roles);
        public TPrincipal GetPrincipalFromToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
