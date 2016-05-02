using System;
using Stash.fluent;

namespace Stash.rules
{
    public static class ThreadSafety
    {
        /// <summary>
        ///     Makes the ICache thread-safe.
        /// </summary>
        public static ICache IsThreadSafe(this IPronounOrConjunction target)
        {
            return new ThreadSafeCacheWrapper(target.Cache);
        }
    }

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

        public ICacheEntry Set<TValue>(string key, TValue value)
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