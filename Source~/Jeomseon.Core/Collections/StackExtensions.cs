using System.Collections.Generic;

namespace Jeomseon.Collections
{
    public static class StackExtensions
    {
        /// <summary>
        /// 스택의 요소를 후입선출 순서로 제거하면서 열거합니다.
        /// 열거를 중단하면 아직 방문하지 않은 요소는 스택에 남습니다.
        /// </summary>
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
