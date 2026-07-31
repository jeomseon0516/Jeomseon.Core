using System;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    public static class EnumerableExtensions
    {
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
