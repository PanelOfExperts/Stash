using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Stash.caches;
using Stash.fluent;
using Stash.rules;

namespace Stash.Test.rules
{
    [TestFixture]
    internal class MixedExpirationTests
    {
        [Test]
        public void Construct_CacheWithBothExpirations()
        {
            var timeSpan = TimeSpan.FromSeconds(2);
            var expiration = DateTime.Today + TimeSpan.FromDays(1);

            var cache = new Cache().Which().Expires().After(timeSpan)
                .OrExpires().At(expiration).WhicheverIsSooner();

            Assert.IsNotNull(cache);
            Assert.IsAssignableFrom<Cache>(cache);

            Assert.AreEqual(timeSpan, cache.ExpirationRules.SlidingTimeSpan);
            Assert.AreEqual(expiration, cache.ExpirationRules.AbsoluteExpiration);
        }

        [Test]
        public void When_AbsoluteExpirationComesFirst_WhicheverIsSoonerEvictsCorrectly()
        {
            // Should expire after absolute expiration
            //var counter = 0;
            var ms = new Random().Next(50, 100);
            Trace.WriteLine($"ms: {ms}");
            var absolute = DateTime.UtcNow + TimeSpan.FromMilliseconds(ms);
            var timeout = TimeSpan.FromSeconds(ms + 2);
            var cache = new Cache().Which().Expires().After(timeout)
                .OrExpires().At(absolute).WhicheverIsSooner();
            cache.Set("robble", ms);

            Assert.AreEqual(1, cache.Count);
            while (DateTime.UtcNow < absolute)
            {
                Assert.AreEqual(1, cache.Count);
                Assert.AreEqual(ms, cache.Get("robble", () => 12345));
                Thread.Sleep(5);
            }

            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }

        [Test]
        public void When_AbsoluteExpirationComesLater_WhicheverIsLaterEvictsCorrectly()
        {
            // Should expire after absolute expiration
            var ms = new Random().Next(50, 100);
            var timeout = TimeSpan.FromMilliseconds(ms);
            var absolute = DateTime.UtcNow + TimeSpan.FromMilliseconds(ms + 50);
            var cache = new Cache().Which().Expires().After(timeout)
                .OrExpires().At(absolute).WhicheverIsLater();
            cache.Set("robble", ms);
            Assert.AreEqual(1, cache.Count);

            while (DateTime.UtcNow < absolute)
            {
                Assert.AreEqual(1, cache.Count);
                //Assert.AreEqual(ms, cache.Get("robble", () => 12345));
                Thread.Sleep(5);
            }

            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }

        [Test]
        public void When_ConstructCacheWithTwoAbsoluteExpirations_Throws()
        {
            var expiration1 = DateTime.Today + TimeSpan.FromDays(1);
            var expiration2 = DateTime.Today + TimeSpan.FromDays(2);

            MixedExpiration.RulesHolder rules = null;
            Assert.Throws<InvalidOperationException>(() => rules = new Cache().Which().Expires().At(expiration1)
                .OrExpires().At(expiration2));
        }

        [Test]
        public void When_ConstructCacheWithTwoSlidingExpirations_Throws()
        {
            var timeSpan1 = TimeSpan.FromSeconds(2);
            var timeSpan2 = TimeSpan.FromMilliseconds(5);

            MixedExpiration.RulesHolder rules = null;
            Assert.Throws<InvalidOperationException>(() => rules = new Cache().Which().Expires().After(timeSpan1)
                .OrExpires().After(timeSpan2));
        }

        [Test]
        public void When_LookingAtValue_SlidingExpirationCanPushExpirationDateOut_AndWhicheverIsLaterEvictsCorrectly()
        {
            // Should expire after sliding expiration
            var ms = new Random().Next(25, 50);
            var startTime = DateTime.UtcNow;
            var absolute = startTime + TimeSpan.FromMilliseconds(ms);
            var timeout = TimeSpan.FromMilliseconds(ms + 10);
            var cache = new Cache().Which().Expires().At(absolute)
                .OrExpires().After(timeout).WhicheverIsLater();
            var ticket = cache.Set("robble", ms);
            var expectedTimeout = ticket.LastAccessedDate + timeout;
            Assert.AreEqual(1, cache.Count);

            // Don't look at the value, or it'll reset the LastAccessedTime. Ha.
            while (DateTime.UtcNow <= expectedTimeout)
            {
                Assert.AreEqual(1, cache.Count);
                Assert.AreEqual(ms, cache.Get("robble", () => 12345));
                Thread.Sleep(5);
            }

            Assert.AreEqual(1, cache.Count);
            Assert.AreEqual(ms, cache.Get("robble", () => 12345));
        }

        [Test]
        public void When_SlidingExpirationComesFirst_WhicheverIsSoonerEvictsCorrectly()
        {
            // Should expire after sliding expiration
            var startTime = DateTime.UtcNow;
            var ms = new Random().Next(25, 50);
            var timeout = TimeSpan.FromMilliseconds(ms);
            var absolute = startTime + TimeSpan.FromSeconds(ms + 1);
            var cache = new Cache().Which().Expires().At(absolute)
                .OrExpires().After(timeout).WhicheverIsSooner();
            var ticket = cache.Set("robble", ms);
            var expectedTimeout = ticket.LastAccessedDate + timeout;
            Assert.AreEqual(1, cache.Count);

            // Don't look at the value, or it'll reset the LastAccessedTime. Ha.
            while (DateTime.UtcNow <= expectedTimeout)
            {
                Assert.AreEqual(1, cache.Count);
                Thread.Sleep(5);
            }

            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }

        [Test]
        public void When_SlidingExpirationComesLater_WhicheverIsLaterEvictsCorrectly()
        {
            // Should expire after sliding expiration
            var ms = new Random().Next(25, 50);
            var startTime = DateTime.UtcNow;
            var absolute = startTime + TimeSpan.FromMilliseconds(ms);
            var timeout = TimeSpan.FromMilliseconds(ms + 10);
            var cache = new Cache().Which().Expires().At(absolute)
                .OrExpires().After(timeout).WhicheverIsLater();
            var ticket = cache.Set("robble", ms);
            var expectedTimeout = ticket.LastAccessedDate + timeout;
            Assert.AreEqual(1, cache.Count);

            // Don't look at the value, or it'll reset the LastAccessedTime. Ha.
            while (DateTime.UtcNow <= expectedTimeout)
            {
                Assert.AreEqual(1, cache.Count);
                Thread.Sleep(5);
            }

            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }
    }
}