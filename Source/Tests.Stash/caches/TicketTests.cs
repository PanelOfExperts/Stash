using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Stash.caches;
using Stash.rules;

namespace Stash.Test.caches
{
    [TestFixture]
    class TicketTests
    {
        [Test]
        public void PropertiesOnConstruction()
        {
            var key = Path.GetRandomFileName();
            var value = new object();
            var beforeCreationTime = DateTime.UtcNow;
            var ticket = new Ticket(key, value);
            var afterCreationTime = DateTime.UtcNow;

            Assert.That(ticket.Created, Is.GreaterThanOrEqualTo(beforeCreationTime).And.LessThanOrEqualTo(afterCreationTime));
            Assert.That(ticket.LastAccessed, Is.EqualTo(ticket.Created));
            Assert.AreEqual(key, ticket.Key);
            Assert.AreEqual(value, ticket.Value);
        }

        [Test]
        public void LastAccessIsUpdatedOnGet()
        {
            var ticket = new Ticket(Guid.NewGuid().ToString(), new object());

            var original = ticket.LastAccessed;
            Thread.Sleep(5); //to make sure we move enough ticks for the resolution of our measurements to notice
            var ignore = ticket.Value;
            
            Assert.That(ticket.LastAccessed, Is.GreaterThan(original));
        }

        [Test]
        public void LastAccessedNotUpdatedOnCheckingOtherProperties()
        {
            var ticket = new Ticket(Guid.NewGuid().ToString(), new object());

            var original = ticket.LastAccessed;
            Thread.Sleep(5); //to make sure we move enough ticks for the resolution of our measurements to notice
            var ignore1 = ticket.Key;
            var ignore2 = ticket.Created;
            var ignore3 = ticket.LastAccessed;

            Assert.That(ticket.LastAccessed, Is.EqualTo(original));
        }

        [Test]
        public void ConstructorWithNullOrEmptyKey_Throws()
        {
            Assert.Throws<ArgumentException>(()=> new Ticket(null, null));
            Assert.Throws<ArgumentException>(() => new Ticket(string.Empty, null));
        }
    }
}