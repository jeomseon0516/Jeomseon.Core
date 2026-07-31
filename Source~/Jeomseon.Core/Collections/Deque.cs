using System;
using System.Collections;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    /// <summary>
    /// 원형 배열을 사용해 양 끝 삽입과 제거를 제공하는 컬렉션입니다.
    /// </summary>
    public sealed class Deque<T> : IReadOnlyCollection<T>, ICollection
    {
        private const int DefaultCapacity = 4;

        private T[] _buffer;
        private int _head;
        private int _version;
        private object _syncRoot;

        public Deque(int capacity = 0)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _buffer = capacity == 0
                ? Array.Empty<T>()
                : new T[capacity];
        }

        public int Count { get; private set; }
        public int Capacity => _buffer.Length;
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => _syncRoot ??= new object();

        public void AddFirst(T item)
        {
            EnsureCapacity(Count + 1);
            _head = WrapIndex(_head - 1);
            _buffer[_head] = item;
            Count++;
            _version++;
        }

        public void AddLast(T item)
        {
            EnsureCapacity(Count + 1);
            _buffer[GetBufferIndex(Count)] = item;
            Count++;
            _version++;
        }

        public T PeekFirst()
        {
            if (!TryPeekFirst(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        public T PeekLast()
        {
            if (!TryPeekLast(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        public bool TryPeekFirst(out T result)
        {
            if (Count == 0)
            {
                result = default;
                return false;
            }

            result = _buffer[_head];
            return true;
        }

        public bool TryPeekLast(out T result)
        {
            if (Count == 0)
            {
                result = default;
                return false;
            }

            result = _buffer[GetBufferIndex(Count - 1)];
            return true;
        }

        public T DequeueFirst()
        {
            if (!TryDequeueFirst(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        public T DequeueLast()
        {
            if (!TryDequeueLast(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        public bool TryDequeueFirst(out T result)
        {
            if (!TryPeekFirst(out result))
            {
                return false;
            }

            _buffer[_head] = default;
            _head = Count == 1 ? 0 : WrapIndex(_head + 1);
            Count--;
            _version++;
            return true;
        }

        public bool TryDequeueLast(out T result)
        {
            if (!TryPeekLast(out result))
            {
                return false;
            }

            _buffer[GetBufferIndex(Count - 1)] = default;
            Count--;
            if (Count == 0)
            {
                _head = 0;
            }

            _version++;
            return true;
        }

        public bool Contains(T item)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int index = 0; index < Count; index++)
            {
                if (comparer.Equals(_buffer[GetBufferIndex(index)], item))
                {
                    return true;
                }
            }

            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (array.Length - arrayIndex < Count)
            {
                throw new ArgumentException(
                    "The destination array does not have enough space.",
                    nameof(array));
            }

            for (int index = 0; index < Count; index++)
            {
                array[arrayIndex + index] = _buffer[GetBufferIndex(index)];
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (array.Rank != 1 || array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException(
                    "The destination must be a zero-based, one-dimensional array.",
                    nameof(array));
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (array.Length - index < Count)
            {
                throw new ArgumentException(
                    "The destination array does not have enough space.",
                    nameof(array));
            }

            if (array is T[] typedArray)
            {
                CopyTo(typedArray, index);
                return;
            }

            try
            {
                for (int itemIndex = 0; itemIndex < Count; itemIndex++)
                {
                    array.SetValue(
                        _buffer[GetBufferIndex(itemIndex)],
                        index + itemIndex);
                }
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(
                    "The destination array type is incompatible.",
                    nameof(array));
            }
        }

        public void Clear()
        {
            if (Count == 0)
            {
                return;
            }

            Array.Clear(_buffer, 0, _buffer.Length);
            _head = 0;
            Count = 0;
            _version++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int version = _version;
            int count = Count;
            for (int index = 0; index < count; index++)
            {
                if (version != _version)
                {
                    throw new InvalidOperationException(
                        "Collection was modified during enumeration.");
                }

                yield return _buffer[GetBufferIndex(index)];
            }

            if (version != _version)
            {
                throw new InvalidOperationException(
                    "Collection was modified during enumeration.");
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void EnsureCapacity(int requiredCapacity)
        {
            if (_buffer.Length >= requiredCapacity)
            {
                return;
            }

            int newCapacity = _buffer.Length == 0
                ? DefaultCapacity
                : _buffer.Length * 2;
            if (newCapacity < requiredCapacity)
            {
                newCapacity = requiredCapacity;
            }

            T[] newBuffer = new T[newCapacity];
            for (int index = 0; index < Count; index++)
            {
                newBuffer[index] = _buffer[GetBufferIndex(index)];
            }

            _buffer = newBuffer;
            _head = 0;
        }

        private int GetBufferIndex(int logicalIndex)
        {
            return (_head + logicalIndex) % _buffer.Length;
        }

        private int WrapIndex(int index)
        {
            return index < 0 ? index + _buffer.Length : index % _buffer.Length;
        }
    }
}
