using Microsoft.Extensions.Configuration;

namespace Vayosoft.Persistence.MongoDB
{
    public class MongoDbSettings
    {
        public const string DefaultConnectionString = "MongoDbConnection";
        public ReplicaSetSetting ReplicaSet { set; get; }

        public string ConnectionString { set; get; }
    }

    public class ReplicaSetSetting
    {
        public string[] BootstrapServers { set; get; }
    }

    public static class ConnectionSettingExtensions
    {
        public static MongoDbSettings GetMongoDbSettings(this IConfiguration configuration)
        {
            var settings =  configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>() ?? new MongoDbSettings();
            if(string.IsNullOrEmpty(settings.ConnectionString))
            {
                settings.ConnectionString = configuration.GetConnectionString(MongoDbSettings.DefaultConnectionString);
            }
            
            return settings;
        }
    }
}
