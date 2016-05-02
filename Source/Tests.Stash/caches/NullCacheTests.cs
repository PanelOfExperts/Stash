using System;
using NUnit.Framework;
using Stash.caches;

namespace Stash.Test.caches
{
    [TestFixture]
    public class NullCacheTests
    {
        [SetUp]
        public void Setup()
        {
            cache_ = new NullCache();
        }

        private ICache cache_;

        [Test]
        public void Clear_DoesNothing()
        {
            Assert.DoesNotThrow(() => cache_.Clear());
        }

        [Test]
        public void Put_ThrowsNotImplementedException()
        {
            Func<int> getter = null;

            Assert.Throws<NotImplementedException>(() => cache_.Set("key", getter));
        }

        [Test]
        public void ReturnsGetterResultEachTime()
        {
            var counter = 0;
            Func<int> getter = () => ++counter;

            Assert.That(cache_.Get("key", getter), Is.EqualTo(1));
            Assert.That(cache_.Get("key", getter), Is.EqualTo(2));
        }

        [Test]
        public void Size_ReportsZero()
        {
            var counter = 0;
            Func<int> getter = () => ++counter;

            Assert.AreEqual(0, cache_.Count);
            cache_.Get("key", getter);
            cache_.Get("key", getter);
            cache_.Get("key", getter);
            Assert.AreEqual(0, cache_.Count);
        }
    }
}