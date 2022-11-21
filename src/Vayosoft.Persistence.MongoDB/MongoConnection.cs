﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Vayosoft.Commons.Entities;
using Vayosoft.Persistence.MongoDB.Extensions;

namespace Vayosoft.Persistence.MongoDB
{
    public sealed class MongoConnection : IMongoConnection
    {
        private const string DefaultDb = "default";
        private readonly MongoClient _client;
        public IMongoDatabase Database { get; }
        public IClientSessionHandle Session { get; private set; }

        [ActivatorUtilitiesConstructor]
        public MongoConnection(IConfiguration config, ILoggerFactory loggerFactory) 
            : this(config.GetConnectionSetting(), loggerFactory) { }
        public MongoConnection(ConnectionSetting config, ILoggerFactory loggerFactory) 
            : this(config?.ConnectionString, config?.ReplicaSet?.BootstrapServers, loggerFactory) { }
        public MongoConnection(string connectionString, string[] bootstrapServers, ILoggerFactory loggerFactory)
        {
            MongoClientSettings settings;
            string databaseName = null;
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                var connectionUrl = new MongoUrl(connectionString);
                settings = MongoClientSettings.FromUrl(connectionUrl);
                databaseName = GetDatabaseName(connectionString!);
            }
            else
            {
                settings = new MongoClientSettings
                {
                    DirectConnection = false,
                    ReadPreference = ReadPreference.Primary
                };

                if (bootstrapServers != null)
                    settings.Servers = bootstrapServers.Select(MongoServerAddress.Parse);
                else
                {
                    settings.Servers = new[]
                    {
                        new MongoServerAddress("localhost", 37017),
                    };
                }
            }

            var logger = loggerFactory.CreateLogger<MongoConnection>();
            settings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    if (logger.IsEnabled(LogLevel.Debug))
                    {
                        logger.LogDebug("{commandName} {command}", e.CommandName, e.Command.ToJson());
                    }
                });
            };

            _client = new MongoClient(settings);
            Database = _client.GetDatabase(databaseName ?? DefaultDb);
        }

        public async Task<IClientSessionHandle> StartSession(CancellationToken cancellationToken = default)
        {
            var session = await _client.StartSessionAsync(cancellationToken: cancellationToken);
            Session = session;
            return session;
        }

        public IMongoDatabase GetDatabase(string db)
            => _client.GetDatabase(db);

        public IMongoCollection<T> Collection<T>(CollectionName collectionName = null) where T : IEntity
            => Database.GetDocumentCollection<T>(collectionName);

        private static string GetDatabaseName(string connectionString)
        {
            var hostIndex = connectionString.IndexOf("//", StringComparison.Ordinal);
            if (hostIndex > 0)
            {
                var startIndex = connectionString.IndexOf("/", hostIndex + 2, StringComparison.Ordinal) + 1;
                var endIndex = connectionString.IndexOf("?", startIndex, StringComparison.Ordinal);
                if (startIndex > 0)
                {
                    return endIndex > 0 ?
                        connectionString[startIndex..endIndex] :
                        connectionString[startIndex..];
                }
            }

            throw new ArgumentException("Unsupported DB connection string", nameof(connectionString));
        }
    }
}
