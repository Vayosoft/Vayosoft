using Microsoft.Extensions.Configuration;

namespace Vayosoft.Persistence.MongoDB
{
    public class ConnectionSetting
    {
        public ReplicaSetSetting ReplicaSet { set; get; }

        public string ConnectionString { set; get; }
    }

    public class ReplicaSetSetting
    {
        public string[] BootstrapServers { set; get; }
    }

    public static class ConnectionSettingExtensions
    {
        public static ConnectionSetting GetConnectionSetting(this IConfiguration configuration)
        {
            var settings =  configuration.GetSection(nameof(MongoConnection)).Get<ConnectionSetting>() ?? new ConnectionSetting();
            if(string.IsNullOrEmpty(settings.ConnectionString))
            {
                settings.ConnectionString = configuration.GetConnectionString("MongoDbConnection");
            }
            
            return settings;
        }
    }
}
