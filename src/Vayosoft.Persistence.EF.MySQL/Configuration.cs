using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Vayosoft.Persistence.EF.MySQL
{
    public static class Configuration
    {
        public static IServiceCollection AddMySqlContext<TContext>(this IServiceCollection services, IConfiguration configuration) where TContext : DbContext
        {
            var connectionString = configuration["EFContext:ConnectionString"]
                ?? configuration.GetConnectionString("DefaultConnection");

            // Replace with your server version and type.
            // Use 'MariaDbServerVersion' for MariaDB.
            // Alternatively, use 'ServerVersion.AutoDetect(connectionString)'.
            // For common usages, see pull request #1233.
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));

            // Replace 'YourDbContext' with the name of your own DbContext derived class.
            services.AddDbContext<TContext>((serviceProvider, dbContextOptions) =>
            {
                dbContextOptions
                    .UseMySql(connectionString, serverVersion)
#if DEBUG
                    .EnableSensitiveDataLogging() // <-- These two calls are optional but help
                    .EnableDetailedErrors() // <-- with debugging (remove for production).
#endif
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());
            });


            return services;
        }
    }
}
