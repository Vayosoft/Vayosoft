namespace Vayosoft.Utilities.Synchronization
{
    public sealed class ReadWriteLocker : IDisposable
    {
        public readonly struct WriteLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _writLock;
            public WriteLockToken(ReaderWriterLockSlim writLock)
            {
                _writLock = writLock;
                writLock.EnterWriteLock();
            }
            public void Dispose() => _writLock.ExitWriteLock();
        }

        public readonly struct ReadLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _readLock;
            public ReadLockToken(ReaderWriterLockSlim readLock)
            {
                _readLock = readLock;
                readLock.EnterReadLock();
            }
            public void Dispose() => _readLock.ExitReadLock();
        }

        private readonly ReaderWriterLockSlim _lock = new();

        public ReadLockToken ReadLock() => new(_lock);
        public WriteLockToken WriteLock() => new(_lock);

        public void Dispose() => _lock.Dispose();
    }
}

//    var rwLock = new RWLock();
//// ...
//    using(rwLock.ReadLock())
//    {
//        // ...
//    }
