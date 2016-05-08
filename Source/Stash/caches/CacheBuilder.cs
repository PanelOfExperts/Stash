using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stash.caches
{
    public abstract class CacheBuilder
    {
        public static CacheBuilder NewCache()
        {
            return NewCache(null);
        }

        internal static CacheBuilder NewCache(ICacheFactory cacheFactory)
        {
            return new Implementation(new CacheSpec(), cacheFactory);
        }

        public abstract CacheBuilder WithEntryLifespan(TimeSpan lifespan);
        public abstract CacheBuilder WhichIsThreadSafe();
        public abstract ICache Create();

        private class Implementation : CacheBuilder
        {
            private readonly CacheSpec cacheSpec_;
            private readonly ICacheFactory cacheFactory_;

            public Implementation(CacheSpec cacheSpec, ICacheFactory cacheFactory)
            {
                cacheSpec_ = cacheSpec;
                cacheFactory_ = cacheFactory;
            }

            public override CacheBuilder WithEntryLifespan(TimeSpan lifespan)
            {
                cacheSpec_.EntryLifespan = lifespan;
                return this;
            }

            public override CacheBuilder WhichIsThreadSafe()
            {
                cacheSpec_.ThreadSafe = true;
                return this;
            }

            public override ICache Create()
            {
                return cacheFactory_.ConstructCache(cacheSpec_);
            }
        }
    }

    public sealed class CacheSpec
    {
        public TimeSpan EntryLifespan { get; set; } 
        public bool ThreadSafe { get; set; }
        //other stuff here

        private bool Equals(CacheSpec other)
        {
            return EntryLifespan.Equals(other.EntryLifespan) && ThreadSafe == other.ThreadSafe;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is CacheSpec && Equals((CacheSpec)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EntryLifespan.GetHashCode() * 397) ^ ThreadSafe.GetHashCode();
            }
        }

        public static bool operator ==(CacheSpec left, CacheSpec right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CacheSpec left, CacheSpec right)
        {
            return !Equals(left, right);
        }
    }

    public interface ICacheFactory
    {
        ICache ConstructCache(CacheSpec spec);
    }
}
