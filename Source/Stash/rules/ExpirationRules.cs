using System;
using Stash.caches;

namespace Stash.rules
{
    public enum ExpirationType
    {
        WhicheverIsSoonest,
        WhicheverIsLatest
    }

    public class ExpirationRules
    {
        internal ExpirationRules()
        {
        }

        public ExpirationRules(Cache cache) : this(cache.ExpirationRules)
        {
        }

        public ExpirationRules(ExpirationRules rules)
        {
            AbsoluteExpiration = rules.AbsoluteExpiration;
            SlidingExpiration = rules.SlidingExpiration;
            Type = rules.Type;
        }

        public ExpirationType Type { get; set; }
        public TimeSpan SlidingExpiration { get; set; }
        public DateTime AbsoluteExpiration { get; set; }

        public DateTime GetNewExpiration(DateTime lastAccessed)
        {
            if ((SlidingExpiration == default(TimeSpan)) && (AbsoluteExpiration == default(DateTime)))
                return DateTime.MaxValue;

            if (SlidingExpiration == default(TimeSpan)) return AbsoluteExpiration;

            var sliding = lastAccessed + SlidingExpiration;
            if (AbsoluteExpiration == default(DateTime)) return sliding;

            if (Type == ExpirationType.WhicheverIsSoonest)
            {
                return sliding < AbsoluteExpiration ? sliding : AbsoluteExpiration;
            }
            
            return sliding > AbsoluteExpiration ? sliding : AbsoluteExpiration;
        }
    }
}