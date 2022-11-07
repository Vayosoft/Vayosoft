using System.Text.Json.Serialization;
using Vayosoft.Commons.Entities;
using Vayosoft.Identity.Tokens;

namespace Vayosoft.Identity
{
    public interface IUser : IEntity
    {
        public string Username { get; }
        [JsonIgnore]
        public string PasswordHash { get; set; }
        public string Email { get; }
        public UserType Type { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; }
    }
}
