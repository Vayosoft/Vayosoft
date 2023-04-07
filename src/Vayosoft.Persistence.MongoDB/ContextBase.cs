using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Commons.Aggregates;

namespace Vayosoft.Persistence.MongoDB
{
    public abstract class ContextBase : IDisposable
    {
        private bool _disposed;

        protected readonly IServiceScope Scope;
        private readonly Dictionary<string, object> _repositories = new();
        private readonly ISet<IAggregateRoot> _identityMap = 
            new HashSet<IAggregateRoot>(AggregateRootEqualityComparer.Instance);   
        // var root = IdentityMap.OfType<T>().FirstOrDefault(a => a.Id.Equals(id));

        protected ContextBase(IServiceProvider serviceProvider)
        {
            Scope = serviceProvider.CreateScope();
        }

        protected T Repository<T, TEntity>() where T : IRepository<TEntity> where TEntity : class, IAggregateRoot
        {
            var key = typeof(T).Name;
            if (_repositories.TryGetValue(key, out var repo))
            {
                return (T)repo;
            }

            var r = Scope.ServiceProvider.GetRequiredService<T>();
            _repositories.Add(key, r);

            return r;
        }

        protected IRepository<T> Repository<T>() where T : class, IAggregateRoot
        {
            var key = typeof(T).Name;
            if (_repositories.TryGetValue(key, out var repo))
            {
                return (IRepository<T>)repo;
            }

            var r = Scope.ServiceProvider.GetRequiredService<IRepository<T>>();
            _repositories.Add(key, r);

            return r;
        }

        protected T Find<T>(object id) where T : IAggregateRoot
            => _identityMap.OfType<T>().FirstOrDefault(ab => ab.Id.Equals(id));

        protected void Register(IAggregateRoot entity)
        {
            _identityMap.Add(entity);
        }

        protected void Register(IEnumerable<IAggregateRoot> entities)
        {
            foreach (var entity in entities)
            {
                Register(entity);
            }
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

    internal sealed class AggregateRootEqualityComparer : IEqualityComparer<IAggregateRoot>
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
