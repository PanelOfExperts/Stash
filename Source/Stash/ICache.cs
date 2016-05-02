using System;
using Stash.caches;

namespace Stash
{
    /// <summary>
    ///     A semi-persistent mapping from keys to values. Cache entries are manually
    ///     added using <see cref="Get{TValue}" /> or <see cref="Set{TValue}" />.
    /// </summary>
    public interface ICache
    {
        /// <summary>
        ///     Returns the number of entries in this cache.
        /// </summary>
        int Count { get; }

        /// <summary>
        ///     Returns the value associated with <paramref name="key" /> in
        ///     this cache, obtaining that value from <paramref name="loader" />
        ///     if necessary.
        /// </summary>
        TValue Get<TValue>(string key, Func<TValue> loader);

        /// <summary>
        ///     Associates <paramref name="value" /> with
        ///     <paramref name="key" /> in this cache. If
        ///     the cache previously contained a value
        ///     associated with <paramref name="key" />,
        ///     the old value is replaced by <paramref name="value" />.
        ///     <para>
        ///         Prefer <see cref="M:Get" /> when using the
        ///         conventional "if cached, return; otherwise
        ///         create, cache and return" pattern.
        ///     </para>
        /// </summary>
        ICacheEntry Set<TValue>(string key, TValue value);

        /// <summary>
        ///     Removes all keys and values from this cache.
        /// </summary>
        void Clear();
    }
}