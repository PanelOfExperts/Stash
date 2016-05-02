using System;
using Stash.caches;
using Stash.fluent;

// return new Cache().Which().Expires().After(15 minutes);
// or
// return new Cache().Which().Expires().At(12:00 AM);

namespace Stash.rules
{
    public static class SingleExpiration
    {
        public static TransientCache Expires(this IPronounOrConjunction cache)
        {
            return new TransientCache(cache);
        }

        /// <summary>
        ///     Creates a cache with Sliding Expiration.
        /// </summary>
        public static Cache After(this TransientCache transient, TimeSpan timeSpan)
        {
            var ticketBuilder = transient.Cache.TicketBuilder;
            var rules = transient.Cache.ExpirationRules;
            rules.SlidingTimeSpan = timeSpan;
            return new Cache(ticketBuilder, ticket =>
                DateTime.UtcNow > ticket.LastAccessedDate + timeSpan
                )
            {
                ExpirationRules = rules
            };
        }

        /// <summary>
        ///     Creates a cache with Absolute Expiration.
        /// </summary>
        public static Cache At(this TransientCache transient, DateTime expirationTime)
        {
            var ticketBuilder = transient.Cache.TicketBuilder;
            var rules = transient.Cache.ExpirationRules;
            rules.AbsoluteExpiration = expirationTime;
            return new Cache(ticketBuilder, ticket => DateTime.UtcNow > expirationTime)
            {
                ExpirationRules = rules
            };
        }

        public class TransientCache
        {
            public TransientCache(IPronounOrConjunction cacheToModify)
            {
                PartOfSpeech = cacheToModify;
            }

            public IPronounOrConjunction PartOfSpeech { get; }
            internal Cache Cache => (Cache) PartOfSpeech.Cache;
        }
    }
}