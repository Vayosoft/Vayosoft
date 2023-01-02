using System.Collections.Concurrent;

namespace Vayosoft.Utilities.Synchronization
{
    public class NamedLocker
    {
        private readonly ConcurrentDictionary<string, object> _locks = new();

        public object GetLockByKey(string key)
        {
            return _locks.GetOrAdd(key, s => new object());
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
