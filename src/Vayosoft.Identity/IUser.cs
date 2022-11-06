using System.Text.Json.Serialization;
using Vayosoft.Commons.Entities;

namespace Vayosoft.Identity
{
    public interface IUser : IEntity
    {
        public string Username { get; }
        public string Email { get; }
        public UserType Type { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; }
    }
}
