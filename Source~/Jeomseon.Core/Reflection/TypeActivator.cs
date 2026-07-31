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
        public static IEnumerable<T> CreateDerivedInstances<T>()
            where T : class
        {
            return CreateInstances<T>(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>());
        }

        public static IEnumerable<T> CreateDerivedInstances<T>(Func<Type, T> factory)
            where T : class
        {
            return CreateInstances(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>(),
                factory);
        }

        public static IEnumerable<TypeActivationResult<T>> CreateDerivedInstanceResults<T>()
            where T : class
        {
            return CreateInstanceResults<T>(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>());
        }

        public static IEnumerable<TypeActivationResult<T>> CreateDerivedInstanceResults<T>(
            Func<Type, T> factory)
            where T : class
        {
            return CreateInstanceResults(
                RuntimeTypeDiscovery.GetChildTypesFromBaseType<T>(),
                factory);
        }

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
