using System.Collections.Generic;

namespace Jeomseon.Collections
{
    /// <summary><see cref="Stack{T}"/>의 요소를 소비하며 열거하는 확장을 제공합니다.</summary>
    public static class StackExtensions
    {
        /// <summary>
        /// 스택의 요소를 후입선출 순서로 제거하면서 열거합니다.
        /// 열거를 중단하면 아직 방문하지 않은 요소는 스택에 남습니다.
        /// </summary>
        /// <typeparam name="T">스택 요소 타입입니다.</typeparam>
        /// <param name="stack">소비하며 열거할 스택입니다.</param>
        /// <returns>스택에서 제거되는 요소의 지연 시퀀스입니다.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="stack"/>이 null입니다.</exception>
        public static IEnumerable<T> Drain<T>(this Stack<T> stack)
        {
            if (stack == null)
                throw new System.ArgumentNullException(nameof(stack));

            while (stack.Count > 0)
            {
                yield return stack.Pop();
            }
        }
    }
}
