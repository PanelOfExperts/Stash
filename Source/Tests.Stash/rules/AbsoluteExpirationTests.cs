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
    public class AbsoluteExpirationTests
    {
        [Test]
        public void Construct_CacheWithAbsoluteExpiration()
        {
            var expirationDate = DateTime.UtcNow + TimeSpan.FromMilliseconds(250);

            var cache = new Cache().Which().Expires().At(expirationDate);
            Assert.IsNotNull(cache);
            Assert.IsAssignableFrom<Cache>(cache);
        }
        
        [Test]
        public void When_YouGetTheTicketValue_TheAbsoluteExpirationTimeDoesNotReset()
        {
            // Add an item
            // check it: should be there
            // wait until creation time + timeout
            // check it: shouldn't be there
            var ms = new Random().Next(50, 250);
            var expirationDate = DateTime.UtcNow + TimeSpan.FromMilliseconds(ms);
            var cache = new Cache().Which().Expires().At(expirationDate);
            cache.Set("robble", ms);

            // Don't look at the value, or it'll reset the LastAccessedTime. Ha.
            while (DateTime.UtcNow < expirationDate)
            {
                Assert.AreEqual(1, cache.Count);
                Assert.AreEqual(ms, cache.Get("robble", () => 12345));
                Thread.Sleep(50);
            }
            
            Assert.AreEqual(0, cache.Count);
            Assert.AreEqual(12345, cache.Get("robble", () => 12345));
        }
    }
}