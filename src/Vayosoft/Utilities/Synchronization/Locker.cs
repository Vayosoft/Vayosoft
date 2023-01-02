using System.Collections.Concurrent;

namespace Vayosoft.Utilities.Synchronization
{
    public class Locker
    {
        private readonly ConcurrentDictionary<string, object> _locks = new();

        public object Lock(string key)
        {
            return _locks.GetOrAdd(key, s => new object());
        }

        public void RemoveLock(string name)
        {
            _locks.TryRemove(name, out _);
        }
    }
}
