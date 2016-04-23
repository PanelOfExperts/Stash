using System.Threading;
using NUnit.Framework;
using Stash.caches;
using Stash.fluent;

namespace Stash.Test.caches
{
    [TestFixture]
    class ThreadSafeCacheWrapperTests
    {
        [SetUp]
        public void Setup()
        {
            _cache = new NonExpiringCache().Which().IsThreadSafe();
        }

        private readonly object lockObject = new object();

        private ICache _cache;

        private void SetManyEntries(int start)
        {
            for (var i = start; i < start + 1000; i++)
            {
                var key = $"key{i}";
                var value = i;
                _cache.Set(key, value);
            }
        }

        private void GetManyMissingEntries(int start)
        {
            for (var i = start; i < start + 1000; i++)
            {
                var key = $"key{i}";
                var value = i;
                _cache.Get(key, () => value);
            }
        }

        private void GetManyExistingEntries(int start)
        {
            for (var i = start; i < start + 1000; i++)
            {
                var key = $"key{i}";
                var value = i;
                _cache.Get(key, () => value);
            }
        }


        [Test]
        public void Clear_CheckThreadSafety()
        {
            SetManyEntries(0);

            var thread1 = new Thread(_cache.Clear);
            var thread2 = new Thread(_cache.Clear);
            var thread3 = new Thread(_cache.Clear);
            var thread4 = new Thread(_cache.Clear);

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Assert.AreEqual(0, _cache.Count);
        }

        [Test]
        public void Get_ExistingEntries_CheckThreadSafety()
        {
            _cache.Clear();
            var thread1 = new Thread(() => GetManyMissingEntries(0));
            var thread2 = new Thread(() => GetManyMissingEntries(1000));
            var thread3 = new Thread(() => GetManyExistingEntries(0));
            var thread4 = new Thread(() => GetManyExistingEntries(1000));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Assert.AreEqual(2000, _cache.Count);
            for (var i = 0; i < 2000; i++)
            {
                Assert.AreEqual(i, _cache.Get($"key{i}", () => -1));
            }
        }

        [Test]
        public void Get_MissingEntries_CheckThreadSafety()
        {
            Assert.That(_cache is ThreadSafeCacheWrapper);

            _cache.Clear();
            var thread1 = new Thread(() => GetManyMissingEntries(0));
            var thread2 = new Thread(() => GetManyMissingEntries(1000));

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();

            Assert.AreEqual(2000, _cache.Count);
        }


        [Test]
        public void Set_CheckThreadSafety()
        {
            _cache.Clear();
            var thread1 = new Thread(() => SetManyEntries(0));
            var thread2 = new Thread(() => SetManyEntries(1000));
            var thread3 = new Thread(() => SetManyEntries(0));
            var thread4 = new Thread(() => SetManyEntries(1000));

            thread1.Start();
            thread2.Start();
            thread3.Start();
            thread4.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();
            thread4.Join();

            Assert.AreEqual(2000, _cache.Count);
        }
    }
}