using System.Collections.Generic;

namespace Jeomseon.Collections
{
    public static class QueueExtensions
    {
        /// <summary>
        /// 큐의 요소를 선입선출 순서로 제거하면서 열거합니다.
        /// 열거를 중단하면 아직 방문하지 않은 요소는 큐에 남습니다.
        /// </summary>
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
