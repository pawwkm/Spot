using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents an optional sequence. The sequence occurs only once or not at all.
    /// </summary>
    public sealed class OptionalSequence : Sequence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionalSequence"/> class.
        /// </summary>
        /// <param name="position">The position in the source where this empty sequence were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public OptionalSequence(InputPosition position) : base(position)
        {
        }
    }
}