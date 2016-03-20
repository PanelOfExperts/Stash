using Stash.@internal;

namespace Stash.counters
{
    /// <summary>
    ///     A thread-safe <see cref="StatsCounter" /> implementation
    ///     for use by <see cref="Cache{TKey,TValue}" /> implementors.
    /// </summary>
    internal sealed class SimpleStatsCounter : StatsCounter
    {
        private readonly LongAddable _evictionCount = new LongAddable();
        private readonly LongAddable _hitCount = new LongAddable();
        private readonly LongAddable _loadExceptionCount = new LongAddable();
        private readonly LongAddable _loadSuccessCount = new LongAddable();
        private readonly LongAddable _missCount = new LongAddable();
        private readonly LongAddable _totalLoadTime = new LongAddable();

        public override void RecordHits(int count)
        {
            _hitCount.Add(count);
        }

        public override void RecordMisses(int count)
        {
            _missCount.Add(count);
        }

        public override void RecordLoadSuccess(long loadTime)
        {
            _loadSuccessCount.Increment();
            _totalLoadTime.Add(loadTime);
        }

        public override void RecordLoadException(long loadTime)
        {
            _loadExceptionCount.Increment();
            _totalLoadTime.Add(loadTime);
        }

        public override void RecordEviction()
        {
            _evictionCount.Increment();
        }

        public override CacheStats Snapshot()
        {
            return new CacheStats(
                _hitCount.Sum(),
                _missCount.Sum(),
                _loadSuccessCount.Sum(),
                _loadExceptionCount.Sum(),
                _totalLoadTime.Sum(),
                _evictionCount.Sum());
        }

        /// <summary>
        ///     Increments all counters by the values in <paramref name="other" />.
        /// </summary>
        /// <param name="other"></param>
        public override void IncrementBy(StatsCounter other)
        {
            var otherStats = other.Snapshot();
            _hitCount.Add(otherStats.HitCount);
            _missCount.Add(otherStats.MissCount);
            _loadSuccessCount.Add(otherStats.LoadSuccessCount);
            _loadExceptionCount.Add(otherStats.LoadExceptionCount);
            _totalLoadTime.Add(otherStats.TotalLoadTime);
            _evictionCount.Add(otherStats.EvictionCount);
        }
    }
}