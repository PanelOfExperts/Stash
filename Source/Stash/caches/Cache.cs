using System;
using System.Collections.Generic;
using System.Diagnostics;
using Stash.rules;

namespace Stash.caches
{
    public class Cache : ICache
    {
        private readonly Dictionary<string, Ticket> _cache = new Dictionary<string, Ticket>();

        /// <summary>
        ///     Returns a non-expiring cache with no special invalidation/eviction rules.
        /// </summary>
        public Cache() : this(value => new Ticket {Value = value}, ticket => false)
        {
        }

        /// <summary>
        ///     Returns a cache with the given ticket builder and invalidation/eviction rules.
        /// </summary>
        public Cache(Func<object, Ticket> ticketBuilder, Func<Ticket, bool> evictionRule)
        {
            TicketBuilder = ticketBuilder;
            ShouldEvict = evictionRule;
            ExpirationRules = new ExpirationRules();
        }

        // Internal methods to allow grammar to 
        // pick up the properties and then
        // chain them into new caches.
        internal Func<Ticket, bool> ShouldEvict { get; }
        internal Func<object, Ticket> TicketBuilder { get; }
        public ExpirationRules ExpirationRules { get; internal set; }

        public TValue Get<TValue>(string key, Func<TValue> getter)
        {
            if (string.IsNullOrEmpty(key)) return getter();

            Ticket ticket;
            if (!_cache.TryGetValue(key, out ticket) || ShouldEvict(ticket))
                ticket = ResetTicket(key, getter);

            return ConvertTo<TValue>(ticket);
        }

        public Ticket Set<TValue>(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), Strings.EXCEPTION_KeyCannotBeNull);

            if (_cache.ContainsKey(key))
            {
                _cache[key] = TicketBuilder(value);
                return _cache[key];
            }
            _cache.Add(key, TicketBuilder(value));
            return _cache[key];
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
            var itemsToRemove = GetEvictionKeys();
                
            foreach (var key in itemsToRemove)
            {
                _cache.Remove(key);
            }
        }

        private List<string> GetEvictionKeys()
        {
            var output = new List<string>();
            foreach (var item in _cache)
            {
                if (ShouldEvict(item.Value))
                {
                    output.Add(item.Key);
                }

                var now = DateTime.UtcNow;
                var absolute = ExpirationRules.AbsoluteExpiration;

                Trace.WriteLine($"Absolute: {absolute.Ticks}   Now:{now.Ticks}   ShouldEvict: {now>absolute}");

                //var slidingTime = item.Value.LastAccessedDate + ExpirationRules.SlidingTimeSpan;
                
                //Trace.WriteLine($"Sliding: {slidingTime.Ticks}   Now:{now.Ticks}   ShouldEvict: {now>slidingTime}");
            }
            return output;
        }


        private Ticket ResetTicket<TValue>(string key, Func<TValue> getter)
        {
            var ticket = TicketBuilder(getter());
            _cache[key] = ticket;
            return ticket;
        }
    }
}