using System;

namespace Stash.caches
{
    internal class ThreadSafeCacheWrapper : ICache
    {
        private readonly object _lockObject = new object();
        private readonly ICache _wrappedCache;

        public ThreadSafeCacheWrapper(ICache wrappedCache)
        {
            _wrappedCache = wrappedCache;
        }


        public int Count
        {
            get
            {
                lock (_lockObject)
                    return _wrappedCache.Count;
            }
        }


        public TValue Get<TValue>(string key, Func<TValue> loader)
        {
            lock (_lockObject)
                return _wrappedCache.Get(key, loader);
        }

        public Ticket Set<TValue>(string key, TValue value)
        {
            lock (_lockObject)
                return _wrappedCache.Set(key, value);
        }

        public void Clear()
        {
            lock (_lockObject)
                _wrappedCache.Clear();
        }
    }
}