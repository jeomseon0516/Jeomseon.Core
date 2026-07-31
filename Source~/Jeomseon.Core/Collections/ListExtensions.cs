using System;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    /// <summary>목록에서 요소를 찾아 제거하는 확장을 제공합니다.</summary>
    public static class ListExtensions
    {
        /// <summary>조건을 만족하는 첫 번째 요소를 제거합니다.</summary>
        /// <typeparam name="T">요소 타입입니다.</typeparam>
        /// <param name="source">검색할 목록입니다.</param>
        /// <param name="predicate">제거할 요소를 선택하는 조건입니다.</param>
        /// <returns>요소를 제거했으면 <see langword="true"/>입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 또는 <paramref name="predicate"/>가 null입니다.</exception>
        public static bool RemoveFirst<T>(this List<T> source, Predicate<T> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            int index = source.FindIndex(predicate);
            if (index < 0)
                return false;

            source.RemoveAt(index);
            return true;
        }

        /// <summary>지정한 인덱스의 요소를 제거하고 가져옵니다.</summary>
        /// <typeparam name="T">요소 타입입니다.</typeparam>
        /// <param name="source">대상 목록입니다.</param>
        /// <param name="index">제거할 요소의 인덱스입니다.</param>
        /// <param name="result">성공하면 제거한 요소이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 제거했으면 <see langword="true"/>입니다.</returns>
        public static bool TryPop<T>(this IList<T> source, int index, out T result)
        {
            if (source is null || index < 0 || index >= source.Count)
            {
                result = default;
                return false;
            }

            result = source[index];
            source.RemoveAt(index);

            return true;
        }

        /// <summary>첫 번째 요소를 제거하고 가져옵니다.</summary>
        /// <typeparam name="T">요소 타입입니다.</typeparam>
        /// <param name="source">대상 목록입니다.</param>
        /// <param name="result">성공하면 제거한 요소이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 제거했으면 <see langword="true"/>입니다.</returns>
        public static bool TryPopFirst<T>(this IList<T> source, out T result)
        {
            return source.TryPop(0, out result);
        }

        /// <summary>마지막 요소를 제거하고 가져옵니다.</summary>
        /// <typeparam name="T">요소 타입입니다.</typeparam>
        /// <param name="source">대상 목록입니다.</param>
        /// <param name="result">성공하면 제거한 요소이고, 실패하면 기본값입니다.</param>
        /// <returns>요소를 제거했으면 <see langword="true"/>입니다.</returns>
        public static bool TryPopLast<T>(this IList<T> source, out T result)
        {
            if (source is null)
            {
                result = default;
                return false;
            }

            return source.TryPop(source.Count - 1, out result);
        }
    }
}
