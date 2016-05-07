using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Stash.rules;

namespace Stash.caches
{
    public class Cache : ICache
    {
        private readonly Dictionary<string, Ticket> _cache = new Dictionary<string, Ticket>();

        /// <summary>
        ///     Returns a non-expiring cache with no special invalidation/eviction rules.
        /// </summary>
        public Cache() : this(ticket => false)
        {
        }

        /// <summary>
        ///     Returns a cache with the given invalidation/eviction rules.
        /// </summary>
        public Cache(Func<Ticket, bool> evictionRule) : this(evictionRule, new ExpirationRules())
        {
        }

        /// <summary>
        ///     Returns a cache with the given invalidation/eviction rules.
        /// </summary>
        internal Cache(Func<Ticket, bool> evictionRule, ExpirationRules rules)
        {
            ShouldEvict = evictionRule;
            ExpirationRules = rules;
        }

        // Internal methods to allow grammar to 
        // pick up the properties and then
        // chain them into new caches.
        internal Func<Ticket, bool> ShouldEvict { get; }
        internal ExpirationRules ExpirationRules { get; set; }

        public TValue Get<TValue>(string key, Func<TValue> getter)
        {
            if (string.IsNullOrEmpty(key)) return getter();

            Ticket ticket;
            if (!_cache.TryGetValue(key, out ticket) || ShouldEvict(ticket))
                ticket = ResetTicket(key, getter);

            return ConvertTo<TValue>(ticket);
        }

        void ICache.Set<TValue>(string key, TValue value)
        {
            Set(key, value);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public int Count
        {
            get
            {
                EvictTickets();
                return _cache.Count;
            }
        }

        public void Set<TValue>(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), Strings.EXCEPTION_KeyCannotBeNull);

            _cache[key] = BuildNewTicket(key, value);
        }

        private TValue ConvertTo<TValue>(Ticket ticket)
        {
            var temp = ticket.Value;

            if (temp is TValue)
                return (TValue) temp;

            // Can store null as well.
            if (temp == null)
                return default(TValue);

            // The cached value is not a TValue, try converting it.
            try
            {
                return (TValue) Convert.ChangeType(temp, typeof (TValue));
            }
            catch (InvalidCastException)
            {
                var message = string.Format(
                    Strings.EXCEPTION_StoredValueCannotBeConverted, temp, typeof (TValue));
                throw new ArgumentException(message);
            }
        }

        private void EvictTickets()
        {
            var itemsToRemove = (from item in _cache where ShouldEvict(item.Value) select item.Key).ToList();

            foreach (var key in itemsToRemove)
            {
                _cache.Remove(key);
            }
        }

        private Ticket BuildNewTicket<TValue>(string key, TValue value)
        {
            return new Ticket(key, value);
        }

        private Ticket ResetTicket<TValue>(string key, Func<TValue> getter)
        {
            Trace.WriteLine(
                $"Resetting ticket... LastAccessedTime={DateTime.UtcNow.Ticks}");
            var ticket = BuildNewTicket(key, getter());
            _cache[key] = ticket;
            return ticket;
        }
    }
}