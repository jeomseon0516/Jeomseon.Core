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

        public static IEnumerable<string> GetClassNamesFromParent(string baseClass)
        {
            Type baseType = GetLoadableTypes()
                .FirstOrDefault(type => type.FullName == baseClass || type.Name == baseClass);

            return baseType == null
                ? Enumerable.Empty<string>()
                : GetChildTypesFromBaseType(baseType).Select(type => type.Name);
        }

        public static IEnumerable<string> GetClassNamesFromParent<TBaseType>() where TBaseType : class
        {
            return GetChildTypesFromBaseType(typeof(TBaseType)).Select(type => type.Name);
        }

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

        public static IEnumerable<Type> GetChildTypesFromBaseType<T>()
        {
            return GetChildTypesFromBaseType(typeof(T));
        }

        public static IEnumerable<Type> GetChildClassesFromFieldTypeName(string typeName)
        {
            Type baseType = GetTypeFromFieldName(typeName);

            if (baseType == null)
            {
                return Enumerable.Empty<Type>();
            }

            return GetChildTypesFromBaseType(baseType);
        }

        public static Type GetTypeFromFieldName(string typeName)
        {
            return TryGetTypeFromFieldName(typeName, out Type type) ? type : null;
        }

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

        public static IEnumerable<string> GetEnumValuesFromEnumName(string enumTypeName)
        {
            return GetLoadableTypes()
                .Where(type => type.IsEnum && type.Name == enumTypeName)
                .SelectMany(Enum.GetNames);
        }

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
