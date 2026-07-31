using System;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    /// <summary><see cref="IEnumerable{T}"/>에 대한 열거 및 실행 확장을 제공합니다.</summary>
    public static class EnumerableExtensions
    {
        /// <summary>원본이 비어 있으면 대체 시퀀스를 열거합니다.</summary>
        /// <typeparam name="T">요소 타입입니다.</typeparam>
        /// <param name="source">먼저 확인할 시퀀스입니다.</param>
        /// <param name="fallback">원본이 비었을 때 사용할 시퀀스입니다.</param>
        /// <returns>원본 또는 대체 시퀀스를 지연 열거하는 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 또는 <paramref name="fallback"/>이 null입니다.</exception>
        public static IEnumerable<T> FallbackIfEmpty<T>(
            this IEnumerable<T> source,
            IEnumerable<T> fallback)
        {
            return source != null 
                ? fallback != null 
                    ? EnumerateSourceOrFallback(source, fallback)
                    : throw new ArgumentNullException(nameof(fallback))
                : throw new ArgumentNullException(nameof(source));
        }

        /// <summary>시퀀스의 각 요소에 작업을 순서대로 실행합니다.</summary>
        /// <typeparam name="T">요소 타입입니다.</typeparam>
        /// <param name="source">열거할 시퀀스입니다.</param>
        /// <param name="action">각 요소에 실행할 작업입니다.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 또는 <paramref name="action"/>이 null입니다.</exception>
        public static void ForEach<T>(
            this IEnumerable<T> source,
            Action<T> action)
        {
            ValidateArguments(source, action);

            foreach (T element in source)
            {
                action(element);
            }
        }

        /// <summary>null이 아닌 각 요소에 작업을 순서대로 실행합니다.</summary>
        /// <typeparam name="T">참조 타입 요소입니다.</typeparam>
        /// <param name="source">열거할 시퀀스입니다.</param>
        /// <param name="action">null이 아닌 요소에 실행할 작업입니다.</param>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> 또는 <paramref name="action"/>이 null입니다.</exception>
        public static void ForEachNotNull<T>(
            this IEnumerable<T> source,
            Action<T> action)
            where T : class
        {
            ValidateArguments(source, action);

            foreach (T element in source)
            {
                if (element != null)
                {
                    action(element);
                }
            }
        }

        private static IEnumerable<T> EnumerateSourceOrFallback<T>(
            IEnumerable<T> source,
            IEnumerable<T> fallback)
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                foreach (T fallbackElement in fallback)
                {
                    yield return fallbackElement;
                }

                yield break;
            }

            do
            {
                yield return enumerator.Current;
            }
            while (enumerator.MoveNext());
        }

        private static void ValidateArguments<T>(
            IEnumerable<T> source,
            Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
        }
    }
}
