using System;
using System.Collections.Generic;

namespace Jeomseon.Collections
{
    public static class ListExtensions
    {
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

        public static bool TryPopFirst<T>(this IList<T> source, out T result)
        {
            return source.TryPop(0, out result);
        }

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
