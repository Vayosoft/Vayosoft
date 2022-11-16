using LinqToDB.AspNet;
using LinqToDB.AspNet.Logging;
using LinqToDB.Configuration;
using LinqToDB.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Linq2db.MySQL
{
    public static class Configuration
    {
        public static IServiceCollection AddMySqlContext<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DataConnection
        {
            var connectionString = configuration["Linq2DbContext:ConnectionString"]
                                   ?? configuration.GetConnectionString("DefaultConnection")!;

            services.AddLinqToDBContext<TContext>((provider, options) =>
            {
                options
                    //will configure the AppDataConnection to use
                    //SqlServer with the provided connection string
                    //there are methods for each supported database
                    .UseSqlServer(connectionString)

                    //default logging will log everything using
                    //an ILoggerFactory configured in the provider
                    .UseDefaultLogging(provider);
            });


            return services;
        }
    }
}