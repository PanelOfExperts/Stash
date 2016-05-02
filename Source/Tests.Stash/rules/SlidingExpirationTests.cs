using System;
using System.Threading;
using NUnit.Framework;
using Stash.caches;
using Stash.fluent;
using Stash.rules;

namespace Stash.Test.rules
{
    /// <summary>
    ///     Sliding Expiration:
    ///     Specifies how long after an item was last accessed that it expires.
    ///     For example, you can set an item to expire 15 min after it was last accessed in the cache
    /// </summary>
    [TestFixture]
    public class SlidingExpirationTests
    {
        [Test]
        public void Construct_CacheWithSlidingExpiration()
        {
            var cache = new Cache().Which().Expires().After(TimeSpan.FromSeconds(2));
            Assert.IsNotNull(cache);
            Assert.IsAssignableFrom<Cache>(cache);
        }

        [Test]
        public void When_YouDoNotAccessTheValueBeforeTheSlidingExpiration_TicketExpires()
        {
            // Add an item
            // check it: should be there
            // wait until creation time + timeout
            // check it: shouldn't be there
            var ms = new Random().Next(50, 250);
            var timeout = TimeSpan.FromMilliseconds(ms);
            var cache = new Cache().Which().Expires().After(timeout);
            var ticket = cache.Set("robble", ms);
            
            // Don't look at the value, or it'll reset the LastAccessedTime. Ha.
            while (DateTime.UtcNow < ticket.Expiration)
            {
                Assert.AreEqual(1, cache.Count);
                Thread.Sleep(50);
            }
            
            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }

        [Test]
        public void When_YouGetTheTicketValue_TheSlidingExpirationResets()
        {
            // Add an item
            // check it: should be there
            // wait until creation time + timeout
            // check it: shouldn't be there
            var ms = new Random().Next(50, 250);
            var timeout = TimeSpan.FromMilliseconds(ms);
            var cache = new Cache().Which().Expires().After(timeout);
            var ticket = cache.Set("robble", ms);
            var expiration = ticket.Expiration;

            // Look at the value, so it resets the Expiration.
            while (DateTime.UtcNow < expiration)
            {
                Assert.AreEqual(1, cache.Count);
                Assert.AreEqual(ms, cache.Get("robble", () => 12345));
                Thread.Sleep(5);
            }
            
            Assert.AreEqual(1, cache.Count);
            Assert.AreEqual(ms, cache.Get("robble", () => 12345));
        }
    }
}