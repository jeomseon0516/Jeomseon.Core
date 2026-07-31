using System;
using System.Collections;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    /// <summary>
    /// 원형 배열을 사용해 양 끝 삽입과 제거를 제공하는 컬렉션입니다.
    /// </summary>
    /// <typeparam name="T">저장할 요소의 타입입니다.</typeparam>
    public sealed class Deque<T> : IReadOnlyCollection<T>, ICollection
    {
        private const int DefaultCapacity = 4;

        private T[] _buffer;
        private int _head;
        private int _version;
        private object _syncRoot;

        /// <summary>
        /// 지정한 초기 용량으로 빈 덱을 만듭니다.
        /// </summary>
        /// <param name="capacity">미리 확보할 요소 수입니다.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="capacity"/>가 0보다 작습니다.
        /// </exception>
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

        /// <summary>현재 덱에 저장된 요소 수를 가져옵니다.</summary>
        public int Count { get; private set; }

        /// <summary>크기를 다시 늘리지 않고 저장할 수 있는 총 요소 수를 가져옵니다.</summary>
        public int Capacity => _buffer.Length;
        bool ICollection.IsSynchronized => false;
        object ICollection.SyncRoot => _syncRoot ??= new object();

        /// <summary>덱의 앞쪽에 요소를 추가합니다.</summary>
        /// <param name="item">추가할 요소입니다.</param>
        public void AddFirst(T item)
        {
            EnsureCapacity(Count + 1);
            _head = WrapIndex(_head - 1);
            _buffer[_head] = item;
            Count++;
            _version++;
        }

        /// <summary>덱의 뒤쪽에 요소를 추가합니다.</summary>
        /// <param name="item">추가할 요소입니다.</param>
        public void AddLast(T item)
        {
            EnsureCapacity(Count + 1);
            _buffer[GetBufferIndex(Count)] = item;
            Count++;
            _version++;
        }

        /// <summary>앞쪽 요소를 제거하지 않고 반환합니다.</summary>
        /// <returns>덱의 앞쪽 요소입니다.</returns>
        /// <exception cref="InvalidOperationException">덱이 비어 있습니다.</exception>
        public T PeekFirst()
        {
            if (!TryPeekFirst(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        /// <summary>뒤쪽 요소를 제거하지 않고 반환합니다.</summary>
        /// <returns>덱의 뒤쪽 요소입니다.</returns>
        /// <exception cref="InvalidOperationException">덱이 비어 있습니다.</exception>
        public T PeekLast()
        {
            if (!TryPeekLast(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        /// <summary>앞쪽 요소를 제거하지 않고 가져옵니다.</summary>
        /// <param name="result">성공하면 앞쪽 요소이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 가져왔으면 <see langword="true"/>입니다.</returns>
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

        /// <summary>뒤쪽 요소를 제거하지 않고 가져옵니다.</summary>
        /// <param name="result">성공하면 뒤쪽 요소이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 가져왔으면 <see langword="true"/>입니다.</returns>
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

        /// <summary>앞쪽 요소를 제거하고 반환합니다.</summary>
        /// <returns>제거한 앞쪽 요소입니다.</returns>
        /// <exception cref="InvalidOperationException">덱이 비어 있습니다.</exception>
        public T DequeueFirst()
        {
            if (!TryDequeueFirst(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        /// <summary>뒤쪽 요소를 제거하고 반환합니다.</summary>
        /// <returns>제거한 뒤쪽 요소입니다.</returns>
        /// <exception cref="InvalidOperationException">덱이 비어 있습니다.</exception>
        public T DequeueLast()
        {
            if (!TryDequeueLast(out T result))
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            return result;
        }

        /// <summary>앞쪽 요소를 제거하고 가져옵니다.</summary>
        /// <param name="result">성공하면 제거한 요소이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 제거했으면 <see langword="true"/>입니다.</returns>
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

        /// <summary>뒤쪽 요소를 제거하고 가져옵니다.</summary>
        /// <param name="result">성공하면 제거한 요소이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 제거했으면 <see langword="true"/>입니다.</returns>
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

        /// <summary>동등성 비교자를 사용해 지정한 요소가 포함되어 있는지 확인합니다.</summary>
        /// <param name="item">찾을 요소입니다.</param>
        /// <returns>일치하는 요소가 있으면 <see langword="true"/>입니다.</returns>
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

        /// <summary>앞쪽부터 뒤쪽 순서로 요소를 배열에 복사합니다.</summary>
        /// <param name="array">요소를 받을 배열입니다.</param>
        /// <param name="arrayIndex">복사를 시작할 인덱스입니다.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/>가 null입니다.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/>가 배열 범위를 벗어났습니다.</exception>
        /// <exception cref="ArgumentException">대상 배열의 남은 공간이 부족합니다.</exception>
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

        /// <summary>모든 요소를 제거하고 확보된 용량은 유지합니다.</summary>
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

        /// <summary>앞쪽부터 뒤쪽 순서로 요소를 열거합니다.</summary>
        /// <returns>덱의 열거자입니다.</returns>
        /// <exception cref="InvalidOperationException">열거 중 덱이 변경되었습니다.</exception>
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
