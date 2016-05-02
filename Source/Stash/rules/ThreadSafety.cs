using Stash.caches;
using Stash.fluent;

namespace Stash.rules
{
    public static class ThreadSafety
    {
        /// <summary>
        ///     Makes the ICache thread-safe.
        /// </summary>
        public static ICache IsThreadSafe(this IPronounOrConjunction target)
        {
            return new ThreadSafeCacheWrapper(target.Cache);
        }
    }
}