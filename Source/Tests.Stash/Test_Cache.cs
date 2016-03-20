using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Stash.caches;

namespace Stash.Test
{
    [TestFixture]
    class Test_Cache
    {
        internal Cache<TKey, TValue> GetUnderlyingCache<TKey, TValue>(Cache<TKey, TValue> cache)
        {
            var type = cache.GetType();
            var field = type.GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance).FirstOrDefault(p => p.Name == "_underlying");
            Assert.IsNotNull(field);
            return (Cache<TKey, TValue>) field.GetValue(cache);
        }

        internal Type GetUnderlyingCacheType<TKey, TValue>(Cache<TKey, TValue> cache)
        {
            return GetUnderlyingCache(cache).GetType();
        }

        [Test]
        public void Test_GetTimeLimitCache()
        {
            var timeout = TimeSpan.FromTicks(500);

            var sut = Cache<string, object>.GetTimeLimitCache(timeout);
            
            Assert.IsNotNull(sut);
            Assert.That(sut is ThreadSafeCache<string, object>);
            Assert.AreEqual(typeof(TimeLimitCache<string,object>), GetUnderlyingCacheType(sut));
        }

        [Test]
        public void Test_GetNonExpiringCache()
        {
            var sut = Cache<string, object>.GetNonExpiringCache();

            Assert.IsNotNull(sut);
            Assert.That(sut is ThreadSafeCache<string, object>);
            Assert.AreEqual(typeof(NonExpiringCache<string, object>), GetUnderlyingCacheType(sut));
        }
    }
}