using System;
using System.Collections.Generic;

namespace Stash.caches
{
    internal class NonExpiringCache : ICache
    {
        private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public TValue Get<TValue>(string key, Func<TValue> getter)
        {
            if (string.IsNullOrEmpty(key)) return getter();

            object temp;
            if (_cache.TryGetValue(key, out temp))
            {
                if (temp is TValue)
                {
                    return (TValue) temp;
                }

                // Can store null as well.
                if (temp == null)
                {
                    return default(TValue);
                }

                // The cached value is not a TValue, try converting it.
                try
                {
                    return (TValue) Convert.ChangeType(temp, typeof (TValue));
                }
                catch (InvalidCastException)
                {
                    var message = string.Format(
                        Strings.EXCEPTION_StoredValueCannotBeConverted, temp, typeof (TValue));
                    throw new ArgumentException(message);
                }
            }

            var output = getter();
            _cache[key] = output;
            return output;
        }

        public void Set<TValue>(string key, TValue value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), Strings.EXCEPTION_KeyCannotBeNull);

            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }
            _cache.Add(key, value);
        }

        public void Clear()
        {
            _cache.Clear();
        }

        public int Count => _cache.Count;
    }
}