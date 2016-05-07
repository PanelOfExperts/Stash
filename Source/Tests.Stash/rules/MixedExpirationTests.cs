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
        private int _ms;
        private TimeSpan _timeout;
        private DateTime _start;

        [SetUp]
        public void Setup()
        {
            _ms = new Random().Next(50, 100);
            _timeout = TimeSpan.FromMilliseconds(_ms);
            _start = DateTime.UtcNow;
        }

        [Test]
        public void Construct_CacheWithBothExpirations()
        {
            var timeSpan = TimeSpan.FromSeconds(2);
            var expiration = DateTime.Today + TimeSpan.FromDays(1);

            var cache = new Cache().Which().Expires().After(timeSpan)
                .OrExpires().At(expiration).WhicheverIsSooner();

            Assert.IsNotNull(cache);
            Assert.IsAssignableFrom<Cache>(cache);

            Assert.AreEqual(timeSpan, cache.ExpirationRules.SlidingExpiration);
            Assert.AreEqual(expiration, cache.ExpirationRules.AbsoluteExpiration);
        }

        [Test]
        public void When_AbsoluteExpirationComesFirst_WhicheverIsSoonerEvictsCorrectly()
        {
            // Should expire after absolute expiration
            var absolute = _start + _timeout;
            
            var cache = new Cache().Which().Expires().After(_timeout+ _timeout)
                .OrExpires().At(absolute).WhicheverIsSooner();
            cache.Set("robble", _ms);
            var expiration = absolute;

            Assert.AreEqual(1, cache.Count);
            while (DateTime.UtcNow < expiration)
            {
                Assert.AreEqual(1, cache.Count);
                Assert.AreEqual(_ms, cache.Get("robble", () => 12345));
                Thread.Sleep(5);
            }

            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }

        [Test]
        public void When_AbsoluteExpirationComesSooner_CorrectExpirationIsChosen()
        {
            // Should expire after sliding expiration
            var absolute = _start + _timeout;
            var cache = new Cache().Which().Expires().After(_timeout+ _timeout)
                .OrExpires().At(absolute).WhicheverIsSooner();
            cache.Set("robble", _ms);

            Assert.AreEqual(absolute, cache.ExpirationRules.GetNewExpiration(DateTime.UtcNow));
        }


        [Test]
        public void When_AbsoluteExpirationComesLater_CorrectExpirationIsChosen()
        {
            // Should expire after sliding expiration
            var absolute = _start + _timeout + _timeout;
            var cache = new Cache().Which().Expires().After(_timeout)
                .OrExpires().At(absolute).WhicheverIsLater();
            cache.Set("robble", _ms);

            Assert.AreEqual(absolute, cache.ExpirationRules.GetNewExpiration(DateTime.UtcNow));
        }



        [Test]
        public void When_AbsoluteExpirationComesLater_WhicheverIsLaterEvictsCorrectly()
        {
            // Should expire after absolute expiration
            var absolute = _start+_timeout+_timeout;
            var cache = new Cache().Which().Expires().After(_timeout)
                .OrExpires().At(absolute).WhicheverIsLater();
            cache.Set("robble", _ms);

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
        public void SlidingExpiration_ExtendsWhenAccessed_AndWhicheverIsLaterEvictsCorrectly()
        {
            var absolute = _start + _timeout;
            var cache = new Cache().Which().Expires().At(absolute)
                .OrExpires().After(_timeout).WhicheverIsLater();
            cache.Set("robble", _ms);
            var expiration = DateTime.UtcNow + _timeout;
            while (DateTime.UtcNow <= expiration)
            {
                Assert.AreEqual(1, cache.Count);
                Assert.AreEqual(_ms, cache.Get("robble", () => 12345));
                Thread.Sleep(5);
            }

            Assert.AreEqual(1, cache.Count);
            Assert.AreEqual(_ms, cache.Get("robble", () => 12345));
        }

        [Test]
        public void When_SlidingExpirationComesFirst_WhicheverIsSoonerEvictsCorrectly()
        {
            // Should expire after sliding expiration
            var absolute = _start + _timeout + _timeout;
            var cache = new Cache().Which().Expires().At(absolute)
                .OrExpires().After(_timeout).WhicheverIsSooner();
            cache.Set("robble", _ms);
            var expiration = DateTime.UtcNow + _timeout;
            
            // Don't look at the value, or it'll reset the LastAccessedTime. Ha.
            while (DateTime.UtcNow <= expiration)
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
            
            var absolute = DateTime.UtcNow;
            var cache = new Cache().Which().Expires().At(absolute)
                .OrExpires().After(_timeout).WhicheverIsLater();
            cache.Set("robble", _ms);
            var expiration = DateTime.UtcNow + _timeout;
            Assert.AreEqual(1, cache.Count);

            // Don't look at the value, or it'll reset the LastAccessedTime. Ha.
            while (DateTime.UtcNow <= expiration)
            {
                Assert.AreEqual(1, cache.Count);
                Thread.Sleep(5);
            }

            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }
    }
}