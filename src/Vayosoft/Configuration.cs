using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Vayosoft.Commands;
using Vayosoft.Commons.Events;
using Vayosoft.Commons.Events.External;
using Vayosoft.Queries;

namespace Vayosoft
{
    public static class Configuration
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddScoped<ICommandBus, CommandBus>()
                .AddScoped<IQueryBus, QueryBus>();

            services.TryAddScoped<IEventBus, EventBus>();
            services.TryAddScoped<IExternalEventProducer, NullExternalEventProducer>();

            return services;
        }
    }
}
