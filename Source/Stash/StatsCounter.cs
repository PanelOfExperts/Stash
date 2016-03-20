using Stash.counters;

// ReSharper disable InconsistentNaming

namespace Stash
{
    /// <summary>
    ///     Accumulates statistics during the operation of a
    ///     <see cref="Cache{TKey, TValue}" /> for presentation
    ///     by <see cref="Cache{TKey,TValue}.Stats" />. This
    ///     is solely intended for consumption by
    ///     <see cref="Cache{TKey,TValue}" /> implementors.
    /// </summary>
    public abstract class StatsCounter
    {
        private static readonly NullStatsCounter NULL_COUNTER = new NullStatsCounter();

        public static StatsCounter GetNullCounter()
        {
            return NULL_COUNTER;
        }

        public static StatsCounter GetSimpleCounter()
        {
            return new SimpleStatsCounter();
        }

        #region Abstract Members

        /// <summary>
        ///     Records cache hits. This should be called when
        ///     a cache request returns a cached value.
        /// </summary>
        /// <param name="count">The number of hits to record.</param>
        public abstract void RecordHits(int count);

        /// <summary>
        ///     Records cache misses. This should be called when
        ///     a cache request returns a value that was not
        ///     found in the cache. This method should be
        ///     called by the loading thread, as well as
        ///     by threads blocking on the load. Multiple
        ///     concurrent calls to <see cref="Cache{TKey,TValue}" />
        ///     lookup methods with the same key on an
        ///     absent value should result in a single
        ///     call to either <see cref="RecordLoadSuccess" /> or
        ///     <see cref="RecordLoadException" /> and multiple
        ///     calls to this method, despite all being served
        ///     by the results of a single load operation.
        /// </summary>
        /// <param name="count">The number of misses to record.</param>
        public abstract void RecordMisses(int count);

        /// <summary>
        ///     Records the successful load of a new entry.
        ///     This should be called when a cache request
        ///     causes an entry to be loaded, and the loading
        ///     completes successfully. In contrast to
        ///     <see cref="RecordMisses" />, this method
        ///     should only be called by the loading thread.
        /// </summary>
        /// <param name="loadTime">
        ///     The number of nanoseconds the cache spent computing
        ///     or retrieving the new value.
        /// </param>
        public abstract void RecordLoadSuccess(long loadTime);

        /// <summary>
        ///     Records the failed load of a new entry.
        ///     This should be called when a cache request
        ///     causes an entry to be loaded, but an
        ///     exception is thrown while loading the entry.
        ///     In contrast to <see cref="RecordMisses" />,
        ///     this method should only be called by the
        ///     loading thread.
        /// </summary>
        /// <param name="loadTime">
        ///     The number of nanoseconds the cache spent computing
        ///     or retrieving the new value prior to an exception
        ///     being thrown.
        /// </param>
        public abstract void RecordLoadException(long loadTime);

        /// <summary>
        ///     Records the eviction of an entry from the cache.
        ///     This should only been called when an entry is
        ///     evicted due to the cache's eviction strategy,
        ///     and not as a result of manual invalidations
        ///     (e.g., <see cref="Cache{TKey,TValue}.Invalidate" />).
        /// </summary>
        public abstract void RecordEviction();

        /// <summary>
        ///     Returns a snapshot of this counter's values.
        ///     Note that this may be an inconsistent view, as
        ///     it may be interleaved with update operations.
        /// </summary>
        public abstract CacheStats Snapshot();

        /// <summary>
        ///     Increments all counters by the values in <paramref name="other" />.
        /// </summary>
        public abstract void IncrementBy(StatsCounter other);
        #endregion
    }
}