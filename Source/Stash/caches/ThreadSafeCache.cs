using System;
using System.Collections.Generic;

namespace Stash.caches
{
    internal class ThreadSafeCache<TKey, TValue> : Cache<TKey, TValue>
    {
        private readonly object _lockObject = new object();
        private readonly Cache<TKey, TValue> _underlying;

        public ThreadSafeCache(Cache<TKey, TValue> underlying)
        {
            _underlying = underlying;
        }
        
        public override TValue GetIfPresent(TKey key)
        {
            lock (_lockObject)
                return _underlying.GetIfPresent(key);
        }

        public override TValue Get(TKey key, Func<TValue> loader)
        {
            lock (_lockObject)
                return _underlying.Get(key, loader);
        }

        public override IDictionary<TKey, TValue> GetAllPresent(IEnumerable<TKey> keys)
        {
            lock (_lockObject)
                return _underlying.GetAllPresent(keys);
        }

        public override void Put(TKey key, TValue value)
        {
            lock (_lockObject)
                _underlying.Put(key, value);
        }

        public override void PutAll(IDictionary<TKey, TValue> map)
        {
            lock (_lockObject)
                _underlying.PutAll(map);
        }

        public override void Invalidate(TKey key)
        {
            lock (_lockObject)
                _underlying.Invalidate(key);
        }

        public override void InvalidateAll(IEnumerable<TKey> keys)
        {
            lock (_lockObject)
                _underlying.InvalidateAll(keys);
        }

        public override void InvalidateAll()
        {
            lock (_lockObject)
                _underlying.InvalidateAll();
        }

        public override long Size()
        {
            lock (_lockObject)
                return _underlying.Size();
        }

        public override CacheStats Stats()
        {
            lock (_lockObject)
                return _underlying.Stats();
        }

        public override void CleanUp()
        {
            lock (_lockObject)
                _underlying.CleanUp();
        }
    }
}