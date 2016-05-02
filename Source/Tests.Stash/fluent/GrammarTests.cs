using NUnit.Framework;
using Stash.caches;
using Stash.fluent;
using Stash.rules;

namespace Stash.Test.fluent
{
    [TestFixture]
    class GrammarTests
    {
        [Test]
        public void Test_Chaining()
        {
            var cache = new Cache()
                .Which().IsThreadSafe()
                .And().TastesLikeChicken();
            
            Assert.That(cache is ChickenCacheWrapper);
        }

        [Test]
        public void Test_ThreadSafe()
        {
            var cache = new Cache();

            var tsCache = cache.Which().IsThreadSafe();

            Assert.That(tsCache is ThreadSafeCacheWrapper);
        }
    }
}