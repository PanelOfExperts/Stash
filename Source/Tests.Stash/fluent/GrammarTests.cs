using NUnit.Framework;
using Stash.caches;
using Stash.fluent;

namespace Stash.Test.fluent
{
    [TestFixture]
    class GrammarTests
    {
        [Test]
        public void Test_ThreadSafe()
        {
            var cache = new NonExpiringCache();

            var tsCache = cache.WhichIs().ThreadSafe();

            Assert.That(tsCache is ThreadSafeCacheWrapper);
        }
    }
}