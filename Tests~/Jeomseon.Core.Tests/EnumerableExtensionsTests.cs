using System;
using System.Collections.Generic;
using System.Linq;
using Jeomseon.Collections;
using NUnit.Framework;

namespace Jeomseon.Tests
{
    public sealed class EnumerableExtensionsTests
    {
        [Test]
        public void FallbackIfEmpty_ReturnsSourceWithoutEnumeratingFallback()
        {
            IEnumerable<int> fallback = ThrowWhenEnumerated();

            int[] result = new[] { 1, 2 }
                .FallbackIfEmpty(fallback)
                .ToArray();

            Assert.That(result, Is.EqualTo(new[] { 1, 2 }));
        }

        [Test]
        public void FallbackIfEmpty_ReturnsFallbackForEmptySource()
        {
            int[] result = Array.Empty<int>()
                .FallbackIfEmpty(new[] { 3, 4 })
                .ToArray();

            Assert.That(result, Is.EqualTo(new[] { 3, 4 }));
        }

        [Test]
        public void FallbackIfEmpty_ValidatesArgumentsImmediately()
        {
            IEnumerable<int> source = null;

            Assert.Throws<ArgumentNullException>(
                () => source.FallbackIfEmpty(Array.Empty<int>()));
            Assert.Throws<ArgumentNullException>(
                () => Array.Empty<int>().FallbackIfEmpty(null));
        }

        [Test]
        public void ForEach_InvokesActionForEveryElement()
        {
            List<int> values = new();

            ((IEnumerable<int>)new[] { 1, 2, 3 }).ForEach(values.Add);

            Assert.That(values, Is.EqualTo(new[] { 1, 2, 3 }));
        }

        [Test]
        public void ForEachNotNull_SkipsNullElements()
        {
            List<string> values = new();

            new[] { "first", null, "second" }.ForEachNotNull(values.Add);

            Assert.That(values, Is.EqualTo(new[] { "first", "second" }));
        }

        [Test]
        public void ForEachMethods_ValidateArguments()
        {
            IEnumerable<int> source = null;

            Assert.Throws<ArgumentNullException>(
                () => source.ForEach(_ => { }));
            Assert.Throws<ArgumentNullException>(
                () => Array.Empty<int>().ForEach(null));
            Assert.Throws<ArgumentNullException>(
                () => ((IEnumerable<string>)null).ForEachNotNull(_ => { }));
            Assert.Throws<ArgumentNullException>(
                () => Array.Empty<string>().ForEachNotNull(null));
        }

        private static IEnumerable<int> ThrowWhenEnumerated()
        {
            throw new InvalidOperationException("Fallback must not be enumerated.");
#pragma warning disable CS0162
            yield break;
#pragma warning restore CS0162
        }
    }
}
