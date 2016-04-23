using NUnit.Framework;
using Stash.caches;
using Stash.fluent;

namespace Stash.Test.fluent
{
    [TestFixture]
    class GrammarTests
    {
        [Test]
        public void Test_Chaining()
        {
            var cache = new NonExpiringCache()
                .Which().IsThreadSafe()
                .And().TastesLikeChicken();
            
            Assert.That(cache is ChickenCacheWrapper);
        }

        [Test]
        public void Test_ThreadSafe()
        {
            var cache = new NonExpiringCache();

            var tsCache = cache.Which().IsThreadSafe();

            Assert.That(tsCache is ThreadSafeCacheWrapper);
        }
    }
}