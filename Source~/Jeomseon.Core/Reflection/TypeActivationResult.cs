using System;

namespace Jeomseon.Reflection
{
    /// <summary>타입 인스턴스 생성이 실패한 이유를 나타냅니다.</summary>
    public enum TypeActivationFailure
    {
        /// <summary>실패하지 않았습니다.</summary>
        None,

        /// <summary>입력 타입이 null이거나 유효하지 않습니다.</summary>
        InvalidType,

        /// <summary>공개 매개변수 없는 생성자를 찾을 수 없습니다.</summary>
        MissingDefaultConstructor,

        /// <summary>생성된 인스턴스가 요청한 타입과 호환되지 않습니다.</summary>
        IncompatibleInstance,

        /// <summary>사용자 팩터리가 null을 반환했습니다.</summary>
        FactoryReturnedNull,

        /// <summary>생성자 또는 사용자 팩터리가 예외를 발생시켰습니다.</summary>
        ActivationException
    }

    /// <summary>
    /// 타입 인스턴스 생성 결과와 실패 원인을 나타냅니다.
    /// </summary>
    /// <typeparam name="T">생성할 인스턴스의 기준 참조 타입입니다.</typeparam>
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

        /// <summary>생성을 시도한 타입을 가져옵니다.</summary>
        public Type Type { get; }

        /// <summary>성공한 경우 생성된 인스턴스를 가져옵니다.</summary>
        public T Instance { get; }

        /// <summary>실패 원인을 가져옵니다.</summary>
        public TypeActivationFailure Failure { get; }

        /// <summary>생성 과정에서 발생한 예외를 가져옵니다.</summary>
        public Exception Exception { get; }

        /// <summary>호환되는 인스턴스가 생성되었는지 나타냅니다.</summary>
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
