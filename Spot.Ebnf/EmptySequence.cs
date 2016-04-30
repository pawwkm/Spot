using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents an empty sequence.
    /// </summary>
    public sealed class EmptySequence : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptySequence"/> class.
        /// </summary>
        /// <param name="position">The position in the source where this empty sequence were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public EmptySequence(InputPosition position) : base(position)
        {
        }
    }
}