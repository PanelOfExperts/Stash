using System;

namespace Stash.caches
{
    internal class NonExpiringCache : RulesCache
    {
        private static readonly Func<object, Ticket> TicketBuilder = value => new Ticket {Value = value};
        private static readonly Func<Ticket, bool> EvictionRule = ticket => false;

        public NonExpiringCache() : base(TicketBuilder, EvictionRule)
        {
        }
    }
}