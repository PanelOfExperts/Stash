using System;

namespace Stash.caches
{
    internal class NullCache : ICache
    {
        public TValue Get<TValue>(string key, Func<TValue> getter)
        {
            return getter();
        }

        public void Set<TValue>(string key, TValue value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            // Do nothing.
        }

        public int Count => 0;
    }
}