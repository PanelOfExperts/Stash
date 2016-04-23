using System;
using Stash.fluent;

namespace Stash.Test.fluent
{
    public static class GrammarTestExtensions
    {
        /// <summary>
        ///     Wraps a cache in another class.
        /// </summary>
        public static ICache TastesLikeChicken(this CacheObject target)
        {
            return new ChickenCacheWrapper(target.Cache);
        }
    }

    internal class ChickenCacheWrapper : ICache
    {
        private readonly object _lockObject = new object();
        private readonly ICache _wrappedCache;

        public ChickenCacheWrapper(ICache wrappedCache)
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

        public void Set<TValue>(string key, TValue value)
        {
            lock (_lockObject)
                _wrappedCache.Set(key, value);
        }

        public void Clear()
        {
            lock (_lockObject)
                _wrappedCache.Clear();
        }
    }
}