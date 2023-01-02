using System.Collections.Concurrent;

namespace Vayosoft.Utilities.Synchronization
{
    public sealed class ReadWriteLocker : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock = new();

        public ReadLockReleaser ReadLock()
        {
            return new ReadLockReleaser(_lock);
        }

        public WriteLockReleaser WriteLock()
        {
            return new WriteLockReleaser(_lock);
        }

        public void Dispose()
        {
            _lock.Dispose();
        }
    }

    public sealed class NamedReadWritLocker
    {
        private readonly ConcurrentDictionary<string, ReaderWriterLockSlim> _locks = new();

        private ReaderWriterLockSlim GetLockByKey(string key)
        {
            return _locks.GetOrAdd(key, _ => new ReaderWriterLockSlim());
        }

        public ReadLockReleaser ReadLock(string key)
        {
            return new ReadLockReleaser(GetLockByKey(key));
        }

        public WriteLockReleaser WriteLock(string key)
        {
            return new WriteLockReleaser(GetLockByKey(key));
        }

        public void RemoveLock(string name)
        {
            _locks.TryRemove(name, out _);
        }
    }

    public readonly struct WriteLockReleaser : IDisposable
    {
        private readonly ReaderWriterLockSlim _writLock;
        public WriteLockReleaser(ReaderWriterLockSlim writLock)
        {
            _writLock = writLock;
            writLock.EnterWriteLock();
        }

        public void Dispose()
        {
            _writLock.ExitWriteLock();
        }
    }

    public readonly struct ReadLockReleaser : IDisposable
    {
        private readonly ReaderWriterLockSlim _readLock;
        public ReadLockReleaser(ReaderWriterLockSlim readLock)
        {
            _readLock = readLock;
            readLock.EnterReadLock();
        }

        public void Dispose()
        {
            _readLock.ExitReadLock();
        }
    }
}
