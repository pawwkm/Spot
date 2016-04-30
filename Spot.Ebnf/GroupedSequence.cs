using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a grouped sequence.
    /// </summary>
    public sealed class GroupedSequence : Sequence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupedSequence"/> class.
        /// </summary>
        /// <param name="position">The position in the source where this grouped sequence were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public GroupedSequence(InputPosition position) : base(position)
        {
        }
    }
}