using System.Threading;

namespace Stash.@internal
{
    /// <summary>
    ///     Abstract interface for objects that can concurrently add longs.
    /// </summary>
    /// <author>Louis Wasserman</author>
    internal sealed class LongAddable
    {
        private long _counter;

        public void Increment()
        {
            _counter = Interlocked.Increment(ref _counter);
        }

        public void Add(long x)
        {
            _counter = Interlocked.Add(ref _counter, x);
        }

        public long Sum()
        {
            return _counter;
        }
    }
}