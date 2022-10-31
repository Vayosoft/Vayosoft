using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vayosoft.Commands;

namespace Vayosoft.RestClient.Commands
{
    public static class Configuration
    {
        public static IServiceCollection AddCommandBusService(this IServiceCollection services)
        {
            services.TryAddScoped<IExternalCommandBus, ExternalCommandBus>();

            return services;
        }
    }
}
