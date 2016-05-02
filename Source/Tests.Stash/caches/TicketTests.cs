using System;
using System.IO;
using NUnit.Framework;
using Stash.caches;
using Stash.rules;

namespace Stash.Test.caches
{
    [TestFixture]
    class TicketTests
    {
        [Test]
        public void Key_Test()
        {
            var expected = Path.GetRandomFileName();
            var rules = new ExpirationRules();
            var ticket = new Ticket(expected, rules);

            Assert.AreEqual(expected, ticket.Key);
        }

        [Test]
        public void ConstructorWithNullOrEmptyKey_Throws()
        {
            Ticket ticket = null;
            var rules = new ExpirationRules();
            Assert.Throws<ArgumentException>(()=> ticket = new Ticket(null, rules));
            Assert.Throws<ArgumentException>(() => ticket = new Ticket(string.Empty, rules));
        }
        
        [Test]
        public void ConstructorWithNullRules_Throws()
        {
            Ticket ticket = null;
            var key = Path.GetRandomFileName();
            Assert.Throws<ArgumentException>(() => ticket = new Ticket(key, null));
        }
    }
}