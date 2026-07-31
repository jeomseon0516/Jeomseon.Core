using System;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    public enum PriorityQueueOrder
    {
        MinimumFirst,
        MaximumFirst
    }

    /// <summary>
    /// 요소와 우선순위를 분리해 관리하는 이진 힙 기반 우선순위 큐입니다.
    /// 동일 우선순위 요소의 FIFO 순서는 보장하지 않습니다.
    /// </summary>
    public sealed class PriorityQueue<TElement, TPriority>
    {
        private readonly List<Entry> _heap;
        private readonly IComparer<TPriority> _priorityComparer;

        public PriorityQueue(
            PriorityQueueOrder order = PriorityQueueOrder.MinimumFirst,
            IComparer<TPriority> priorityComparer = null,
            int initialCapacity = 0)
        {
            if (initialCapacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity));
            }

            if (order != PriorityQueueOrder.MinimumFirst &&
                order != PriorityQueueOrder.MaximumFirst)
            {
                throw new ArgumentOutOfRangeException(nameof(order));
            }

            Order = order;
            _priorityComparer = priorityComparer ?? Comparer<TPriority>.Default;
            _heap = new List<Entry>(initialCapacity);
        }

        public int Count => _heap.Count;
        public PriorityQueueOrder Order { get; }
        public IComparer<TPriority> PriorityComparer => _priorityComparer;

        public void Enqueue(TElement element, TPriority priority)
        {
            _heap.Add(new Entry(element, priority));
            MoveUp(_heap.Count - 1);
        }

        public TElement Dequeue()
        {
            if (!TryDequeue(out TElement element, out _))
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            return element;
        }

        public bool TryDequeue(
            out TElement element,
            out TPriority priority)
        {
            if (_heap.Count == 0)
            {
                element = default;
                priority = default;
                return false;
            }

            Entry root = _heap[0];
            int lastIndex = _heap.Count - 1;
            if (lastIndex == 0)
            {
                _heap.RemoveAt(0);
            }
            else
            {
                _heap[0] = _heap[lastIndex];
                _heap.RemoveAt(lastIndex);
                MoveDown(0);
            }

            element = root.Element;
            priority = root.Priority;
            return true;
        }

        public TElement Peek()
        {
            if (!TryPeek(out TElement element, out _))
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            return element;
        }

        public bool TryPeek(
            out TElement element,
            out TPriority priority)
        {
            if (_heap.Count == 0)
            {
                element = default;
                priority = default;
                return false;
            }

            Entry root = _heap[0];
            element = root.Element;
            priority = root.Priority;
            return true;
        }

        public void Clear()
        {
            _heap.Clear();
        }

        public IEnumerable<(TElement Element, TPriority Priority)> UnorderedItems
        {
            get
            {
                foreach (Entry entry in _heap)
                {
                    yield return (entry.Element, entry.Priority);
                }
            }
        }

        private void MoveUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (!HasHigherPriority(_heap[index], _heap[parentIndex]))
                {
                    break;
                }

                Swap(index, parentIndex);
                index = parentIndex;
            }
        }

        private void MoveDown(int index)
        {
            while (true)
            {
                int leftIndex = index * 2 + 1;
                if (leftIndex >= _heap.Count)
                {
                    return;
                }

                int rightIndex = leftIndex + 1;
                int candidateIndex =
                    rightIndex < _heap.Count &&
                    HasHigherPriority(_heap[rightIndex], _heap[leftIndex])
                        ? rightIndex
                        : leftIndex;

                if (!HasHigherPriority(_heap[candidateIndex], _heap[index]))
                {
                    return;
                }

                Swap(index, candidateIndex);
                index = candidateIndex;
            }
        }

        private bool HasHigherPriority(Entry left, Entry right)
        {
            int comparison = _priorityComparer.Compare(
                left.Priority,
                right.Priority);
            return Order == PriorityQueueOrder.MinimumFirst
                ? comparison < 0
                : comparison > 0;
        }

        private void Swap(int leftIndex, int rightIndex)
        {
            (_heap[leftIndex], _heap[rightIndex]) =
                (_heap[rightIndex], _heap[leftIndex]);
        }

        private readonly struct Entry
        {
            public Entry(TElement element, TPriority priority)
            {
                Element = element;
                Priority = priority;
            }

            public TElement Element { get; }
            public TPriority Priority { get; }
        }
    }
}
