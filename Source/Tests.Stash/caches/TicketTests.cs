using System;
using System.Threading;
using NUnit.Framework;
using Stash.caches;

namespace Stash.Test.caches
{
    [TestFixture]
    class TicketTests
    {
        [Test]
        public void PropertiesOnConstruction()
        {
            var value = new object();
            var beforeCreationTime = DateTime.UtcNow;
            var ticket = new Ticket(value);
            var afterCreationTime = DateTime.UtcNow;

            Assert.That(ticket.Created, Is.GreaterThanOrEqualTo(beforeCreationTime).And.LessThanOrEqualTo(afterCreationTime));
            Assert.That(ticket.LastAccessed, Is.EqualTo(ticket.Created));
            Assert.AreEqual(value, ticket.Value);
        }

        [Test]
        public void LastAccessIsUpdatedOnGet()
        {
            var ticket = new Ticket(new object());

            var original = ticket.LastAccessed;
            Thread.Sleep(5); //to make sure we move enough ticks for the resolution of our measurements to notice
            var ignore = ticket.Value;
            
            Assert.That(ticket.LastAccessed, Is.GreaterThan(original));
        }

        [Test]
        public void LastAccessedNotUpdatedOnCheckingOtherProperties()
        {
            var ticket = new Ticket(new object());

            var original = ticket.LastAccessed;
            Thread.Sleep(5); //to make sure we move enough ticks for the resolution of our measurements to notice
            var ignore2 = ticket.Created;
            var ignore3 = ticket.LastAccessed;

            Assert.That(ticket.LastAccessed, Is.EqualTo(original));
        }
    }
}