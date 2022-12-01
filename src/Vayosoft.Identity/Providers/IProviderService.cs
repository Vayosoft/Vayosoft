using Vayosoft.Identity.Security.Models;

namespace Vayosoft.Identity.Providers
{
    public interface IProviderService
    {
        public Task Send(string notification);

        Task<UserSubscription> GetUserSubscription(UserEntity user);
    }
}
