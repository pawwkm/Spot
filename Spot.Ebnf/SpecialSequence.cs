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
        /// <param name="value">The value of the special sequence without question marks.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public SpecialSequence(Token<TokenType> value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        /// <summary>
        /// The value of the special sequence without question marks.
        /// </summary>
        public Token<TokenType> Value
        {
            get;
            private set;
        }
    }
}