using System;
using System.Collections.Generic;
using Stash.caches;
using Stash.counters;

namespace Stash
{
    public abstract class Cache<TKey, TValue>
    {
        /// <summary>
        ///     Accumulates statistics during the operation of a
        ///     <see cref="Cache{TKey, TValue}" /> for presentation
        ///     by <see cref="Stats" />. This
        ///     is solely intended for consumption by
        ///     <see cref="Cache{TKey,TValue}" /> implementors.
        /// </summary>
        protected readonly StatsCounter StatsCounter;

        protected Cache() : this(new SimpleStatsCounter())
        {
        }

        protected Cache(StatsCounter counter)
        {
            StatsCounter = counter;
        }


        public static Cache<TKey, TValue> GetTimeLimitCache(TimeSpan timeout)
        {
            return new ThreadSafeCache<TKey, TValue>(new TimeLimitCache<TKey, TValue>(timeout));
        }

        public static Cache<TKey, TValue> GetNonExpiringCache()
        {
            return new ThreadSafeCache<TKey, TValue>(new NonExpiringCache<TKey, TValue>());
        }

        #region Abstract Methods

        /// <summary>
        ///     Returns the value associated with <paramref name="key" />
        ///     in this cache, or <c>null</c> if there is no
        ///     cached value for <paramref name="key" />.
        /// </summary>
        public abstract TValue GetIfPresent(TKey key);

        /// <summary>
        ///     Returns the value associated with <paramref name="key" /> in
        ///     this cache, obtaining that value from <paramref name="loader" />
        ///     if necessary.
        ///     The method improves upon the conventional "if cached, return;
        ///     otherwise create, cache and return" pattern.
        ///     <para>
        ///         <b> Warning:</b> For any given key, every
        ///         <paramref name="loader" /> used with it should
        ///         compute the same value. Otherwise, a call that
        ///         passes one <paramref name="loader" /> may return
        ///         the result of another call with a differently
        ///         behaving <paramref name="loader" />. For example,
        ///         a call that requests a short timeout for an RPC
        ///         may wait for a similar call that requests a long
        ///         timeout, or a call by an unprivileged user may
        ///         return a resource accessible only to a privileged
        ///         user making a similar call. To prevent this
        ///         problem, create a key object that includes all
        ///         values that affect the result of the query.
        ///     </para>
        ///     <para>
        ///         <b>Warning:</b> <paramref name="loader" />
        ///         <b>must not</b> return <c>null</c>; it
        ///         may either return a non-null value or
        ///         throw an exception.
        ///     </para>
        ///     <para>
        ///         No observable state associated with this cache
        ///         is modified until loading completes.
        ///     </para>
        /// </summary>
        public abstract TValue Get(TKey key, Func<TValue> loader);

        /// <summary>
        ///     Returns a map of the values associated with
        ///     the given <paramref name="keys" /> in this
        ///     cache. The returned map will only contain
        ///     entries which are  already present in the cache.
        /// </summary>
        public abstract IDictionary<TKey, TValue> GetAllPresent(IEnumerable<TKey> keys);

        /// <summary>
        ///     Associates <paramref name="value" /> with
        ///     <paramref name="key" /> in this cache. If
        ///     the cache previously contained a value
        ///     associated with <paramref name="key" />,
        ///     the old value is replaced by <paramref name="value" />.
        ///     <para>
        ///         Prefer <see cref="Get" /> when using the
        ///         conventional "if cached, return; otherwise
        ///         create, cache and return" pattern.
        ///     </para>
        /// </summary>
        public abstract void Put(TKey key, TValue value);

        /// <summary>
        ///     Copies all of the mappings from the specified map to the cache.
        ///     The effect of this call is equivalent to that of calling
        ///     <see cref="Put" /> on this map once for each mapping from
        ///     key <c>key</c> to value <c>value</c> in the specified map.
        ///     The behavior of this operation is undefined if the specified
        ///     map is modified while the operation is in progress.
        /// </summary>
        public abstract void PutAll(IDictionary<TKey, TValue> map);

        /// <summary>
        ///     Discards any cached values for key <paramref name="key" />.
        /// </summary>
        public abstract void Invalidate(TKey key);

        /// <summary>
        ///     Discards any cached values for keys in
        ///     <paramref name="keys">the given set of keys</paramref>.
        /// </summary>
        public abstract void InvalidateAll(IEnumerable<TKey> keys);

        /// <summary>
        ///     Discards all entries in the cache.
        /// </summary>
        public abstract void InvalidateAll();

        /// <summary>
        ///     Returns the approximate number of entries in this cache.
        /// </summary>
        public abstract long Size();

        /// <summary>
        ///     Returns a current snapshot of this cache's cumulative
        ///     statistics, or a set of default values if the cache
        ///     is not recording statistics. All statistics begin
        ///     at zero and never decrease over the cache.
        ///     <p>
        ///         If statistics are not being recorded,
        ///         a <see cref="T:CacheStats" /> instance
        ///         with zero for all values is returned.
        ///     </p>
        /// </summary>
        public abstract CacheStats Stats();

        /// <summary>
        ///     Performs any pending maintenance operations needed
        ///     by the cache. Exactly which activities are performed
        ///     -- if any -- is implementation-dependent.
        /// </summary>
        public abstract void CleanUp();

        #endregion
    }
}