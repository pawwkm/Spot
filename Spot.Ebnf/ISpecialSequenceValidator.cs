using Pote.Text;
using System;
using System.IO;

namespace Spot.Ebnf
{
    /// <summary>
    /// Validates a special sequence.
    /// </summary>
    public interface ISpecialSequenceValidator
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
        /// Consumes the given <paramref name="source"/> as dictated by the special 
        /// <paramref name="sequence"/>.
        /// </summary>
        /// <param name="source">The source to consume from.</param>
        /// <param name="sequence">The special sequence to interpret.</param>
        /// <param name="position">
        /// The current position in the source measured in characters.
        /// The position is increased if any character is consumed.
        /// </param>
        /// <returns>
        /// True if the special sequence consumed at least one character in 
        /// the <paramref name="source"/>; otherwise false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/>, <paramref name="sequence"/> or <paramref name="position"/> is null.
        /// </exception>
        bool Consume(Stream source, string sequence, InputPosition position);
    }
}