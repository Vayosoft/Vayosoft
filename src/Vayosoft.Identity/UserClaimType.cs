using Vayosoft.Commons.Models;

namespace Vayosoft.Identity
{
    public static class UserClaimType
    {
        public const string UserType = nameof(IUser.Type);
        public const string ProviderId = nameof(IProviderId.ProviderId);
    }
}
