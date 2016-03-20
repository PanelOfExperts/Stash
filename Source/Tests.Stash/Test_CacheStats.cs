using System;
using NUnit.Framework;

namespace Stash.Test
{
    [TestFixture]
    public class Test_CacheStats
    {
        const int hits = 3;
        const int misses = 5;
        const int evictions = 7;
        const int time = 11;
        const int successes = 13;
        const int exceptions = 15;


        private CacheStats RandomishCacheStats()
        {
            return new CacheStats(hits, misses, successes, exceptions, time, evictions);
        }

        private void Test_Equality(CacheStats x, CacheStats y, CacheStats z)
        {
            // x = x
            Assert.AreEqual(x, x);
            Assert.AreEqual(y, y);
            Assert.AreEqual(z, z);

            // x = y  ==>  y = z
            Assert.AreEqual(x.Equals(y), y.Equals(x));
            Assert.AreEqual(x.Equals(z), z.Equals(x));
            Assert.AreEqual(y.Equals(z), z.Equals(y));

            // if (x = y) && (y = z)  ==>  (x = z)
            Assert.AreEqual(x.Equals(y) && y.Equals(z), x.Equals(z));

            // Successive invocations of x.Equals(y) return the same value
            var expected = x.Equals(y);
            for (var index = 0; index < 1000; index++)
            {
                Assert.AreEqual(expected, x.Equals(y));
            }

            // x.Equals(null) returns false.
            Assert.AreEqual(false, x.Equals(null));
        }

        private CacheStats Vector(int scalar)
        {
            return new CacheStats(hits*scalar, misses*scalar, successes*scalar, exceptions*scalar, time*scalar,
                evictions*scalar);
        }

        [Test]
        public void Test_AverageLoadPenalty_NonZero()
        {
            var sut = RandomishCacheStats();

            Assert.AreEqual((double) time/(successes + exceptions), sut.AverageLoadPenalty);
            Assert.AreEqual((double) sut.TotalLoadTime/sut.LoadCount, sut.AverageLoadPenalty);
        }

        [Test]
        public void Test_AverageLoadPenalty_Zero()
        {
            var sut = CacheStats.Empty;
            Assert.AreEqual(0.0, sut.AverageLoadPenalty);
        }

        [Test]
        public void Test_Constructor()
        {
            var sut = RandomishCacheStats();

            Assert.IsNotNull(sut);
            Assert.AreEqual(hits, sut.HitCount);
            Assert.AreEqual(misses, sut.MissCount);
            Assert.AreEqual(evictions, sut.EvictionCount);
            Assert.AreEqual(time, sut.TotalLoadTime);
            Assert.AreEqual(successes, sut.LoadSuccessCount);
            Assert.AreEqual(exceptions, sut.LoadExceptionCount);
        }

        [Test]
        public void Test_Constructor_DoesNotSupportNegatives()
        {
            // ReSharper disable once NotAccessedVariable
            CacheStats sut;
            Assert.DoesNotThrow(() => sut = new CacheStats(hits, misses, successes, exceptions, time, evictions));

            Assert.Throws<ArgumentException>(
                () => sut = new CacheStats(-hits, misses, successes, exceptions, time, evictions));

            Assert.Throws<ArgumentException>(
                () => sut = new CacheStats(hits, -misses, successes, exceptions, time, evictions));

            Assert.Throws<ArgumentException>(
                () => sut = new CacheStats(hits, misses, -successes, exceptions, time, evictions));

            Assert.Throws<ArgumentException>(
                () => sut = new CacheStats(hits, misses, successes, -exceptions, time, evictions));

            Assert.Throws<ArgumentException>(
                () => sut = new CacheStats(hits, misses, successes, exceptions, -time, evictions));

            Assert.Throws<ArgumentException>(
                () => sut = new CacheStats(hits, misses, successes, exceptions, time, -evictions));
        }

        [Test]
        public void Test_Empty()
        {
            var sut = CacheStats.Empty;

            Assert.IsNotNull(sut);
            Assert.AreEqual(0, sut.HitCount);
            Assert.AreEqual(0, sut.MissCount);
            Assert.AreEqual(0, sut.EvictionCount);
            Assert.AreEqual(0, sut.TotalLoadTime);
            Assert.AreEqual(0, sut.LoadSuccessCount);
            Assert.AreEqual(0, sut.LoadExceptionCount);
        }

        [Test]
        public void Test_Equals()
        {
            const int i = 3;
            const int j = 5;

            var one = Vector(i);
            var two = Vector(i);
            var three = Vector(j);

            Test_Equality(one, two, three);
        }

        [Test]
        public void Test_GetHashCode()
        {
            var one = RandomishCacheStats();
            var two = RandomishCacheStats();
            Assert.That(!ReferenceEquals(one, two));
            Assert.AreEqual(one.GetHashCode(), two.GetHashCode());
        }

        [Test]
        public void Test_HitRate_NonZero()
        {
            var sut = RandomishCacheStats();

            Assert.AreEqual((double) hits/(hits + misses), sut.HitRate);
            Assert.AreEqual((double) sut.HitCount/sut.RequestCount, sut.HitRate);
        }

