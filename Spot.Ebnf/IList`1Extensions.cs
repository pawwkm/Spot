using Pote;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// Defines extension methods for the <see cref="IList{T}"/> interface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class IList1Extensions
    {
        /// <summary>
        /// Calculates all possible combinations of the given <paramref name="list"/>.
        /// All combinations will have the same length as the <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">The type of element in the <paramref name="list"/>.</typeparam>
        /// <param name="list">The original list of elements to calculate the combinations of.</param>
        /// <returns>All possible combinations of the <paramref name="list"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="list"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = Justifications.NestedGenericsAreUnavoidable)]
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IList<T> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            return list.Combinations(list.Count);
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
        public static IEnumerable<IEnumerable<T>> Combinations<T>(this IList<T> list, Range range)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));
            if (range == null)
                throw new ArgumentNullException(nameof(range));

            return IEnumerable1Extensions.Combinations(list, range);
        }
    }
}