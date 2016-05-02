using System;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Stash.caches;

namespace Stash.Test.caches
{
    [TestFixture]
    internal class RulesCacheTests
    {
        private ICache _cache;

        private string _key;
        private int _counter;
        private Func<object> _getter;
        private TimeSpan _timeout;
        private Func<Ticket, bool> _evictionRule;

        //private readonly TimeSpan _timeout;
        //private readonly Func<DateTime> _now;
        //_now = () => DateTime.UtcNow;
        //_builder = (value) => Ticket.GetInstance(value, _now());
        [SetUp]
        public void SetUp()
        {
            _key = Path.GetRandomFileName();
            _counter = 0;
            _getter = () => ++_counter;
            
            _timeout = TimeSpan.FromMilliseconds(250);
            _evictionRule = (ticket) => DateTime.UtcNow > ticket.Created + _timeout;

            _cache = new Cache(_evictionRule);
        }

        [Test]
        public void WhenSuppliedKeyIsNullOrEmpty_PutThrowsError()
        {
            var expectedValue = new Random().Next();

            Assert.Throws<ArgumentNullException>(() => _cache.Set(null, expectedValue));
            Assert.Throws<ArgumentNullException>(() => _cache.Set(string.Empty, expectedValue));
        }

        [Test]
        public void WhenSuppliedIsNullOrEmpty_ReturnsValueFromGetter()
        {
            var counter = 0;
            Func<int> getter = () => ++counter;

            Assert.AreEqual(1, _cache.Get(null, getter));
            Assert.AreEqual(2, _cache.Get(null, getter));
            Assert.AreEqual(3, _cache.Get(string.Empty, getter));
            Assert.AreEqual(4, _cache.Get(string.Empty, getter));
        }

        [Test]
        public void WhenNullValueIsPut_GetReturnsNull()
        {
            string expectedValue = null;
            Func<string> getter = () => "abc123";
            var key = Path.GetRandomFileName();

            _cache.Set(key, expectedValue);

            Assert.AreEqual(expectedValue, _cache.Get(key, getter));
        }

        [Test]
        public void WhenValueIsPut_GetReturnsThatValue()
        {
            var expectedValue = new Random().Next();
            Func<int> getter = () => ++expectedValue;
            var key = Path.GetRandomFileName();

            _cache.Set(key, expectedValue - 1);
            _cache.Set(key, expectedValue);

            Assert.AreEqual(expectedValue, _cache.Get(key, getter));
        }

        [Test]
        public void IfStoredTypeCanBeConvertedToTValue_ThenConvertIt()
        {
            var expectedValue = new Random().Next();
            Func<string> getter = () => "abc123";
            var key = Path.GetRandomFileName();

            // put an int in
            _cache.Set(key, expectedValue);

            // get a string out?  Should be OK
            string actual;
            actual = _cache.Get<string>(key, null);
            Assert.AreEqual(expectedValue.ToString(), actual);
        }

        [Test]
        public void IfStoredTypeCannotBeConvertedToTValue_ThenThrow()
        {
            var expectedValue = new Random().Next();
            Func<DateTime> getter = () => DateTime.Now;
            var key = Path.GetRandomFileName();

            // put an int in
            _cache.Set(key, expectedValue);

            // Get a date/time out?  Nope.
            DateTime actual;
            Assert.Throws<ArgumentException>(() => actual = _cache.Get(key, getter));
        }

        [Test]
        public void WhenKeyIsMissing_GetStoresAndReturnsValueFromGetter()
        {
            var key = Path.GetRandomFileName();
            var counter = 0;
            Func<int> getter = () => ++counter;

            Assert.AreEqual(1, _cache.Get(key, getter));
            Assert.AreEqual(1, _cache.Get(key, getter));
            Assert.AreEqual(1, _cache.Get(key, getter));
        }

        [Test]
        public void Clear_EmptiesCache()
        {
            Assert.AreEqual(0, _cache.Count);
            var expected = new Random().Next(50, 100);
            for (var i = 1; i <= expected; i++)
            {
                _cache.Set($"key{i}", Path.GetRandomFileName());
                Assert.AreEqual(i, _cache.Count);
            }
            Assert.AreEqual(expected, _cache.Count);

            _cache.Clear();

            Assert.AreEqual(0, _cache.Count);
        }

        [Test]
        public void Size_ReportsSize()
        {
            Assert.AreEqual(0, _cache.Count);
            var expected = new Random().Next(50, 100);

            for (var i = 1; i <= expected; i++)
            {
                _cache.Set($"key{i}", Path.GetRandomFileName());
                Assert.AreEqual(i, _cache.Count);
            }
            Assert.AreEqual(expected, _cache.Count);
        }

        [Test]
        public void Count_checksAndEvicts()
        {
            //var cache = new Cache(_ticketBuilder, _evictionRule);
            Assert.AreEqual(0, _cache.Count);
            _cache.Set(_key, _getter);
            Assert.AreEqual(1, _cache.Count);

            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            
            Assert.AreEqual(0, _cache.Count);
        }

        [Test]
        public void Get_appliesEvictionRule()
        {
            _counter = 0;

            // doesn't exist, get new thing
            var actual = _cache.Get(_key, _getter);
            Assert.AreEqual(1, actual);

            // Exists and not expired, get existing value
            actual = _cache.Get(_key, _getter);
            Assert.AreEqual(1, actual); 

            Thread.Sleep(TimeSpan.FromMilliseconds(500));

            // Exists but expired, get new value
            actual = _cache.Get(_key, _getter);
            Assert.AreEqual(2, actual);

            // Exists and not expired, get existing value
            actual = _cache.Get(_key, _getter);
            Assert.AreEqual(2, actual);
        }
    }
}