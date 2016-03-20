using NUnit.Framework;
using Stash.@internal;

namespace Stash.Test.@internal
{
    [TestFixture]
    public class Test_LongAddable
    {
        [Test]
        public void Test_Add()
        {
            const int iterations = 5;
            const int valueToAdd = 17;
            var sut = new LongAddable();

            for (var i = 1; i <= iterations; i++)
            {
                sut.Add(valueToAdd);
                Assert.AreEqual(i*valueToAdd, sut.Sum());
            }
        }

        [Test]
        public void Test_Constructor()
        {
            var sut = new LongAddable();
            Assert.IsNotNull(sut);
            Assert.AreEqual(0, sut.Sum());
        }

        [Test]
        public void Test_Increment()
        {
            var sut = new LongAddable();
            sut.Increment();
            Assert.AreEqual(1, sut.Sum());
        }
    }
}