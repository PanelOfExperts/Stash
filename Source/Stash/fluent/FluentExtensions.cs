using Stash.caches;

namespace Stash.fluent
{
    public static class FluentExtensions
    {
        /// <summary>
        ///     Makes the ICache thread-safe.
        /// </summary>
        public static ICache IsThreadSafe(this CacheObject target)
        {
            return new ThreadSafeCacheWrapper(target.Cache);
        }
    }
}