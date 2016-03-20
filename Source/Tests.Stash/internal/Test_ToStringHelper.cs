using NUnit.Framework;
using Stash.@internal;

namespace Stash.Test.@internal
{
    [TestFixture]
    internal class Test_ToStringHelper
    {
        private static readonly CacheStats testObject = CacheStats.Empty;

        [Test]
        public void Test_Add_bool()
        {
            var name = "bob";
            var value = true;
            var expected = $"CacheStats{{{name}={value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_char()
        {
            var name = "bob";
            var value = 'u';
            var expected = $"CacheStats{{{name}={value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_double()
        {
            var name = "bob";
            var value = 331.4;
            var expected = $"CacheStats{{{name}={value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_float()
        {
            var name = "bob";
            var value = (float) 331.4;
            var expected = $"CacheStats{{{name}={value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_int()
        {
            var name = "bob";
            var value = (int) 331.4;
            var expected = $"CacheStats{{{name}={value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_long()
        {
            var name = "bob";
            var value = (long) 227.9;
            var expected = $"CacheStats{{{name}={value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_LongArray()
        {
            var name = "bob";
            var value = new long[] {1, 2, 3, 4, 5};
            var expected = $"CacheStats{{{name}=1, 2, 3, 4, 5}}";

            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);
            var actual = sut.ToString();
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_MultipleItems()
        {
            var name1 = "alice";
            var name2 = "bob";
            var value1 = (long) 227.9;
            var value2 = (int) 324.7;
            var expected = $"CacheStats{{{name1}={value1}, {name2}={value2}}}";
            var sut = ToStringHelper.GetInstance(testObject)
                .Add(name1, value1)
                .Add(name2, value2);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Add_string()
        {
            var name = "bob";
            var value = "uncle";
            var expected = $"CacheStats{{{name}={value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.Add(name, value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_AddValue_bool()
        {
            var value = true;
            var expected = $"CacheStats{{{value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.AddValue(value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_AddValue_char()
        {
            var value = 'v';
            var expected = $"CacheStats{{{value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.AddValue(value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_AddValue_double()
        {
            var value = 331.4;
            var expected = $"CacheStats{{{value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.AddValue(value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_AddValue_float()
        {
            var value = (float) 331.4;
            var expected = $"CacheStats{{{value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.AddValue(value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_AddValue_int()
        {
            var value = (int) 331.4;
            var expected = $"CacheStats{{{value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.AddValue(value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_AddValue_long()
        {
            var value = (long) 331.4;
            var expected = $"CacheStats{{{value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.AddValue(value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_AddValue_object()
        {
            var value = "uncle";
            var expected = $"CacheStats{{{value}}}";
            var sut = ToStringHelper.GetInstance(testObject);
            sut.AddValue(value);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_Constructor()
        {
            var sut = ToStringHelper.GetInstance(testObject);
            Assert.IsNotNull(sut);
        }

        [Test]
        public void Test_No_Add()
        {
            var expected = "CacheStats{}";
            var sut = ToStringHelper.GetInstance(testObject);
            Assert.IsNotNull(sut);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual, "CacheStats.ToString() should return '{0}'", expected);
        }

        [Test]
        public void Test_OmitNullValues()
        {
            var name1 = "alice";
            var name2 = "bob";
            var value1 = (long) 227.9;
            object value2 = null;
            var expected = $"CacheStats{{{name1}={value1}}}";
            var sut = ToStringHelper.GetInstance(testObject)
                .OmitNullValues()
                .Add(name1, value1)
                .Add(name2, value2);

            var actual = sut.ToString();

            Assert.AreEqual(expected, actual);
        }
    }
}