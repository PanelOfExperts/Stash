using Stash.caches;
using Stash.fluent;

namespace Stash
{
    public sealed class CacheFactory
    {
        public ICache GetNullCache()
        {
            return new NullCache();
        }

        public ICache GetNonExpiringCache()
        {
            return new NonExpiringCache().Which().IsThreadSafe();
        }
    }
}