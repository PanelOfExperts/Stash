using System;
using System.Collections.Generic;
using System.Linq;

namespace Stash.caches
{
    
    internal class RulesCache : ICache
    {
        private readonly Dictionary<string, Ticket> _cache = new Dictionary<string, Ticket>();
        private readonly Func<Ticket, bool> _shouldEvict;
        private readonly Func<object, Ticket> _ticketBuilder;

        public RulesCache(Func<object, Ticket> ticketBuilder, Func<Ticket, bool> evictionRule)
        {
            _ticketBuilder = ticketBuilder;
            _shouldEvict = evictionRule;
        }

        public TValue Get<TValue>(string key, Func<TValue> getter)
        {
            if (string.IsNullOrEmpty(key)) return getter();

            Ticket ticket;
            if (!_cache.TryGetValue(key, out ticket) || _shouldEvict(ticket))
                ticket = ResetTicket(key, getter);

            return ProperType<TValue>(ticket);
        }

        public void Set<TValue>(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), Strings.EXCEPTION_KeyCannotBeNull);

            if (_cache.ContainsKey(key))
            {
                _cache[key] = _ticketBuilder(value);
                return;
            }
            _cache.Add(key, _ticketBuilder(value));
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
        
        private TValue ProperType<TValue>(Ticket ticket)
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
            var itemsToRemove = 
                (from item in _cache
                 where _shouldEvict(item.Value)
                 select item.Key).ToList();

            foreach (var key in itemsToRemove)
            {
                _cache.Remove(key);
            }
        }

        private Ticket ResetTicket<TValue>(string key, Func<TValue> getter)
        {
            var ticket = _ticketBuilder(getter());
            _cache[key] = ticket;
            return ticket;
        }
    }
}