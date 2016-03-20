 // ReSharper disable InconsistentNaming

namespace Stash.counters
{
    internal sealed class NullStatsCounter : StatsCounter
    {
        private static readonly CacheStats EMPTY_STATS = new CacheStats(0, 0, 0, 0, 0, 0);

        public override void RecordHits(int count)
        {
            // Do nothing.
        }

        public override void RecordMisses(int count)
        {
            // Do nothing.
        }

        public override void RecordLoadSuccess(long loadTime)
        {
            // Do nothing.
        }

        public override void RecordLoadException(long loadTime)
        {
            // Do nothing.
        }

        public override void RecordEviction()
        {
            // Do nothing.
        }

        public override CacheStats Snapshot()
        {
            return EMPTY_STATS;
        }

        public override void IncrementBy(StatsCounter other)
        {
            // Do nothing.
        }
    }
}