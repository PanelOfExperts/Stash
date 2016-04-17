using Stash.caches;

namespace Stash.fluent
{
    public static class Grammar
    {
        /// <summary>
        ///     no op.  Linguistic pass-through.
        /// </summary>
        public static ICache WhichIs(this ICache cache)
        {
            return cache;
        }

        /// <summary>
        ///     Makes the ICache thread-safe.
        /// </summary>
        public static ICache ThreadSafe(this ICache cache)
        {
            return new ThreadSafeCacheWrapper(cache);
        }

    }
}