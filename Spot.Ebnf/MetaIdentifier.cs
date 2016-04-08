using Pote.Text;
using System;
using System.Diagnostics;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a meta identifier.
    /// </summary>
    [DebuggerDisplay("{Value.Text, nq}")]
    public sealed class MetaIdentifier : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaIdentifier"/> class.
        /// </summary>
        /// <param name="value">The value of the meta identifier.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public MetaIdentifier(Token<TokenType> value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        /// <summary>
        /// The value of the meta identifier.
        /// </summary>
        public Token<TokenType> Value
        {
            get;
            private set;
        }
    }
}