using System;
using System.Linq;
using System.Reflection;
using Jeomseon.Reflection;
using NUnit.Framework;

namespace Jeomseon.Tests
{
    public sealed class ReflectionTests
    {
        public interface ITestService
        {
        }

        public sealed class TestService : ITestService
        {
        }

        public sealed class TestServiceWithoutDefaultConstructor : ITestService
        {
            public TestServiceWithoutDefaultConstructor(string value)
            {
            }
        }

        public sealed class TestServiceWithThrowingConstructor : ITestService
        {
            public TestServiceWithThrowingConstructor()
            {
                throw new InvalidOperationException("Activation failed.");
            }
        }

        private abstract class AbstractTestService : ITestService
        {
        }

        private enum TestValue
        {
            First = 3,
            Second = 7
        }

        private sealed class FieldTarget
        {
            public string PublicValue = "public";
            private readonly int _privateValue = 17;

            public int ExpectedPrivateValue => _privateValue;
        }

        [Test]
        public void GetChildTypes_FiltersInterfacesAndAbstractTypes()
        {
            Type[] types = RuntimeTypeDiscovery.GetChildTypesFromBaseType<ITestService>().ToArray();

            Assert.That(types, Does.Contain(typeof(TestService)));
            Assert.That(types, Does.Not.Contain(typeof(AbstractTestService)));
            Assert.That(types, Does.Not.Contain(typeof(ITestService)));
        }

        [Test]
        public void CreateChildClasses_CreatesConcreteImplementations()
        {
            ITestService[] instances = TypeActivator.CreateDerivedInstances<ITestService>().ToArray();

            Assert.That(instances.Any(instance => instance is TestService), Is.True);
            Assert.That(instances.Any(instance => instance is TestServiceWithoutDefaultConstructor), Is.False);
            Assert.That(instances.Any(instance => instance is TestServiceWithThrowingConstructor), Is.False);
        }

        [Test]
        public void CreateInstanceResults_ReportsMissingDefaultConstructor()
        {
            TypeActivationResult<ITestService> result = TypeActivator
                .CreateInstanceResults<ITestService>(
                    new[] { typeof(TestServiceWithoutDefaultConstructor) })
                .Single();

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Type, Is.EqualTo(typeof(TestServiceWithoutDefaultConstructor)));
            Assert.That(result.Failure, Is.EqualTo(TypeActivationFailure.MissingDefaultConstructor));
            Assert.That(result.Exception, Is.Null);
        }

        [Test]
        public void CreateInstanceResults_ReportsUnderlyingConstructorException()
        {
            TypeActivationResult<ITestService> result = TypeActivator
                .CreateInstanceResults<ITestService>(
                    new[] { typeof(TestServiceWithThrowingConstructor) })
                .Single();

            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Failure, Is.EqualTo(TypeActivationFailure.ActivationException));
            Assert.That(result.Exception, Is.TypeOf<InvalidOperationException>());
            Assert.That(result.Exception.Message, Is.EqualTo("Activation failed."));
        }

        [Test]
        public void CreateInstanceResults_ReportsFactoryFailureWithoutStoppingEnumeration()
        {
            Type[] types =
            {
                typeof(TestServiceWithThrowingConstructor),
                typeof(TestService)
            };

            TypeActivationResult<ITestService>[] results = TypeActivator
                .CreateInstanceResults<ITestService>(
                    types,
                    type =>
                    {
                        if (type == typeof(TestServiceWithThrowingConstructor))
                        {
                            throw new InvalidOperationException("Factory failed.");
                        }

                        return new TestService();
                    })
                .ToArray();

            Assert.That(results[0].Failure, Is.EqualTo(TypeActivationFailure.ActivationException));
            Assert.That(results[0].Exception.Message, Is.EqualTo("Factory failed."));
            Assert.That(results[1].IsSuccess, Is.True);
            Assert.That(results[1].Instance, Is.TypeOf<TestService>());
        }

        [Test]
        public void TryGetTypeFromFieldName_ReturnsFalseForInvalidInput()
        {
            bool result = RuntimeTypeDiscovery.TryGetTypeFromFieldName(string.Empty, out Type type);

            Assert.That(result, Is.False);
            Assert.That(type, Is.Null);
        }

        [Test]
        public void EnumLookup_ReturnsNamesAndValues()
        {
            string[] names = RuntimeTypeDiscovery.GetEnumValuesFromEnumName(nameof(TestValue)).ToArray();
            var values = RuntimeTypeDiscovery.GetEnumKvpFromEnumName(nameof(TestValue));

            Assert.That(names, Is.EquivalentTo(new[] { nameof(TestValue.First), nameof(TestValue.Second) }));
            Assert.That(values[nameof(TestValue.First)], Is.EqualTo(3));
            Assert.That(values[nameof(TestValue.Second)], Is.EqualTo(7));
        }

        [Test]
        public void GetFieldInfo_ReturnsCachedFieldMetadata()
        {
            FieldInfo first = MemberReflection.GetFieldInfo(
                typeof(FieldTarget),
                nameof(FieldTarget.PublicValue));
            FieldInfo second = MemberReflection.GetFieldInfo(
                typeof(FieldTarget),
                nameof(FieldTarget.PublicValue));

            Assert.That(first, Is.Not.Null);
            Assert.That(second, Is.SameAs(first));
        }

        [Test]
        public void TryGetFieldValue_ReadsPublicAndPrivateFields()
        {
            FieldTarget target = new();

            bool foundPublic = MemberReflection.TryGetFieldValue(
                target,
                nameof(FieldTarget.PublicValue),
                out string publicValue);
            bool foundPrivate = MemberReflection.TryGetFieldValue(
                target,
                "_privateValue",
                out int privateValue);

            Assert.That(foundPublic, Is.True);
            Assert.That(publicValue, Is.EqualTo("public"));
            Assert.That(foundPrivate, Is.True);
            Assert.That(privateValue, Is.EqualTo(target.ExpectedPrivateValue));
        }

        [Test]
        public void TryGetFieldValue_ReturnsFalseForMissingOrIncompatibleField()
        {
            FieldTarget target = new();

            bool missing = MemberReflection.TryGetFieldValue(
                target,
                "Missing",
                out object missingValue);
            bool incompatible = MemberReflection.TryGetFieldValue(
                target,
                nameof(FieldTarget.PublicValue),
                out int incompatibleValue);

            Assert.That(missing, Is.False);
            Assert.That(missingValue, Is.Null);
            Assert.That(incompatible, Is.False);
            Assert.That(incompatibleValue, Is.Zero);
        }

        [Test]
        public void TryGetFieldValue_ReturnsFalseForNullTarget()
        {
            bool found = MemberReflection.TryGetFieldValue(
                null,
                nameof(FieldTarget.PublicValue),
                out object value);

            Assert.That(found, Is.False);
            Assert.That(value, Is.Null);
        }
    }
}
