using NUnit.Framework;
using System.Collections.Generic;

namespace Spot.Ebnf
{
    /// <summary>
    /// Provides tests for the <see cref="IEnumerable1Extensions"/> interface.
    /// </summary>
    public class IEnumerable1ExtensionsTests
    {
        /// <summary>
        /// Lol <see cref="IEnumerable1Extensions.Combinations{T}(IEnumerable{T}, int)"/>
        /// </summary>
        [Test]
        public void Combinations()
        {
            var input = new[] { 'a', 'b', 'c' };
            var expected = new[]
            {
                new[] { 'a', 'a', 'a' },
                new[] { 'a', 'a', 'b' },
                new[] { 'a', 'a', 'c' },
                new[] { 'a', 'b', 'a' },
                new[] { 'a', 'b', 'b' },
                new[] { 'a', 'b', 'c' },
                new[] { 'a', 'c', 'a' },
                new[] { 'a', 'c', 'b' },
                new[] { 'a', 'c', 'c' },
                new[] { 'b', 'a', 'a' },
                new[] { 'b', 'a', 'b' },
                new[] { 'b', 'a', 'c' },
                new[] { 'b', 'b', 'a' },
                new[] { 'b', 'b', 'b' },
                new[] { 'b', 'b', 'c' },
                new[] { 'b', 'c', 'a' },
                new[] { 'b', 'c', 'b' },
                new[] { 'b', 'c', 'c' },
                new[] { 'c', 'a', 'a' },
                new[] { 'c', 'a', 'b' },
                new[] { 'c', 'a', 'c' },
                new[] { 'c', 'b', 'a' },
                new[] { 'c', 'b', 'b' },
                new[] { 'c', 'b', 'c' },
                new[] { 'c', 'c', 'a' },
                new[] { 'c', 'c', 'b' },
                new[] { 'c', 'c', 'c' },
            };

            CollectionAssert.AreEquivalent(expected, input.Combinations(input.Length));
        }
    }
}