        [Test]
        public void Test_HitRate_Zero()
        {
            var sut = CacheStats.Empty;
            Assert.AreEqual(1.0, sut.HitRate);
        }

        [Test]
        public void Test_LoadCount()
        {
            var sut = RandomishCacheStats();
            Assert.AreEqual(successes + exceptions, sut.LoadCount);
            Assert.AreEqual(sut.LoadSuccessCount + sut.LoadExceptionCount, sut.LoadCount);
        }

        [Test]
        public void Test_LoadExceptionRate_NonZero()
        {
            var sut = RandomishCacheStats();
            Assert.AreEqual((double) exceptions/(successes + exceptions), sut.LoadExceptionRate);
            Assert.AreEqual((double) sut.LoadExceptionCount/sut.LoadCount, sut.LoadExceptionRate);
        }

        [Test]
        public void Test_LoadExceptionRate_Zero()
        {
            var sut = CacheStats.Empty;
            Assert.AreEqual(0.0, sut.LoadExceptionRate);
        }

        [Test]
        public void Test_Minus_BigStats()
        {
            var i = 7;
            var j = 3;

            var one = new CacheStats(hits*i, misses*i, successes*i, exceptions*i, time*i, evictions*i);
            var two = new CacheStats(hits*j, misses*j, successes*j, exceptions*j, time*j, evictions*j);

            var sut = one.Minus(two);

            Assert.IsNotNull(sut);
            Assert.AreEqual((i - j)*hits, sut.HitCount);
            Assert.AreEqual((i - j)*misses, sut.MissCount);
            Assert.AreEqual((i - j)*evictions, sut.EvictionCount);
            Assert.AreEqual((i - j)*time, sut.TotalLoadTime);
            Assert.AreEqual((i - j)*successes, sut.LoadSuccessCount);
            Assert.AreEqual((i - j)*exceptions, sut.LoadExceptionCount);
        }

        [Test]
        public void Test_Minus_Negatives()
        {
            var one = CacheStats.Empty;
            var two = RandomishCacheStats();

            var sut = one.Minus(two);

            Assert.IsNotNull(sut);
            Assert.AreEqual(0, sut.HitCount);
            Assert.AreEqual(0, sut.MissCount);
            Assert.AreEqual(0, sut.EvictionCount);
            Assert.AreEqual(0, sut.TotalLoadTime);
            Assert.AreEqual(0, sut.LoadSuccessCount);
            Assert.AreEqual(0, sut.LoadExceptionCount);
        }

        [Test]
        public void Test_Minus_ToZero()
        {
            var one = RandomishCacheStats();
            var two = RandomishCacheStats();

            var sut = one.Minus(two);

            Assert.IsNotNull(sut);
            Assert.AreEqual(0, sut.HitCount);
            Assert.AreEqual(0, sut.MissCount);
            Assert.AreEqual(0, sut.EvictionCount);
            Assert.AreEqual(0, sut.TotalLoadTime);
            Assert.AreEqual(0, sut.LoadSuccessCount);
            Assert.AreEqual(0, sut.LoadExceptionCount);
        }

        [Test]
        public void Test_MissRate_NonZero()
        {
            var sut = RandomishCacheStats();
            Assert.AreEqual((double) misses/(hits + misses), sut.MissRate);
            Assert.AreEqual((double) sut.MissCount/(sut.RequestCount), sut.MissRate);
        }

        [Test]
        public void Test_MissRate_Zero()
        {
            var sut = CacheStats.Empty;
            Assert.AreEqual(0.0, sut.MissRate);
        }


        [Test]
        public void Test_Plus()
        {
            var i = 7;
            var j = 3;

            var one = new CacheStats(hits*i, misses*i, successes*i, exceptions*i, time*i, evictions*i);
            var two = new CacheStats(hits*j, misses*j, successes*j, exceptions*j, time*j, evictions*j);

            var sut = one.Plus(two);

            Assert.IsNotNull(sut);
            Assert.AreEqual((i + j)*hits, sut.HitCount);
            Assert.AreEqual((i + j)*misses, sut.MissCount);
            Assert.AreEqual((i + j)*evictions, sut.EvictionCount);
            Assert.AreEqual((i + j)*time, sut.TotalLoadTime);
            Assert.AreEqual((i + j)*successes, sut.LoadSuccessCount);
            Assert.AreEqual((i + j)*exceptions, sut.LoadExceptionCount);
        }

        [Test]
        public void Test_RequestCount()
        {
            var sut = RandomishCacheStats();
            Assert.AreEqual(hits + misses, sut.RequestCount);
            Assert.AreEqual(sut.HitCount + sut.MissCount, sut.RequestCount);
        }

        [Test]
        public void Test_ToString()
        {
            var sut = RandomishCacheStats();
            var expected =
                $"CacheStats{{HitCount={hits}, MissCount={misses}, LoadSuccessCount={successes}, LoadExceptionCount={exceptions}, TotalLoadTime={time}, EvictionCount={evictions}}}";
            Assert.AreEqual(expected, sut.ToString());
        }
    }
}