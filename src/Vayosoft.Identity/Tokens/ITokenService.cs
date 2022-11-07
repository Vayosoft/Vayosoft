using System.Security.Principal;
using Vayosoft.Identity.Security;

namespace Vayosoft.Identity.Tokens
{
    public interface ITokenService
    {
        public string GenerateToken(IUser user, IEnumerable<SecurityRoleEntity> roles);
        public IPrincipal GetPrincipalFromToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
