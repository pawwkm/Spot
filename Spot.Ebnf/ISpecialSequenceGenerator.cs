using System;
using System.Collections.Generic;

namespace Spot.Ebnf
{
    /// <summary>
    /// Generates strings based on an special sequence.
    /// </summary>
    public interface ISpecialSequenceGenerator
    {
        /// <summary>
        /// Checks that the given sequence is valid.
        /// </summary>
        /// <param name="sequence">The sequence to validate.</param>
        /// <returns>True if the sequence is valid; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        bool IsValid(string sequence);

        /// <summary>
        /// Generates all possible strings for the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">The sequence to generate strings from.</param>
        /// <returns>All generated strings.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> is null.
        /// </exception>
        IEnumerable<string> Generate(string sequence);
    }
}