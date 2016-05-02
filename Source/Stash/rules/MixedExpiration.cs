using System;
using Stash.caches;

// return new Cache().Which().Expires().After(15 minutes).OrExpires().At(12:00 AM).WhicheverIsSooner();

// OrExpires() creates a new class to hold both rules, and 
// WhicheverIsSooner() (and WhicheverIsLater()) evaluates and 
// returns the Cache.

namespace Stash.rules
{
    public static class MixedExpiration
    {
        public static RulesHolder OrExpires(this Cache cache)
        {
            return new RulesHolder(cache);
        }

        /// <summary>
        ///     Adds a Sliding Expiration.
        /// </summary>
        public static RulesHolder After(this RulesHolder rules, TimeSpan timeSpan)
        {
            if (rules.SlidingTimeSpan != default(TimeSpan))
                throw new InvalidOperationException("Cannot have two sliding expirations.");

            return new RulesHolder(rules) {SlidingTimeSpan = timeSpan};
        }

        /// <summary>
        ///     Adds an Absolute Expiration.
        /// </summary>
        public static RulesHolder At(this RulesHolder rules, DateTime expirationTime)
        {
            if (rules.AbsoluteExpiration != default(DateTime))
                throw new InvalidOperationException("Cannot have two absolute expirations.");

            return new RulesHolder(rules) {AbsoluteExpiration = expirationTime};
        }

        public static Cache WhicheverIsSooner(this RulesHolder rules)
        {
            Func<Ticket, bool> evictionRule = ticket =>
            {
                var sliding = ticket.LastAccessedDate + rules.SlidingTimeSpan;
                var absolute = rules.AbsoluteExpiration;
                var first = sliding < absolute ? sliding : absolute;

                var now = DateTime.UtcNow;
                //Trace.WriteLine($"Sooner:   Sliding: {sliding.Ticks}   Now:{now.Ticks}   ShouldEvict: {now > first}");
                return now > first;
            };

            return new Cache(rules.TicketBuilder, evictionRule)
            {
                ExpirationRules = new ExpirationRules
                {
                    AbsoluteExpiration = rules.AbsoluteExpiration,
                    SlidingTimeSpan = rules.SlidingTimeSpan
                }
            };
        }

        public static Cache WhicheverIsLater(this RulesHolder rules)
        {
            Func<Ticket, bool> evictionRule = ticket =>
            {
                var sliding = ticket.LastAccessedDate + rules.SlidingTimeSpan;
                var absolute = rules.AbsoluteExpiration;
                var later = sliding > absolute ? sliding : absolute;
                return DateTime.UtcNow > later;
            };

            return new Cache(rules.TicketBuilder, evictionRule)
            {
                ExpirationRules = new ExpirationRules
                {
                    AbsoluteExpiration = rules.AbsoluteExpiration,
                    SlidingTimeSpan = rules.SlidingTimeSpan
                }
            };
        }

        public sealed class RulesHolder : ExpirationRules
        {
            internal RulesHolder(RulesHolder rules) : base(rules)
            {
                TicketBuilder = rules.TicketBuilder;
            }

            internal RulesHolder(Cache cache) : base(cache)
            {
                TicketBuilder = cache.TicketBuilder;
            }

            internal Func<object, Ticket> TicketBuilder { get; }
        }
    }


    public class ExpirationRules
    {
        internal ExpirationRules()
        {
        }

        public ExpirationRules(ExpirationRules rules)
        {
            AbsoluteExpiration = rules.AbsoluteExpiration;
            SlidingTimeSpan = rules.SlidingTimeSpan;
        }

        public ExpirationRules(Cache cache)
        {
            AbsoluteExpiration = cache.ExpirationRules.AbsoluteExpiration;
            SlidingTimeSpan = cache.ExpirationRules.SlidingTimeSpan;
        }

        public TimeSpan SlidingTimeSpan { get; set; }
        public DateTime AbsoluteExpiration { get; set; }
    }
}