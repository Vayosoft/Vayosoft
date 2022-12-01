using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Vayosoft.Commons.Entities;
using Vayosoft.Commons.Enums;
using Vayosoft.Mapping;

namespace Vayosoft.Identity.Security.Models
{
    [ConventionalMap(typeof(UserEntity), direction: MapDirection.EntityToDto)]
    public class UserEntityDto : IEntity<long>
    {
        object IEntity.Id => Id;
        public long Id { get; set; }
        [Required]
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserType Type { get; set; }
        public DateTime? Registered { get; set; }
        public DateTime? Deregistered { get; set; }
        public string CultureId { get; set; }
        public long ProviderId { get; set; }
        public LogEventType? LogLevel { get; set; }
    }
}
