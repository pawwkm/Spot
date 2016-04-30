using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a sequence that is repeated 0 or more times.
    /// </summary>
    public sealed class RepeatedSequence : Sequence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepeatedSequence"/> class.
        /// </summary>
        /// <param name="position">The position in the source where this repeated sequence were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public RepeatedSequence(InputPosition position) : base(position)
        {
        }
    }
}