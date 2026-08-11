using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Jeomseon.Reflection
{
    using Attribute = System.Attribute;

    /// <summary>
    /// 특정 객체나 멤버의 CLR Reflection 메타데이터를 조회합니다.
    /// </summary>
    public static class MemberReflection
    {
        private const BindingFlags DefaultInstanceFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static readonly ConcurrentDictionary<FieldCacheKey, FieldLookupResult>
            _fields = new();

        /// <summary>메서드가 선언된 타입에서 이름과 바인딩 옵션에 맞는 필드를 찾습니다.</summary>
        /// <param name="method">필드 검색의 기준이 되는 메서드입니다.</param>
        /// <param name="fieldName">찾을 필드 이름입니다.</param>
        /// <param name="bindingFlags">필드 검색에 사용할 바인딩 옵션입니다.</param>
        /// <returns>찾은 필드 정보이며, 입력이 유효하지 않거나 필드가 없으면 null입니다.</returns>
        public static FieldInfo GetFieldInfo(
            MethodInfo method,
            string fieldName,
            BindingFlags bindingFlags = DefaultInstanceFlags)
        {
            if (method == null || string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            return GetFieldInfo(method.DeclaringType, fieldName, bindingFlags);
        }

        /// <summary>타입에서 이름과 바인딩 옵션에 맞는 필드를 찾아 캐시합니다.</summary>
        /// <param name="type">필드를 선언한 타입입니다.</param>
        /// <param name="fieldName">찾을 필드 이름입니다.</param>
        /// <param name="bindingFlags">필드 검색에 사용할 바인딩 옵션입니다.</param>
        /// <returns>찾은 필드 정보이며, 입력이 유효하지 않거나 필드가 없으면 null입니다.</returns>
        public static FieldInfo GetFieldInfo(
            Type type,
            string fieldName,
            BindingFlags bindingFlags = DefaultInstanceFlags)
        {
            if (type == null || string.IsNullOrEmpty(fieldName))
            {
                return null;
            }

            FieldCacheKey key = new(type, fieldName, bindingFlags);
            return _fields
                .GetOrAdd(
                    key,
                    cacheKey => new FieldLookupResult(
                        cacheKey.Type.GetField(
                            cacheKey.FieldName,
                            cacheKey.BindingFlags)))
                .FieldInfo;
        }

        /// <summary>객체에서 지정한 필드의 값을 가져옵니다.</summary>
        /// <param name="target">값을 읽을 객체입니다.</param>
        /// <param name="fieldName">읽을 필드 이름입니다.</param>
        /// <param name="bindingFlags">필드 검색에 사용할 바인딩 옵션입니다.</param>
        /// <returns>필드 값이며, 객체 또는 필드를 찾을 수 없으면 null입니다.</returns>
        public static object GetFieldValue(
            object target,
            string fieldName,
            BindingFlags bindingFlags = DefaultInstanceFlags)
        {
            if (target == null)
            {
                return null;
            }

            return GetFieldInfo(target.GetType(), fieldName, bindingFlags)?
                .GetValue(target);
        }

        /// <summary>객체에서 지정한 필드 값을 호환되는 타입으로 가져옵니다.</summary>
        /// <typeparam name="T">반환할 값의 타입입니다.</typeparam>
        /// <param name="target">값을 읽을 객체입니다.</param>
        /// <param name="fieldName">읽을 필드 이름입니다.</param>
        /// <param name="bindingFlags">필드 검색에 사용할 바인딩 옵션입니다.</param>
        /// <returns>타입이 호환되는 필드 값이며, 읽을 수 없으면 기본값입니다.</returns>
        public static T GetFieldValue<T>(
            object target,
            string fieldName,
            BindingFlags bindingFlags = DefaultInstanceFlags)
        {
            return GetFieldValue(target, fieldName, bindingFlags) is T value
                ? value
                : default;
        }

        /// <summary>객체에서 지정한 필드 값을 가져옵니다.</summary>
        /// <param name="target">값을 읽을 객체입니다.</param>
        /// <param name="fieldName">읽을 필드 이름입니다.</param>
        /// <param name="value">성공하면 필드 값이고, 실패하면 null입니다.</param>
        /// <param name="bindingFlags">필드 검색에 사용할 바인딩 옵션입니다.</param>
        /// <returns>필드를 찾아 값을 읽었으면 <see langword="true"/>입니다.</returns>
        public static bool TryGetFieldValue(
            object target,
            string fieldName,
            out object value,
            BindingFlags bindingFlags = DefaultInstanceFlags)
        {
            value = null;
            if (target == null)
            {
                return false;
            }

            FieldInfo field = GetFieldInfo(
                target.GetType(),
                fieldName,
                bindingFlags);
            if (field == null)
            {
                return false;
            }

            value = field.GetValue(target);
            return true;
        }

        /// <summary>객체에서 지정한 필드 값을 호환되는 타입으로 가져옵니다.</summary>
        /// <typeparam name="T">가져올 값의 타입입니다.</typeparam>
        /// <param name="target">값을 읽을 객체입니다.</param>
        /// <param name="fieldName">읽을 필드 이름입니다.</param>
        /// <param name="value">성공하면 변환된 필드 값이고, 실패하면 기본값입니다.</param>
        /// <param name="bindingFlags">필드 검색에 사용할 바인딩 옵션입니다.</param>
        /// <returns>필드를 찾아 호환되는 값으로 읽었으면 <see langword="true"/>입니다.</returns>
        public static bool TryGetFieldValue<T>(
            object target,
            string fieldName,
            out T value,
            BindingFlags bindingFlags = DefaultInstanceFlags)
        {
            if (TryGetFieldValue(
                    target,
                    fieldName,
                    out object rawValue,
                    bindingFlags) &&
                rawValue is T typedValue)
            {
                value = typedValue;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>객체 타입에 선언된 모든 특성을 가져옵니다.</summary>
        /// <param name="target">타입 특성을 조회할 객체입니다.</param>
        /// <returns>타입에 선언된 특성이며, 객체가 null이면 빈 시퀀스입니다.</returns>
        public static IEnumerable<Attribute> GetTypeAttributes(object target)
        {
            return target == null
                ? Array.Empty<Attribute>()
                : target.GetType().GetCustomAttributes<Attribute>();
        }

        /// <summary>객체 타입에 지정한 특성이 선언되어 있는지 확인합니다.</summary>
        /// <typeparam name="TAttribute">확인할 특성 타입입니다.</typeparam>
        /// <param name="target">타입 특성을 조회할 객체입니다.</param>
        /// <returns>지정한 특성이 있으면 <see langword="true"/>입니다.</returns>
        public static bool HasTypeAttribute<TAttribute>(object target)
            where TAttribute : Attribute
        {
            return target?.GetType().GetCustomAttribute<TAttribute>() != null;
        }

        private readonly struct FieldLookupResult
        {
            public FieldLookupResult(FieldInfo fieldInfo)
            {
                FieldInfo = fieldInfo;
            }

            public FieldInfo FieldInfo { get; }
        }

        private readonly struct FieldCacheKey : IEquatable<FieldCacheKey>
        {
            public FieldCacheKey(
                Type type,
                string fieldName,
                BindingFlags bindingFlags)
            {
                Type = type;
                FieldName = fieldName;
                BindingFlags = bindingFlags;
            }

            public Type Type { get; }
            public string FieldName { get; }
            public BindingFlags BindingFlags { get; }

            public bool Equals(FieldCacheKey other)
            {
                return Type == other.Type &&
                       string.Equals(
                           FieldName,
                           other.FieldName,
                           StringComparison.Ordinal) &&
                       BindingFlags == other.BindingFlags;
            }

            public override bool Equals(object obj)
            {
                return obj is FieldCacheKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = Type.GetHashCode();
                    hashCode = (hashCode * 397) ^
                               StringComparer.Ordinal.GetHashCode(FieldName);
                    return (hashCode * 397) ^ (int)BindingFlags;
                }
            }
        }
    }
}
