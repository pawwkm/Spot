using Pote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Spot.Ebnf
{
    /// <summary>
    /// Defines extension methods for the <see cref="IEnumerable{T}"/> interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IEnumerable1Extensions
    {
        /// <summary>
        /// Calculates all possible combinations of the given <paramref name="list"/>.
        /// All combinations will have the same <paramref name="length"/>.
        /// </summary>
        /// <typeparam name="T">The type of element in the <paramref name="list"/>.</typeparam>
        /// <param name="list">The original list of elements to calculate the combinations of.</param>
        /// <param name="length">The length of each generated list.</param>
        /// <returns>All possible combinations of the <paramref name="list"/> with the given <paramref name="length"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="length"/> is less then 1.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = Justifications.NestedGenericsAreUnavoidable)]
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> list, int length)
        {
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length));

            if (length == 1) 
                return list.Select(t => new T[] { t });

            return Combinations(list, length - 1).SelectMany(t => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        /// <summary>
        /// Calculates all possible combinations of the given <paramref name="list"/>.
        /// The length of each combination is equal to the given <paramref name="range"/>.
        /// </summary>
        /// <typeparam name="T">The type of element in the <paramref name="list"/>.</typeparam>
        /// <param name="list">The original list of elements to calculate the combinations of.</param>
        /// <param name="range">The range to calculate all combinations in.</param>
        /// <returns>All possible combinations of the <paramref name="list"/> within the given <paramref name="range"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> or <paramref name="range"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = Justifications.NestedGenericsAreUnavoidable)]
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IEnumerable<T> list, Range range)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (range == null)
                throw new ArgumentNullException(nameof(range));

            foreach (var amount in range)
            {
                if (amount == 0)
                    yield return new T[0];
                else
                {
                    foreach (var set in list.Combinations(amount))
                        yield return set;
                }
            }
        }
    }
}