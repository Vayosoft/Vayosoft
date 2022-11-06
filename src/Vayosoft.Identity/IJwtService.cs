using System.Security.Principal;

namespace Vayosoft.Identity
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IUser user, IEnumerable<SecurityRoleEntity> roles);
        public IPrincipal GetPrincipalFromJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
