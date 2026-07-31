using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jeomseon.Reflection
{
    /// <summary>
    /// 현재 AppDomain에 로드된 어셈블리에서 타입을 탐색합니다.
    /// </summary>
    public static class RuntimeTypeDiscovery
    {
        private static readonly ConcurrentDictionary<Assembly, Type[]> LoadableTypesByAssembly = new();

        /// <summary>이름으로 찾은 기준 타입에 할당 가능한 구체 클래스 이름을 가져옵니다.</summary>
        /// <param name="baseClass">기준 타입의 전체 이름 또는 단순 이름입니다.</param>
        /// <returns>발견된 구체 클래스의 단순 이름 시퀀스입니다.</returns>
        public static IEnumerable<string> GetClassNamesFromParent(string baseClass)
        {
            Type baseType = GetLoadableTypes()
                .FirstOrDefault(type => type.FullName == baseClass || type.Name == baseClass);

            return baseType == null
                ? Enumerable.Empty<string>()
                : GetChildTypesFromBaseType(baseType).Select(type => type.Name);
        }

        /// <summary>지정한 기준 타입에 할당 가능한 구체 클래스 이름을 가져옵니다.</summary>
        /// <typeparam name="TBaseType">기준 클래스 또는 인터페이스 타입입니다.</typeparam>
        /// <returns>발견된 구체 클래스의 단순 이름 시퀀스입니다.</returns>
        public static IEnumerable<string> GetClassNamesFromParent<TBaseType>() where TBaseType : class
        {
            return GetChildTypesFromBaseType(typeof(TBaseType)).Select(type => type.Name);
        }

        /// <summary>기준 타입에 할당 가능한 닫힌 구체 타입을 가져옵니다.</summary>
        /// <param name="baseType">기준 클래스 또는 인터페이스 타입입니다.</param>
        /// <returns>현재 AppDomain에서 발견된 구체 타입 시퀀스입니다.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="baseType"/>이 null입니다.</exception>
        public static IEnumerable<Type> GetChildTypesFromBaseType(Type baseType)
        {
            if (baseType == null)
            {
                throw new ArgumentNullException(nameof(baseType));
            }

            return GetLoadableTypes()
                .Where(type =>
                    type != baseType &&
                    !type.IsInterface &&
                    !type.IsAbstract &&
                    !type.ContainsGenericParameters &&
                    baseType.IsAssignableFrom(type));
        }

        /// <summary>지정한 기준 타입에 할당 가능한 닫힌 구체 타입을 가져옵니다.</summary>
        /// <typeparam name="T">기준 클래스 또는 인터페이스 타입입니다.</typeparam>
        /// <returns>현재 AppDomain에서 발견된 구체 타입 시퀀스입니다.</returns>
        public static IEnumerable<Type> GetChildTypesFromBaseType<T>()
        {
            return GetChildTypesFromBaseType(typeof(T));
        }

        /// <summary>직렬화된 타입 이름을 해석하고 해당 타입에 할당 가능한 구체 타입을 가져옵니다.</summary>
        /// <param name="typeName">어셈블리 이름과 타입 이름을 포함한 문자열입니다.</param>
        /// <returns>타입을 해석하지 못하면 빈 시퀀스이고, 그렇지 않으면 발견된 구체 타입입니다.</returns>
        public static IEnumerable<Type> GetChildClassesFromFieldTypeName(string typeName)
        {
            Type baseType = GetTypeFromFieldName(typeName);

            if (baseType == null)
            {
                return Enumerable.Empty<Type>();
            }

            return GetChildTypesFromBaseType(baseType);
        }

        /// <summary>직렬화된 필드 타입 이름을 현재 AppDomain의 타입으로 해석합니다.</summary>
        /// <param name="typeName">어셈블리 이름과 타입 이름을 포함한 문자열입니다.</param>
        /// <returns>해석한 타입이며, 찾을 수 없으면 null입니다.</returns>
        public static Type GetTypeFromFieldName(string typeName)
        {
            return TryGetTypeFromFieldName(typeName, out Type type) ? type : null;
        }

        /// <summary>직렬화된 필드 타입 이름을 현재 AppDomain의 타입으로 해석합니다.</summary>
        /// <param name="typeName">어셈블리 이름과 타입 이름을 포함한 문자열입니다.</param>
        /// <param name="type">성공하면 해석한 타입이고, 실패하면 null입니다.</param>
        /// <returns>타입을 찾았으면 <see langword="true"/>입니다.</returns>
        public static bool TryGetTypeFromFieldName(string typeName, out Type type)
        {
            type = null;

            if (string.IsNullOrWhiteSpace(typeName))
            {
                return false;
            }

            string[] splitTypeNames = typeName.Split(new[] { ' ', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitTypeNames.Length == 0)
            {
                return false;
            }

            string assemblyName = splitTypeNames[0];
            string baseTypeName = splitTypeNames[^1];
            Assembly targetAssembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(assembly => assembly.GetName().Name == assemblyName);

            if (targetAssembly == null)
            {
                return false;
            }

            type = GetLoadableTypes(targetAssembly)
                .FirstOrDefault(type => type.Name == baseTypeName);

            return type != null;
        }

        /// <summary>단순 이름이 일치하는 모든 열거형의 멤버 이름을 가져옵니다.</summary>
        /// <param name="enumTypeName">찾을 열거형의 단순 이름입니다.</param>
        /// <returns>발견된 열거형 멤버 이름 시퀀스입니다.</returns>
        public static IEnumerable<string> GetEnumValuesFromEnumName(string enumTypeName)
        {
            return GetLoadableTypes()
                .Where(type => type.IsEnum && type.Name == enumTypeName)
                .SelectMany(Enum.GetNames);
        }

        /// <summary>단순 이름이 일치하는 첫 열거형의 멤버 이름과 정수 값을 가져옵니다.</summary>
        /// <param name="enumTypeName">찾을 열거형의 단순 이름입니다.</param>
        /// <returns>멤버 이름과 정수 값의 사전이며, 열거형이 없으면 빈 사전입니다.</returns>
        public static Dictionary<string, int> GetEnumKvpFromEnumName(string enumTypeName)
        {
            Type enumType = GetLoadableTypes()
                .FirstOrDefault(type => type.IsEnum && type.Name == enumTypeName);

            if (enumType == null)
            {
                return new Dictionary<string, int>();
            }

            return Enum
                .GetNames(enumType)
                .ToDictionary(
                    name => name,
                    name => Convert.ToInt32(Enum.Parse(enumType, name)));
        }

        private static IEnumerable<Type> GetLoadableTypes()
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(GetLoadableTypes);
        }

        private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
        {
            return LoadableTypesByAssembly.GetOrAdd(assembly, LoadTypes);
        }

        private static Type[] LoadTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types
                    .Where(type => type != null)
                    .ToArray();
            }
        }
    }
}
