using System;
using Stash.@internal;

namespace Stash
{
    /// <summary>
    ///     Statistics about the performance of a <see cref="Cache{TKey,TValue}" />. Instances
    ///     of this class are immutable.
    ///     <para>
    ///         Cache statistics are incremented according to the following rules:
    ///         <ul>
    ///             <li>
    ///                 When a cache lookup encounters an existing cache entry
    ///                 <see cref="HitCount" /> is incremented.
    ///             </li>
    ///             <li>
    ///                 When a cache lookup first encounters a missing cache
    ///                 entry, a new entry is loaded.
    ///             </li>
    ///             <ul>
    ///                 <li>
    ///                     After successfully loading an entry, <see cref="MissCount" />
    ///                     and <see cref="LoadSuccessCount" /> are incremented, and the
    ///                     total loading time, in nanoseconds, is added to
    ///                     <see cref="TotalLoadTime" />.
    ///                 </li>
    ///                 <li>
    ///                     When an exception is thrown while loading an entry,
    ///                     <see cref="MissCount" /> and <see cref="LoadExceptionCount" />
    ///                     are incremented, and the total loading time, in nanoseconds,
    ///                     is added to <see cref="TotalLoadTime" />.
    ///                 </li>
    ///                 <li>
    ///                     Cache lookups that encounter a missing cache entry that is
    ///                     still loading will wait for loading to complete (whether
    ///                     successful or not) and then increment <see cref="MissCount" />.
    ///                 </li>
    ///             </ul>
    ///             <li>
    ///                 When an entry is evicted from the cache,
    ///                 <see cref="EvictionCount" /> is incremented.
    ///             </li>
    ///             <li>
    ///                 No stats are modified when a cache entry is invalidated or
    ///                 manually removed.
    ///             </li>
    ///         </ul>
    ///     </para>
    /// </summary>
    public sealed class CacheStats
    {
        public static readonly CacheStats Empty = new CacheStats(0, 0, 0, 0, 0, 0);
        
        /// <summary>
        ///     Constructs a new instance of the <see cref="T:CacheStats" /> class.
        ///     <p>
        ///         Five parameters of the same type in a row is a bad thing,
        ///         but this class is not constructed by the users and is too
        ///         fine-grained for a builder.
        ///     </p>
        /// </summary>
        public CacheStats(long hitCount, long missCount, long loadSuccessCount, long loadExceptionCount, long totalLoadTime, long evictionCount)
        {
            Preconditions.CheckArgument(hitCount >= 0);
            Preconditions.CheckArgument(missCount >= 0);
            Preconditions.CheckArgument(loadSuccessCount >= 0);
            Preconditions.CheckArgument(loadExceptionCount >= 0);
            Preconditions.CheckArgument(totalLoadTime >= 0);
            Preconditions.CheckArgument(evictionCount >= 0);

            HitCount = hitCount;
            MissCount = missCount;
            LoadSuccessCount = loadSuccessCount;
            LoadExceptionCount = loadExceptionCount;
            TotalLoadTime = totalLoadTime;
            EvictionCount = evictionCount;
        }
        
        /// <summary>
        ///     Returns the number of times <see cref="Cache{TKey,TValue}" /> lookup methods have
        ///     returned either a chached or uncached value. This is defined as
        ///     <code>HitCount + MissCount</code>.
        /// </summary>
        public long RequestCount => HitCount + MissCount;

        /// <summary>
        ///     Returns the number of times <see cref="Cache{TKey,TValue}" /> lookup methods have
        ///     returned a cached value.
        /// </summary>
        public long HitCount { get; }

        /// <summary>
        ///     Returns the number of times <see cref="Cache{TKey,TValue}" /> lookup methods have
        ///     returned an uncached (newly loaded) value, or null. Multiple
        ///     concurrent calls to <see cref="Cache{TKey,TValue}" /> lookup methods on an absent
        ///     value can result in multiple misses, all returning the results of a
        ///     single cache load operation.
        /// </summary>
        public long MissCount { get; }

