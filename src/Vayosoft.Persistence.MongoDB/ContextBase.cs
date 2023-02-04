using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence.MongoDB
{
    public abstract class ContextBase : IDisposable
    {
        private bool _disposed;

        protected readonly IServiceScope Scope;
        protected readonly Dictionary<string, object> Repositories = new();

        protected ContextBase(IServiceProvider serviceProvider)
        {
            Scope = serviceProvider.CreateScope();
        }

        protected IRepository<T> Repository<T>() where T : class, IAggregateRoot
        {
            var key = typeof(T).Name;
            if (Repositories.TryGetValue(key, out var repo))
            {
                return (IRepository<T>)repo;
            }

            var r = Scope.ServiceProvider.GetRequiredService<IRepository<T>>();
            Repositories.Add(key, r);

            return r;
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
                Scope.Dispose();
            }
            _disposed = true;
        }
    }
}
