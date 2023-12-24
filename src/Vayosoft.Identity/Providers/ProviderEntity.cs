using Vayosoft.Commons.Entities;

namespace Vayosoft.Identity.Providers
{
    public class ProviderEntity : EntityBase<long>
    {
        public long? Parent { get; set; }
    }
}
