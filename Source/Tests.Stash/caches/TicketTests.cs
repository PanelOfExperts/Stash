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
            var ticket = new Ticket(expected, null);

            Assert.AreEqual(expected, ticket.Key);
        }

        [Test]
        public void ConstructorWithNullOrEmptyKey_Throws()
        {
            Ticket ticket = null;
            Assert.Throws<ArgumentException>(()=> ticket = new Ticket(null, null));
            Assert.Throws<ArgumentException>(() => ticket = new Ticket(string.Empty, null));
        }
    }
}