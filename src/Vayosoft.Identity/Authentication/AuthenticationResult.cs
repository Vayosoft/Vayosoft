using System.Text.Json.Serialization;
using Vayosoft.Identity.Security;

namespace Vayosoft.Identity.Authentication
{
    public class AuthenticationResult
    {
        public IUser User { get; set; }
        [JsonIgnore]
        public IReadOnlyCollection<RoleDTO> Roles { get; set; }
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticationResult(IUser user, string jwtToken, string refreshToken, DateTime expirationTime)
        {
            User = user;
            Token = jwtToken;
            TokenExpirationTime = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds();
            RefreshToken = refreshToken;
        }
    }
}
