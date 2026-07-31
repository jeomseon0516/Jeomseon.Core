using System.Collections.Generic;

namespace Jeomseon.Collections
{
    /// <summary><see cref="Queue{T}"/>의 요소를 소비하며 열거하는 확장을 제공합니다.</summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// 큐의 요소를 선입선출 순서로 제거하면서 열거합니다.
        /// 열거를 중단하면 아직 방문하지 않은 요소는 큐에 남습니다.
        /// </summary>
        /// <typeparam name="T">큐 요소 타입입니다.</typeparam>
        /// <param name="queue">소비하며 열거할 큐입니다.</param>
        /// <returns>큐에서 제거되는 요소의 지연 시퀀스입니다.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="queue"/>가 null입니다.</exception>
        public static IEnumerable<T> Drain<T>(this Queue<T> queue)
        {
            if (queue == null)
                throw new System.ArgumentNullException(nameof(queue));

            while (queue.Count > 0)
            {
                yield return queue.Dequeue();
            }
        }
    }
}
