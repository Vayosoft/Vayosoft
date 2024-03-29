﻿using System.Collections.Concurrent;

namespace Vayosoft.Utilities.Synchronization
{
    public sealed class Locker
    {
        private readonly ConcurrentDictionary<string, object> _locks = new();

        public object Lock(string key)
        {
            return _locks.GetOrAdd(key, _ => new object());
        }

        public void RemoveLock(string name)
        {
            _locks.TryRemove(name, out _);
        }
    }

    public sealed class NamedLocker
    {
        private readonly string _name;

        private static readonly Dictionary<string, RefCounted<object>> Locks = new();

        public NamedLocker(string name)
        {
            _name = name;
        }

        public static NamedLocker GetLockerByName(string name)
        {
            return new NamedLocker(name);
        }

        public IDisposable Lock()
        {
            return new DisposableLock(_name);
        }

        private readonly struct DisposableLock : IDisposable
        {
            private readonly bool _taken = false;
            private readonly string _name;

            public DisposableLock(string name)
            {
                _name = name;

                RefCounted<object> item;
                lock (Locks)
                {
                    if (Locks.TryGetValue(name, out item!))
                    {
                        ++item.RefCount;
                    }
                    else
                    {
                        item = new RefCounted<object>(new object());
                        Locks[name] = item;
                    }
                }

                Monitor.Enter(item.Value, ref _taken);
            }

            public void Dispose()
            {
                RefCounted<object> item;
                lock (Locks)
                {
                    item = Locks[_name];
                    --item.RefCount;
                    if (item.RefCount == 0)
                    {
                        Locks.Remove(_name);
                    }
                }

                if (_taken)
                {
                    Monitor.Exit(item.Value);
                }
            }
        }

        private sealed class RefCounted<T>
        {
            public RefCounted(T value)
            {
                RefCount = 1;
                Value = value;
            }

            public int RefCount { get; set; }
            public T Value { get; }
        }
    }
}
