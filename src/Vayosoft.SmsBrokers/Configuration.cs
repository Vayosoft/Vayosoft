using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.SmsBrokers
{
    public static class Configuration
    {
        public static IServiceCollection AddSmsBrokers(this IServiceCollection services)
        {
            services.AddSingleton<SmsBrokerFactory>();
            services.AddSingleton<DiafanSmsBroker>()
                .AddSingleton<ISmsBroker, DiafanSmsBroker>(s => s.GetService<DiafanSmsBroker>()!);

            return services;
        }
    }
}
