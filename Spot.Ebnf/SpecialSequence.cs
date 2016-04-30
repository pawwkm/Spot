using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a special sequence.
    /// </summary>
    public sealed class SpecialSequence : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpecialSequence"/> class.
        /// </summary>
        /// <param name="sequence">The value of the special sequence without question marks.</param>
        /// <param name="position">The position in the source where this special sequence were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sequence"/> or <paramref name="position"/> is null.
        /// </exception>
        public SpecialSequence(string sequence, InputPosition position) : base(position)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            Value = sequence;
        }

        /// <summary>
        /// The value of the special sequence without question marks.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
    }
}