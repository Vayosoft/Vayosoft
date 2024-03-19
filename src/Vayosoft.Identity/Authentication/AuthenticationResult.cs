using System.Text.Json.Serialization;
using Vayosoft.Identity.Security;

namespace Vayosoft.Identity.Authentication
{
    public class AuthenticationResult(IUser user, string jwtToken, string refreshToken, DateTime expirationTime)
    {
        public IUser User { get; set; } = user;

        [JsonIgnore]
        public IReadOnlyCollection<RoleDTO> Roles { get; set; }
        public string Token { get; set; } = jwtToken;
        public long TokenExpirationTime { get; set; } = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds();

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; } = refreshToken;
    }
}
