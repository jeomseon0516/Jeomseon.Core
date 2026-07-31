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
            Fields = new();

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
            return Fields
                .GetOrAdd(
                    key,
                    cacheKey => new FieldLookupResult(
                        cacheKey.Type.GetField(
                            cacheKey.FieldName,
                            cacheKey.BindingFlags)))
                .FieldInfo;
        }

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

        public static T GetFieldValue<T>(
            object target,
            string fieldName,
            BindingFlags bindingFlags = DefaultInstanceFlags)
        {
            return GetFieldValue(target, fieldName, bindingFlags) is T value
                ? value
                : default;
        }

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

        public static IEnumerable<Attribute> GetTypeAttributes(object target)
        {
            return target == null
                ? Array.Empty<Attribute>()
                : target.GetType().GetCustomAttributes<Attribute>();
        }

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
