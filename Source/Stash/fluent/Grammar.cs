namespace Stash.fluent
{
    public static class Grammar
    {
        /// <summary>
        ///     Creates a CacheObject for the given cache, enabling fluent extension.
        /// </summary>
        public static CacheObject Which(this ICache cacheToModify)
        {
            return new CacheObject(cacheToModify);
        }
		
		/// <summary>
        ///     Creates a CacheObject for the given cache, enabling fluent extension.
        /// </summary>
        public static CacheObject With(this ICache cacheToModify)
        {
            return new CacheObject(cacheToModify);
        }

        /// <summary>
        ///     Creates a CacheObject for the given cache, enabling fluent extension.
        /// </summary>
        public static CacheObject And(this ICache cacheToModify)
        {
            return new CacheObject(cacheToModify);
        }
    }

    public class CacheObject
    {
        public CacheObject(ICache cacheToModify)
        {
            Cache = cacheToModify;
        }

        public ICache Cache { get; }
    }
}