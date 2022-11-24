using System.Collections.Concurrent;

namespace Vayosoft.Utilities
{
    public class NamedReaderWriterLocker
    {
        private readonly ConcurrentDictionary<string, ReaderWriterLockSlim> _locks = new();

        public ReaderWriterLockSlim GetLock(string name)
        {
            return _locks.GetOrAdd(name, s => new ReaderWriterLockSlim());
        }

        public TResult RunWithReadLock<TResult>(string name, Func<TResult> body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterReadLock();
                return body();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public void RunWithReadLock(string name, Action body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterReadLock();
                body();
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public TResult RunWithWriteLock<TResult>(string name, Func<TResult> body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterWriteLock();
                return body();
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public void RunWithWriteLock(string name, Action body)
        {
            var rwLock = GetLock(name);
            try
            {
                rwLock.EnterWriteLock();
                body();
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }

        public void RemoveLock(string name)
        {
            _locks.TryRemove(name, out _);
        }
    }

    //static readonly NamedReaderWriterLocker _namedRwlocker = new NamedReaderWriterLocker();
    //void Write(string key, Stream s)
    //{
    //    var rwLock = _namedRwlocker.GetLock(key);
    //    try
    //    {
    //        rwLock.EnterWriteLock();
    //        //write file
    //    }
    //    finally
    //    {
    //        rwLock.ExitWriteLock();
    //    }
    //    //OR
    //    _namedRwlocker.RunWithWriteLock(key, () => /*write file*/);
    //}
    //Stream Read(string key)
    //{
    //    var rwLock = _namedRwlocker.GetLock(key);
    //    try
    //    {
    //        rwLock.EnterReadLock();
    //        //read file
    //    }
    //    finally
    //    {
    //        rwLock.ExitReadLock();
    //    }
    //    //OR
    //    return _namedRwlocker.RunWithReadLock(key, () => /*read file*/);
    //}
}
