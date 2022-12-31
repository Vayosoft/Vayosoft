using System.Collections.Concurrent;

namespace Vayosoft.Utilities.Synchronization
{
    public class NamedLocker
    {
        private readonly ConcurrentDictionary<string, object> _locks = new();

        public object GetLock(string name)
        {
            return _locks.GetOrAdd(name, s => new object());
        }

        public TResult RunWithLock<TResult>(string name, Func<TResult> body)
        {
            lock (_locks.GetOrAdd(name, s => new object()))
                return body();
        }

        public void RunWithLock(string name, Action body)
        {
            lock (_locks.GetOrAdd(name, s => new object()))
                body();
        }

        public void RemoveLock(string name)
        {
            _locks.TryRemove(name, out _);
        }
    }

    //static readonly NamedLocker _namedlocker = new NamedLocker();
    //void Write(string key, Stream s)
    //{
    //    lock (_namedlocker.GetLock(key))
    //    {
    //        write file
    //    }
    //    OR
    //    _namedlocker.RunWithLock(key, () => /*write file*/);
    //}
    //Stream Read(string key)
    //{
    //    lock (_namedlocker.GetLock(key))
    //    {
    //        read file
    //    }
    //    OR
    //    return _namedlocker.RunWithLock(key, () => /*read file*/);
    //}
}
