using System;
using NUnit.Framework;
using Stash.@internal;

namespace Stash.Test.@internal
{
    [TestFixture]
    public class Test_Preconditions
    {
        private const double one = 1.0;
        private const string messageTemplate = "one={0}";

        [Test]
        public void Test_CheckArgument_DoesNotThrowOnTrue()
        {
            
            Assert.DoesNotThrow(() => Preconditions.CheckArgument(true));
        }

        [Test]
        public void Test_CheckArgument_ThrowsOnFalse()
        {
            Assert.Throws<ArgumentException>(() => Preconditions.CheckArgument(false));
        }

        [Test]
        public void Test_CheckArgument_ThrowsWithCustomMessage()
        {
            var expected = "suck it murica";

            //Preconditions.CheckArgument(one >= one + 1, expected);

            Assert.That(() => Preconditions.CheckArgument(false, expected),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(expected));
        }

        [Test]
        public void Test_CheckArgument_ThrowsWithCustomMessageWithArgs()
        {
            
            var expected = string.Format(messageTemplate, one);

            Assert.That(() => Preconditions.CheckArgument(false, messageTemplate, one),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(expected));
        }

        [Test]
        public void Test_CheckArgument_ThrowsWithoutCustomMessage()
        {
            var one = 1.0;
            var expected = "Invalid argument: expression is false.";

            Assert.That(() => Preconditions.CheckArgument(one >= one + 1),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(expected));
        }

        [Test]
        public void Test_CheckNotNull_ReturnsValue()
        {
            var expected = new TestAttribute();
            TestAttribute actual = null;
            Assert.DoesNotThrow(() => actual = Preconditions.CheckNotNull(expected));
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Test_CheckNotNull_ThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => Preconditions.CheckNotNull((TestAttribute) null));
        }

        [Test]
        public void Test_CheckNotNull_ThrowsWithArgumentName()
        {
            // ReSharper disable once RedundantAssignment
            var value = new TestAttribute();
            value = null;
            const string name = "robble robble";
            var expected = $"Value cannot be null.\r\nParameter name: {name}";
            Assert.That(() => Preconditions.CheckNotNull(value, name),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo(expected));
        }

        [Test]
        public void Test_CheckNotNull_ThrowsWithoutArgumentName()
        {
            // ReSharper disable once RedundantAssignment
            var value = new TestAttribute();
            value = null;
            const string expected = "Value cannot be null.";
            Assert.That(() => Preconditions.CheckNotNull(value),
                Throws.TypeOf<ArgumentNullException>()
                    .With.Message.EqualTo(expected));
        }

        [Test]
        public void Test_CheckState_DoesNotThrowOnTrue()
        {
            Assert.DoesNotThrow(() => Preconditions.CheckState(true));
        }

        [Test]
        public void Test_CheckState_ThrowsOnFalse()
        {
            Assert.Throws<ArgumentException>(() => Preconditions.CheckState(false));
        }

        [Test]
        public void Test_CheckState_ThrowsWithCustomMessage()
        {
            const string expected = "blarg blarg blarg";

            Assert.That(() => Preconditions.CheckState(false, expected),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(expected));
        }

        [Test]
        public void Test_CheckState_ThrowsWithCustomMessageWithArgs()
        {
            var expected = string.Format(messageTemplate, one);

            Assert.That(() => Preconditions.CheckState(false, messageTemplate, one),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(expected));
        }

        [Test]
        public void Test_CheckState_ThrowsWithoutCustomMessage()
        {
            const string expected = "Invalid state: expression is false.";

            Assert.That(() => Preconditions.CheckState(false),
                Throws.TypeOf<ArgumentException>()
                    .With.Message.EqualTo(expected));
        }
    }
}