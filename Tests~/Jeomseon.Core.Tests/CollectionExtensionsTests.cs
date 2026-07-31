using System.Collections.Generic;
using Jeomseon.Collections;
using NUnit.Framework;

namespace Jeomseon.Tests
{
    public sealed class CollectionExtensionsTests
    {
        [Test]
        public void RemoveFirst_RemovesOnlyFirstMatch()
        {
            List<int> values = new() { 1, 2, 2, 3 };

            bool removed = values.RemoveFirst(value => value == 2);

            Assert.That(removed, Is.True);
            Assert.That(values, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void QueueDrain_RemovesElementsInFifoOrder()
        {
            Queue<int> queue = new(new[] { 1, 2, 3 });

            Assert.That(queue.Drain(), Is.EqualTo(new[] { 1, 2, 3 }));
            Assert.That(queue, Is.Empty);
        }

        [Test]
        public void StackDrain_RemovesElementsInLifoOrder()
        {
            Stack<int> stack = new(new[] { 1, 2, 3 });

            Assert.That(stack.Drain(), Is.EqualTo(new[] { 3, 2, 1 }));
            Assert.That(stack, Is.Empty);
        }
    }
}
