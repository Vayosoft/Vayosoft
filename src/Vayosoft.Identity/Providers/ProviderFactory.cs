using Vayosoft.Identity.Providers.Default;
using Vayosoft.Identity.Security.Models;

namespace Vayosoft.Identity.Providers
{
    public class ProviderFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ProviderFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IProviderService GetProviderService(Provider provider)
        {
            return provider switch
            {
                _ => (IProviderService)serviceProvider.GetService(typeof(DefaultProviderService))!
            };
        }

        public IProviderService GetProviderService(string providerName)
        {
            return GetProviderService((Provider) providerName);
        }
    }
}
