using System;

namespace Jeomseon.Reflection
{
    public enum TypeActivationFailure
    {
        None,
        InvalidType,
        MissingDefaultConstructor,
        IncompatibleInstance,
        FactoryReturnedNull,
        ActivationException
    }

    /// <summary>
    /// 타입 인스턴스 생성 결과와 실패 원인을 나타냅니다.
    /// </summary>
    public readonly struct TypeActivationResult<T>
        where T : class
    {
        private TypeActivationResult(
            Type type,
            T instance,
            TypeActivationFailure failure,
            Exception exception)
        {
            Type = type;
            Instance = instance;
            Failure = failure;
            Exception = exception;
        }

        public Type Type { get; }
        public T Instance { get; }
        public TypeActivationFailure Failure { get; }
        public Exception Exception { get; }
        public bool IsSuccess =>
            Failure == TypeActivationFailure.None &&
            Instance != null;

        internal static TypeActivationResult<T> Success(Type type, T instance)
        {
            return new TypeActivationResult<T>(
                type,
                instance,
                TypeActivationFailure.None,
                null);
        }

        internal static TypeActivationResult<T> Failed(
            Type type,
            TypeActivationFailure failure,
            Exception exception = null)
        {
            return new TypeActivationResult<T>(type, null, failure, exception);
        }
    }
}
