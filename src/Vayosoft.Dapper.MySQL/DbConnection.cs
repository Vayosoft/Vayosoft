using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Vayosoft.Dapper.MySQL
{
    public class DbConnection : IDisposable
    {
        private bool _disposed;

        public DbConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            InnerConnection = new MySqlConnection(connectionString);
        }

        protected IDbConnection InnerConnection { get; }

        public Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return InnerConnection.ExecuteAsync(sql, param, transaction);
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return (await InnerConnection.QueryAsync<T>(sql, param, transaction)).AsList();
            //return (await connection.QueryAsync<T>(
            //    new CommandDefinition(sql, param, transaction, cancellationToken: cancellationToken)
            //)).AsList();
        }

        public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return InnerConnection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return InnerConnection.QuerySingleAsync<T>(sql, param, transaction);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                InnerConnection.Dispose();
            }
            _disposed = true;
        }
    }
}