using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stash.@internal;

namespace Stash.caches
{
    internal class NonExpiringCache<TKey, TValue> : Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        
        public override TValue GetIfPresent(TKey key)
        {
            Preconditions.CheckNotNull(key);
            TValue value;
            var foundIt = _cache.TryGetValue(key, out value);
            if (!foundIt)
            {
                StatsCounter.RecordMisses(1);
                return default(TValue);
            }

            StatsCounter.RecordHits(1);
            return value;
        }

        public override TValue Get(TKey key, Func<TValue> loader)
        {
            Preconditions.CheckNotNull(key);
            
            TValue val;
            if (_cache.TryGetValue(key, out val))
            {
                StatsCounter.RecordHits(1);
                return val;
            }

            //StatsCounter.RecordMisses(1);
            val = loader();
            _cache[key] = val;

            return val;
        }

        public override IDictionary<TKey, TValue> GetAllPresent(IEnumerable<TKey> keys)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var key in keys)
            {
                var value = GetIfPresent(key);
                if (value == null)
                {
                    StatsCounter.RecordMisses(1);
                    continue;
                }
                StatsCounter.RecordHits(1);
                result.Add(key, value);
            }
            return new ReadOnlyDictionary<TKey, TValue>(result);
        }

        public override void Put(TKey key, TValue value)
        {
            Preconditions.CheckNotNull(key);
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }
            _cache.Add(key, value);
        }

        public override void PutAll(IDictionary<TKey, TValue> map)
        {
            Preconditions.CheckNotNull(map);
            foreach (var kvp in map)
            {
                Put(kvp.Key, kvp.Value);
            }
        }

        public override void Invalidate(TKey key)
        {
            _cache.Remove(key);
        }

        public override void InvalidateAll(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
                Invalidate(key);
        }

        public override void InvalidateAll()
        {
            _cache.Clear();
        }

        public override long Size()
        {
            return _cache.Count;
        }

        public override CacheStats Stats()
        {
            return StatsCounter.Snapshot();
        }

        public override void CleanUp()
        {
            // Do Nothing.

            // lock
            //  expire entries
            //  set read count to zero
            // unlock 
        }
    }
}