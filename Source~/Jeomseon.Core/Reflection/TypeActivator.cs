using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jeomseon.Reflection
{
    /// <summary>
    /// 발견된 CLR 타입을 인스턴스화합니다.
    /// </summary>
    public static class TypeActivator
    {
        /// <summary>현재 AppDomain에서 발견한 파생 구체 타입의 인스턴스를 생성합니다.</summary>
        /// <typeparam name="T">기준 참조 타입입니다.</typeparam>
        /// <returns>생성에 성공한 인스턴스의 지연 시퀀스입니다.</returns>
        public static IEnumerable<T> CreateDerivedInstances<T>()
            where T : class
        {
            return CreateInstances<T>(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>());
        }

        /// <summary>현재 AppDomain에서 발견한 파생 구체 타입을 팩터리로 생성합니다.</summary>
        /// <typeparam name="T">기준 참조 타입입니다.</typeparam>
        /// <param name="factory">발견된 타입을 인스턴스로 변환할 팩터리입니다.</param>
        /// <returns>생성에 성공한 인스턴스의 지연 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/>가 null입니다.</exception>
        public static IEnumerable<T> CreateDerivedInstances<T>(Func<Type, T> factory)
            where T : class
        {
            return CreateInstances(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>(),
                factory);
        }

        /// <summary>현재 AppDomain에서 발견한 파생 구체 타입의 생성 결과를 가져옵니다.</summary>
        /// <typeparam name="T">기준 참조 타입입니다.</typeparam>
        /// <returns>성공과 실패를 모두 포함한 생성 결과의 지연 시퀀스입니다.</returns>
        public static IEnumerable<TypeActivationResult<T>> CreateDerivedInstanceResults<T>()
            where T : class
        {
            return CreateInstanceResults<T>(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>());
        }

        /// <summary>현재 AppDomain에서 발견한 파생 구체 타입을 팩터리로 생성한 결과를 가져옵니다.</summary>
        /// <typeparam name="T">기준 참조 타입입니다.</typeparam>
        /// <param name="factory">발견된 타입을 인스턴스로 변환할 팩터리입니다.</param>
        /// <returns>성공과 실패를 모두 포함한 생성 결과의 지연 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="factory"/>가 null입니다.</exception>
        public static IEnumerable<TypeActivationResult<T>> CreateDerivedInstanceResults<T>(
            Func<Type, T> factory)
            where T : class
        {
            return CreateInstanceResults(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>(),
                factory);
        }

        /// <summary>지정한 타입들의 공개 매개변수 없는 생성자를 호출합니다.</summary>
        /// <typeparam name="T">생성할 인스턴스의 기준 참조 타입입니다.</typeparam>
        /// <param name="types">생성을 시도할 타입 시퀀스입니다.</param>
        /// <returns>생성에 성공한 인스턴스의 지연 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="types"/>가 null입니다.</exception>
        public static IEnumerable<T> CreateInstances<T>(IEnumerable<Type> types)
            where T : class
        {
            foreach (TypeActivationResult<T> result in CreateInstanceResults<T>(types))
            {
                if (result.IsSuccess)
                {
                    yield return result.Instance;
                }
            }
        }

        /// <summary>지정한 타입들의 공개 매개변수 없는 생성자를 호출한 결과를 가져옵니다.</summary>
        /// <typeparam name="T">생성할 인스턴스의 기준 참조 타입입니다.</typeparam>
        /// <param name="types">생성을 시도할 타입 시퀀스입니다.</param>
        /// <returns>성공과 실패를 모두 포함한 생성 결과의 지연 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="types"/>가 null입니다.</exception>
        public static IEnumerable<TypeActivationResult<T>> CreateInstanceResults<T>(
            IEnumerable<Type> types)
            where T : class
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (Type type in types)
            {
                if (type == null)
                {
                    yield return TypeActivationResult<T>.Failed(
                        null,
                        TypeActivationFailure.InvalidType);
                    continue;
                }

                ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                {
                    yield return TypeActivationResult<T>.Failed(
                        type,
                        TypeActivationFailure.MissingDefaultConstructor);
                    continue;
                }

                object value = null;
                TypeActivationResult<T> failure = default;
                bool failed = false;
                try
                {
                    value = constructor.Invoke(null);
                }
                catch (TargetInvocationException exception)
                {
                    failure = TypeActivationResult<T>.Failed(
                        type,
                        TypeActivationFailure.ActivationException,
                        exception.InnerException ?? exception);
                    failed = true;
                }
                catch (Exception exception)
                {
                    failure = TypeActivationResult<T>.Failed(
                        type,
                        TypeActivationFailure.ActivationException,
                        exception);
                    failed = true;
                }

                if (failed)
                {
                    yield return failure;
                    continue;
                }

                if (value is T instance)
                {
                    yield return TypeActivationResult<T>.Success(type, instance);
                    continue;
                }

                yield return TypeActivationResult<T>.Failed(
                    type,
                    TypeActivationFailure.IncompatibleInstance);
            }
        }

        /// <summary>지정한 타입들을 사용자 팩터리로 생성합니다.</summary>
        /// <typeparam name="T">생성할 인스턴스의 기준 참조 타입입니다.</typeparam>
        /// <param name="types">생성을 시도할 타입 시퀀스입니다.</param>
        /// <param name="factory">각 타입을 인스턴스로 변환할 팩터리입니다.</param>
        /// <returns>생성에 성공한 인스턴스의 지연 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="types"/> 또는 <paramref name="factory"/>가 null입니다.</exception>
        public static IEnumerable<T> CreateInstances<T>(
            IEnumerable<Type> types,
            Func<Type, T> factory)
            where T : class
        {
            return 
                from result 
                in CreateInstanceResults(types, factory) 
                where result.IsSuccess 
                select result.Instance;
        }

        /// <summary>지정한 타입들을 사용자 팩터리로 생성한 결과를 가져옵니다.</summary>
        /// <typeparam name="T">생성할 인스턴스의 기준 참조 타입입니다.</typeparam>
        /// <param name="types">생성을 시도할 타입 시퀀스입니다.</param>
        /// <param name="factory">각 타입을 인스턴스로 변환할 팩터리입니다.</param>
        /// <returns>성공과 실패를 모두 포함한 생성 결과의 지연 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="types"/> 또는 <paramref name="factory"/>가 null입니다.</exception>
        public static IEnumerable<TypeActivationResult<T>> CreateInstanceResults<T>(
            IEnumerable<Type> types,
            Func<Type, T> factory)
            where T : class
        {
            if (types == null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            foreach (Type type in types)
            {
                if (type == null)
                {
                    yield return TypeActivationResult<T>.Failed(
                        null,
                        TypeActivationFailure.InvalidType);
                    continue;
                }

                T instance = null;
                TypeActivationResult<T> failure = default;
                try
                {
                    instance = factory(type);
                }
                catch (Exception exception)
                {
                    failure = TypeActivationResult<T>.Failed(
                        type,
                        TypeActivationFailure.ActivationException,
                        exception);
                }

                if (failure.Failure != TypeActivationFailure.None)
                {
                    yield return failure;
                    continue;
                }

                if (instance != null)
                {
                    yield return TypeActivationResult<T>.Success(type, instance);
                    continue;
                }

                yield return TypeActivationResult<T>.Failed(
                    type,
                    TypeActivationFailure.FactoryReturnedNull);
            }
        }
    }
}
