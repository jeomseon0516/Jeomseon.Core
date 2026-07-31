using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jeomseon.Collections;
using NUnit.Framework;

namespace Jeomseon.Tests
{
    public sealed class DequeTests
    {
        [Test]
        public void AddAndDequeue_PreserveBothEndOrders()
        {
            Deque<int> deque = new();

            deque.AddLast(2);
            deque.AddFirst(1);
            deque.AddLast(3);

            Assert.That(deque.Count, Is.EqualTo(3));
            Assert.That(deque.PeekFirst(), Is.EqualTo(1));
            Assert.That(deque.PeekLast(), Is.EqualTo(3));
            Assert.That(deque.ToArray(), Is.EqualTo(new[] { 1, 2, 3 }));
            Assert.That(deque.DequeueFirst(), Is.EqualTo(1));
            Assert.That(deque.DequeueLast(), Is.EqualTo(3));
            Assert.That(deque.DequeueFirst(), Is.EqualTo(2));
            Assert.That(deque.Count, Is.Zero);
        }

        [Test]
        public void EmptyDeque_TryMethodsReturnFalse_AndThrowingMethodsThrow()
        {
            Deque<string> deque = new();

            Assert.That(deque.TryPeekFirst(out string first), Is.False);
            Assert.That(first, Is.Null);
            Assert.That(deque.TryPeekLast(out string last), Is.False);
            Assert.That(last, Is.Null);
            Assert.That(deque.TryDequeueFirst(out _), Is.False);
            Assert.That(deque.TryDequeueLast(out _), Is.False);
            Assert.Throws<InvalidOperationException>(() => deque.PeekFirst());
            Assert.Throws<InvalidOperationException>(() => deque.PeekLast());
            Assert.Throws<InvalidOperationException>(() => deque.DequeueFirst());
            Assert.Throws<InvalidOperationException>(() => deque.DequeueLast());
        }

        [Test]
        public void Clear_RemovesAllItems()
        {
            Deque<int> deque = new();
            deque.AddLast(10);
            deque.AddLast(20);

            deque.Clear();

            Assert.That(deque.Count, Is.Zero);
            Assert.That(deque.Contains(10), Is.False);
        }

        [Test]
        public void CircularBuffer_WrapsAndGrowsWithoutChangingOrder()
        {
            Deque<int> deque = new(4);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.AddLast(4);

            Assert.That(deque.DequeueFirst(), Is.EqualTo(1));
            Assert.That(deque.DequeueFirst(), Is.EqualTo(2));

            deque.AddLast(5);
            deque.AddLast(6);
            deque.AddFirst(2);
            deque.AddFirst(1);
            deque.AddLast(7);

            Assert.That(deque.Capacity, Is.GreaterThanOrEqualTo(7));
            Assert.That(
                deque.ToArray(),
                Is.EqualTo(new[] { 1, 2, 3, 4, 5, 6, 7 }));
        }

        [Test]
        public void CopyTo_CopiesLogicalOrderAcrossWrappedStorage()
        {
            Deque<int> deque = new(3);
            deque.AddLast(1);
            deque.AddLast(2);
            deque.AddLast(3);
            deque.DequeueFirst();
            deque.AddLast(4);
            int[] destination = new int[5];

            deque.CopyTo(destination, 1);

            Assert.That(destination, Is.EqualTo(new[] { 0, 2, 3, 4, 0 }));
        }

        [Test]
        public void Enumerator_ThrowsWhenDequeIsModified()
        {
            Deque<int> deque = new();
            deque.AddLast(1);
            deque.AddLast(2);
            IEnumerator<int> enumerator = deque.GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True);
            deque.AddLast(3);

            Assert.Throws<InvalidOperationException>(
                () => enumerator.MoveNext());
        }

        [Test]
        public void Enumerator_ThrowsWhenRemainingItemsAreRemoved()
        {
            Deque<int> deque = new();
            deque.AddLast(1);
            IEnumerator<int> enumerator = deque.GetEnumerator();

            Assert.That(enumerator.MoveNext(), Is.True);
            deque.Clear();

            Assert.Throws<InvalidOperationException>(
                () => enumerator.MoveNext());
        }

        [Test]
        public void Constructor_RejectsNegativeCapacity()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new Deque<int>(-1));
        }

        [Test]
        public void NonGenericCollection_CopiesLogicalOrder()
        {
            Deque<int> deque = new(2);
            deque.AddLast(1);
            deque.AddLast(2);
            ICollection collection = deque;
            Array destination = new object[4];

            collection.CopyTo(destination, 1);

            Assert.That(
                destination,
                Is.EqualTo(new object[] { null, 1, 2, null }));
            Assert.That(collection.IsSynchronized, Is.False);
            Assert.That(collection.SyncRoot, Is.SameAs(collection.SyncRoot));
        }
    }
}
