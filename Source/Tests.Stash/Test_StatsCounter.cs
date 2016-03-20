using NUnit.Framework;
using Stash.counters;

namespace Stash.Test
{
    [TestFixture]
    class Test_StatsCounter
    {
        [Test]
        public void Test_GetSimpleCounter()
        {
            var sut = StatsCounter.GetSimpleCounter();
            Assert.IsNotNull(sut);
            Assert.That(sut is SimpleStatsCounter);
        }

        [Test]
        public void Test_GetNullCounter()
        {
            var sut = StatsCounter.GetNullCounter();
            Assert.IsNotNull(sut);
            Assert.That(sut is NullStatsCounter);
        }
    }
}