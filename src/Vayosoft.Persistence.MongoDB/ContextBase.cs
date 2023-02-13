using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence.MongoDB
{
    public abstract class ContextBase : IDisposable
    {
        private bool _disposed;

        protected readonly IServiceScope Scope;
        protected readonly Dictionary<string, object> Repositories = new();

        // var root = IdentityMap.OfType<T>().FirstOrDefault(a => a.Id.Equals(id));
        protected readonly ISet<IAggregateRoot> IdentityMap = new HashSet<IAggregateRoot>(AggregateRootEqualityComparer.Instance);

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

    public class AggregateRootEqualityComparer : IEqualityComparer<IAggregateRoot>
    {
        private static readonly Lazy<AggregateRootEqualityComparer> Lazy =
            new(() => new AggregateRootEqualityComparer());

        public static AggregateRootEqualityComparer Instance => Lazy.Value;

        public bool Equals(IAggregateRoot x, IAggregateRoot y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;

            return x.Id.Equals(y.Id);
        }

        public int GetHashCode(IAggregateRoot obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
