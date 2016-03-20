using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Stash.@internal;

namespace Stash.caches
{
    internal class TimeLimitCache<TKey, TValue> : Cache<TKey, TValue>
    {
        private readonly Dictionary<TKey, Ticket> _cache = new Dictionary<TKey, Ticket>();
        private readonly Func<DateTime> _now;
        private readonly TimeSpan _timeout;

        public TimeLimitCache(TimeSpan timeout) 
        {
            _now = () => DateTime.UtcNow;
            _timeout = timeout;
        }

        private TValue GetWithBusinessRules(TKey key, Func<TValue> loader = null)
        {
            Ticket ticket;
            var foundIt = _cache.TryGetValue(key, out ticket);
            if (foundIt && !ticket.IsExpired)
            {
                StatsCounter.RecordHits(1);
                return ticket.Value;
            }

            if (foundIt)
            {
                // It's expired, so evict it.
                StatsCounter.RecordEviction();
                Invalidate(key);
            }

            StatsCounter.RecordMisses(1);
            if (loader == null) return default(TValue);

            ticket = ResetTicket(key, loader);
            return ticket.Value;
        }

        public override TValue GetIfPresent(TKey key)
        {
            Preconditions.CheckNotNull(key);
            return GetWithBusinessRules(key);
        }

        public override TValue Get(TKey key, Func<TValue> loader)
        {
            Preconditions.CheckNotNull(key);
            return GetWithBusinessRules(key, loader);
        }

        public override IDictionary<TKey, TValue> GetAllPresent(IEnumerable<TKey> keys)
        {
            var result = new Dictionary<TKey, TValue>();
            foreach (var key in keys)
            {
                var value = GetIfPresent(key);
                if (value == null)
                {
                    continue;
                }
                result.Add(key, value);
            }
            return new ReadOnlyDictionary<TKey, TValue>(result);
        }

        public override void Put(TKey key, TValue value)
        {
            Preconditions.CheckNotNull(key);
            var ticket = Ticket.GetInstance(value, _now(), _timeout);
            if (_cache.ContainsKey(key))
            {
                _cache[key] = ticket;
                return;
            }
            _cache.Add(key, ticket);
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
            Preconditions.CheckNotNull(key);
            _cache.Remove(key);
        }

        public override void InvalidateAll(IEnumerable<TKey> keys)
        {
            foreach (var key in keys)
                Invalidate(key);
        }

        public override void InvalidateAll()
        {
            foreach (var item in _cache)
                Invalidate(item.Key);
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

        private Ticket ResetTicket(TKey key, Func<TValue> getter)
        {
            Preconditions.CheckNotNull(key);
            StatsCounter.RecordMisses(1);
            var ticket = Ticket.GetInstance(getter(), _now(), _timeout);
            _cache[key] = ticket;
            return ticket;
        }

        private class Ticket
        {
            private readonly DateTime _creationDate;
            private readonly Func<DateTime> _now;
            private readonly TimeSpan _timeout;

            private Ticket(TValue value, DateTime creationDate, TimeSpan timeout)
            {
                _timeout = timeout;
                _now = () => DateTime.UtcNow;
                Value = value;
                _creationDate = creationDate;
            }

            public TValue Value { get; }

            public bool IsExpired => (_creationDate + _timeout < _now());

            internal static Ticket GetInstance(TValue value, DateTime creationDate, TimeSpan timeout)
            {
                return new Ticket(value, creationDate, timeout);
            }
        }
    }
}