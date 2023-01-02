using System.Collections.Concurrent;

namespace Vayosoft.Utilities.Synchronization
{
    public class NamedReaderWriterLocker
    {
        private readonly ConcurrentDictionary<string, ReaderWriterLockSlim> _locks = new();

        public ReaderWriterLockSlim GetLockByKey(string key)
        {
            return _locks.GetOrAdd(key, _ => new ReaderWriterLockSlim());
        }

        public ReadWriteLocker.ReadLockToken ReadLock(string key)
        {
            return new ReadWriteLocker.ReadLockToken(GetLockByKey(key));
        }

        public ReadWriteLocker.WriteLockToken WriteLock(string key)
        {
            return new ReadWriteLocker.WriteLockToken(GetLockByKey(key));
        }

        public void RemoveLock(string name)
        {
            _locks.TryRemove(name, out _);
        }
    }

    //var rwLock = new NamedReaderWriterLocker();
    //...
    //using(rwLock.GetReadLock(key))
    //{
    //   
    //}
}