        /// <summary>
        ///     Returns the ratio of cache requests which were misses. This is defined as
        ///     <code>MissCount / RequestCount</code>, or <code>1.0</code> when
        ///     <code>RequestCount == 0</code>.
        ///     Note that <code>HitRate + MissRate =~ 1.0</code>.
        ///     Cache misses include all requests which weren't cache hits, including
        ///     requests which resulted in either successful or failed loading attempts,
        ///     and requests which waited for other threads to finish loading.
        ///     It is thus the case that
        ///     <code>MissCount &gt;= LoadSuccessCount + LoadExceptionCount</code>.
        ///     Multiple concurrent misses for the same key will result in a single
        ///     load operation.
        /// </summary>
        public double MissRate
        {
            get
            {
                var requestCount = RequestCount;
                return (requestCount == 0) ? 0.0 : (double) MissCount/requestCount;
            }
        }

        /// <summary>
        ///     Returns the total number of times that <see cref="Cache{TKey,TValue}" /> lookup methods
        ///     attempted to load new values. This includes both successful load
        ///     operations, as well as those that threw exceptions.
        ///     This is defined as <code>LoadSuccessCount + LoadExceptionCount</code>.
        /// </summary>
        public long LoadCount => LoadSuccessCount + LoadExceptionCount;

        /// <summary>
        ///     Returns the number of times <see cref="Cache{TKey,TValue}" /> lookup methods have
        ///     successfully loaded a new value. This is usually incremented in
        ///     conjunction with <see cref="MissCount" />, though <see cref="MissCount" />
        ///     is also incremented when an exception is encountered during cache
        ///     loading (see <see cref="LoadExceptionCount" />). Multiple concurrent misses
        ///     for the same key will result in a single load operation. This may be
        ///     incremented not in conjunction with <see cref="MissCount" /> if the load
        ///     occurs as a result of a refresh or if the cache loader returned more
        ///     items than was requested. <see cref="MissCount" /> may also be
        ///     incremented not in conjunction with this (nor <see cref="LoadExceptionCount" />)
        ///     on calls to <see cref="M:GetIfPresent" />.
        /// </summary>
        public long LoadSuccessCount { get; }

        /// <summary>
        ///     Returns the number of times <see cref="Cache{TKey,TValue}" /> lookup methods threw an
        ///     exception while loading a new value. This is usually incremented in
        ///     conjunction with <see cref="MissCount" />, though <see cref="MissCount" />
        ///     is also incremented when cache loading completes successfully (see
        ///     <see cref="LoadSuccessCount" />). Multiple concurrent misses for
        ///     the same key will result in a single load operation. This may be
        ///     incremented not in conjunction with { @code missCount} if the load
        ///     occurs as a result of a refresh or if the cache loader returned
        ///     more items than was requested. <see cref="MissCount" /> may also be
        ///     incremented not in conjunction with this (nor <see cref="LoadSuccessCount" />)
        ///     on calls to <see cref="M:GetIfPresent" />.
        /// </summary>
        public long LoadExceptionCount { get; }
        
        /// <summary>
        ///     Returns the ratio of cache loading attempts which threw exceptions.
        ///     This is defined as
        ///     <code>LoadExceptionCount / (LoadSuccessCount + LoadExceptionCount)</code>,
        ///     or <code>0.0</code> when
        ///     <code>LoadSuccessCount+LoadExceptionCount == 0</code>.
        /// </summary>
        public double LoadExceptionRate
        {
            get
            {
                var totalLoadCount = LoadSuccessCount + LoadExceptionCount;
                return (totalLoadCount == 0) ? 0.0 : (double) LoadExceptionCount/totalLoadCount;
            }
        }

