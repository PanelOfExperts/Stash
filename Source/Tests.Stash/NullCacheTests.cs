using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Stash.Test
{
    [TestFixture]
    public class NullCacheTests
    {
        private ICache cache_;

        [SetUp]
        public void Setup()
        {
            cache_ = new CacheFactory().GetNullCache();
        }

        [Test]
        public void ReturnsGetterResultEachTime()
        {
            var counter = 0;
            Func<int> getter = () => ++counter;

            Assert.That(cache_.Get("key", getter), Is.EqualTo(1));
            Assert.That(cache_.Get("key", getter), Is.EqualTo(2));
        }
    }
}
