using System;
using System.Collections.Generic;
using System.Linq;
using Jeomseon.Collections;
using NUnit.Framework;

namespace Jeomseon.Tests
{
    public sealed class PriorityQueueTests
    {
        [Test]
        public void DefaultOrder_DequeuesLowestPriorityFirst()
        {
            Jeomseon.Collections.PriorityQueue<string, int> queue = new();
            queue.Enqueue("middle", 3);
            queue.Enqueue("first", 1);
            queue.Enqueue("last", 5);
            queue.Enqueue("second", 2);

            Assert.That(queue.Peek(), Is.EqualTo("first"));
            Assert.That(queue.Dequeue(), Is.EqualTo("first"));
            Assert.That(queue.Dequeue(), Is.EqualTo("second"));
            Assert.That(queue.Dequeue(), Is.EqualTo("middle"));
            Assert.That(queue.Dequeue(), Is.EqualTo("last"));
        }

        [Test]
        public void MaximumFirst_DequeuesHighestPriorityFirst()
        {
            Jeomseon.Collections.PriorityQueue<string, int> queue =
                new(PriorityQueueOrder.MaximumFirst);
            queue.Enqueue("middle", 3);
            queue.Enqueue("low", 1);
            queue.Enqueue("high", 5);

            Assert.That(queue.Dequeue(), Is.EqualTo("high"));
            Assert.That(queue.Dequeue(), Is.EqualTo("middle"));
            Assert.That(queue.Dequeue(), Is.EqualTo("low"));
        }

        [Test]
        public void CustomComparer_DefinesPriorityOrdering()
        {
            IComparer<string> comparer = Comparer<string>.Create(
                (left, right) => left.Length.CompareTo(right.Length));
            Jeomseon.Collections.PriorityQueue<int, string> queue = new(
                PriorityQueueOrder.MinimumFirst,
                comparer);
            queue.Enqueue(3, "three");
            queue.Enqueue(1, "a");
            queue.Enqueue(2, "two");

            Assert.That(queue.Dequeue(), Is.EqualTo(1));
            Assert.That(queue.Dequeue(), Is.EqualTo(2));
            Assert.That(queue.Dequeue(), Is.EqualTo(3));
        }

        [Test]
        public void TryMethods_ReturnElementAndPriority()
        {
            Jeomseon.Collections.PriorityQueue<string, int> queue =
                new(PriorityQueueOrder.MaximumFirst);
            queue.Enqueue("low", 1);
            queue.Enqueue("high", 10);

            Assert.That(queue.TryPeek(out string peeked, out int peekedPriority), Is.True);
            Assert.That(peeked, Is.EqualTo("high"));
            Assert.That(peekedPriority, Is.EqualTo(10));
            Assert.That(queue.TryDequeue(out string removed, out int removedPriority), Is.True);
            Assert.That(removed, Is.EqualTo("high"));
            Assert.That(removedPriority, Is.EqualTo(10));
        }

        [Test]
        public void EmptyQueue_TryMethodsReturnFalse_AndThrowingMethodsThrow()
        {
            Jeomseon.Collections.PriorityQueue<string, int> queue = new();

            Assert.That(queue.TryPeek(out string peeked, out int peekedPriority), Is.False);
            Assert.That(peeked, Is.Null);
            Assert.That(peekedPriority, Is.Zero);
            Assert.That(queue.TryDequeue(out string removed, out int removedPriority), Is.False);
            Assert.That(removed, Is.Null);
            Assert.That(removedPriority, Is.Zero);
            Assert.Throws<InvalidOperationException>(() => queue.Peek());
            Assert.Throws<InvalidOperationException>(() => queue.Dequeue());
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            Jeomseon.Collections.PriorityQueue<int, int> queue = new();
            queue.Enqueue(1, 1);
            queue.Enqueue(2, 2);

            queue.Clear();

            Assert.That(queue.Count, Is.Zero);
        }

        [Test]
        public void Constructor_RejectsNegativeCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Jeomseon.Collections.PriorityQueue<int, int>(
                    initialCapacity: -1));
        }

        [Test]
        public void Constructor_RejectsUnknownOrder()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Jeomseon.Collections.PriorityQueue<int, int>(
                    (PriorityQueueOrder)999));
        }

        [Test]
        public void UnorderedItems_ExposeElementsAndPriorities()
        {
            Jeomseon.Collections.PriorityQueue<string, int> queue = new();
            queue.Enqueue("one", 1);
            queue.Enqueue("two", 2);

            (string Element, int Priority)[] items =
                queue.UnorderedItems.ToArray();

            Assert.That(items, Has.Length.EqualTo(2));
            Assert.That(items, Does.Contain(("one", 1)));
            Assert.That(items, Does.Contain(("two", 2)));
        }
    }
}