        /// <summary>
        ///     Returns the total number of nanoseconds the cache has spent loading new
        ///     values. This can be used to calculate the miss penalty. This value
        ///     is increased every time <see cref="LoadSuccessCount" /> or
        ///     <see cref="LoadExceptionCount" /> is incremented.
        /// </summary>
        public long TotalLoadTime { get; }
        
        /// <summary>
        ///     Returns the average time spent loading new values. This is defined as
        ///     <code>TotalLoadTime / (LoadSuccessCount + LoadExceptionCount)</code>.
        /// </summary>
        public double AverageLoadPenalty
        {
            get
            {
                var totalLoadCount = LoadSuccessCount + LoadExceptionCount;
                return (totalLoadCount == 0) ? 0.0 : (double) TotalLoadTime/totalLoadCount;
            }
        }

        /// <summary>
        ///     Returns the number of times an entry has been evicted. This count does
        ///     not include manual <see cref="M:Cache.Invalidate">invalidations</see>.
        /// </summary>
        public long EvictionCount { get; }
        
        /// <summary>
        ///     Returns the ratio of cache requests which were hits. This is defined as
        ///     <code>HitCount / RequestCount</code>, or <code>1.0</code> when
        ///     <code>RequestCount == 0</code>.
        ///     Note that <code>HitRate + MissRate =~ 1.0</code>.
        /// </summary>
        public double HitRate
        {
            get
            {
                var requestCount = RequestCount;
                return (requestCount == 0) ? 1.0 : (double) HitCount/requestCount;
            }
        }

        /// <summary>
        ///     Returns a new <see cref="CacheStats" /> representing the difference between
        ///     this <see cref="CacheStats" /> and <code>other</code>. Negative values,
        ///     which aren't supported by <see cref="CacheStats" />, will be
        ///     rounded up to zero.
        /// </summary>
        public CacheStats Minus(CacheStats other)
        {
            return new CacheStats(
                Math.Max(0, HitCount - other.HitCount),
                Math.Max(0, MissCount - other.MissCount),
                Math.Max(0, LoadSuccessCount - other.LoadSuccessCount),
                Math.Max(0, LoadExceptionCount - other.LoadExceptionCount),
                Math.Max(0, TotalLoadTime - other.TotalLoadTime),
                Math.Max(0, EvictionCount - other.EvictionCount));
        }

        /// <summary>
        /// Returns a new <see cref="T:CacheStats"/> representing the sum of this <see cref="T:CacheStats"/> and <paramref name="other"/>.
        /// </summary>
        public CacheStats Plus(CacheStats other)
        {
            return new CacheStats(
                HitCount + other.HitCount,
                MissCount + other.MissCount,
                LoadSuccessCount + other.LoadSuccessCount,
                LoadExceptionCount + other.LoadExceptionCount,
                TotalLoadTime + other.TotalLoadTime,
                EvictionCount + other.EvictionCount);
        }

        public override int GetHashCode()
        {
            return
                Tuple.Create(HitCount, MissCount, LoadSuccessCount, LoadExceptionCount, TotalLoadTime, EvictionCount).GetHashCode();
        }
        
        public override bool Equals(object obj)
        {
            // If parameter is null or cannot be cast to this type, then return false.
            var other = obj as CacheStats;
            if (other == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (HitCount==other.HitCount
                && MissCount==other.MissCount
                && LoadSuccessCount==other.LoadSuccessCount
                && LoadExceptionCount==other.LoadExceptionCount
                && TotalLoadTime==other.TotalLoadTime
                && EvictionCount==other.EvictionCount);
        }
        
        public override string ToString()
        {
            return ToStringHelper.GetInstance(this)
                .Add("HitCount", HitCount)
                .Add("MissCount", MissCount)
                .Add("LoadSuccessCount", LoadSuccessCount)
                .Add("LoadExceptionCount", LoadExceptionCount)
                .Add("TotalLoadTime", TotalLoadTime)
                .Add("EvictionCount", EvictionCount)
                .ToString();
        }
    }
}