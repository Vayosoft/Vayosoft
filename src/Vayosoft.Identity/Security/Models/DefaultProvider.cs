using System.Globalization;
using System.Text.Json.Serialization;

namespace Vayosoft.Identity.Security.Models
{
    public abstract partial class Provider
    {
        private class DefaultProvider :Provider
        {
            public DefaultProvider(int id, string name) : base(id, name) { }
            [JsonIgnore]
            public override CultureInfo Culture
                => CultureInfo.GetCultureInfo("en");
        }
    }
}
