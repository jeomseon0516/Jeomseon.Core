using System;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    /// <summary>우선순위 큐가 먼저 반환할 우선순위의 정렬 방향을 지정합니다.</summary>
    public enum PriorityQueueOrder
    {
        /// <summary>비교 결과가 작은 우선순위를 먼저 반환합니다.</summary>
        MinimumFirst,

        /// <summary>비교 결과가 큰 우선순위를 먼저 반환합니다.</summary>
        MaximumFirst
    }

    /// <summary>
    /// 요소와 우선순위를 분리해 관리하는 이진 힙 기반 우선순위 큐입니다.
    /// 동일 우선순위 요소의 FIFO 순서는 보장하지 않습니다.
    /// </summary>
    /// <typeparam name="TElement">큐에 저장할 요소 타입입니다.</typeparam>
    /// <typeparam name="TPriority">요소의 우선순위 타입입니다.</typeparam>
    public sealed class PriorityQueue<TElement, TPriority>
    {
        private readonly List<Entry> _heap;
        private readonly IComparer<TPriority> _priorityComparer;

        /// <summary>정렬 방향, 비교자 및 초기 용량을 지정해 빈 우선순위 큐를 만듭니다.</summary>
        /// <param name="order">먼저 반환할 우선순위 방향입니다.</param>
        /// <param name="priorityComparer">우선순위를 비교할 비교자이며, null이면 기본 비교자를 사용합니다.</param>
        /// <param name="initialCapacity">미리 확보할 요소 수입니다.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="order"/>가 유효하지 않거나 <paramref name="initialCapacity"/>가 음수입니다.</exception>
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

        /// <summary>현재 큐에 저장된 요소 수를 가져옵니다.</summary>
        public int Count => _heap.Count;

        /// <summary>우선순위 정렬 방향을 가져옵니다.</summary>
        public PriorityQueueOrder Order { get; }

        /// <summary>우선순위 비교에 사용하는 비교자를 가져옵니다.</summary>
        public IComparer<TPriority> PriorityComparer => _priorityComparer;

        /// <summary>요소와 우선순위를 큐에 추가합니다.</summary>
        /// <param name="element">추가할 요소입니다.</param>
        /// <param name="priority">요소의 우선순위입니다.</param>
        public void Enqueue(TElement element, TPriority priority)
        {
            _heap.Add(new Entry(element, priority));
            MoveUp(_heap.Count - 1);
        }

        /// <summary>가장 높은 순위의 요소를 제거하고 반환합니다.</summary>
        /// <returns>제거한 요소입니다.</returns>
        /// <exception cref="InvalidOperationException">큐가 비어 있습니다.</exception>
        public TElement Dequeue()
        {
            if (!TryDequeue(out TElement element, out _))
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            return element;
        }

        /// <summary>가장 높은 순위의 요소와 우선순위를 제거하고 가져옵니다.</summary>
        /// <param name="element">성공하면 제거한 요소이고, 실패하면 기본값입니다.</param>
        /// <param name="priority">성공하면 제거한 요소의 우선순위이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 제거했으면 <see langword="true"/>입니다.</returns>
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

        /// <summary>가장 높은 순위의 요소를 제거하지 않고 반환합니다.</summary>
        /// <returns>가장 높은 순위의 요소입니다.</returns>
        /// <exception cref="InvalidOperationException">큐가 비어 있습니다.</exception>
        public TElement Peek()
        {
            if (!TryPeek(out TElement element, out _))
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            return element;
        }

        /// <summary>가장 높은 순위의 요소와 우선순위를 제거하지 않고 가져옵니다.</summary>
        /// <param name="element">성공하면 요소이고, 실패하면 기본값입니다.</param>
        /// <param name="priority">성공하면 우선순위이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 가져왔으면 <see langword="true"/>입니다.</returns>
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

        /// <summary>큐의 모든 요소를 제거합니다.</summary>
        public void Clear()
        {
            _heap.Clear();
        }

        /// <summary>힙 내부 순서로 요소와 우선순위를 열거하는 시퀀스를 가져옵니다.</summary>
        /// <remarks>반환되는 순서는 우선순위 정렬 순서가 아닙니다.</remarks>
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
