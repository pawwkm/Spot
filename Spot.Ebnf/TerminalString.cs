using Pote.Text;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a terminal string.
    /// </summary>
    [DebuggerDisplay("{Value.Text}")]
    public sealed class TerminalString : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalString"/> class.
        /// </summary>
        /// <param name="value">The value of the terminal string without quotes.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public TerminalString(Token<TokenType> value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        /// <summary>
        /// The value of the terminal string without quotes.
        /// </summary>
        public Token<TokenType> Value
        {
            get;
            private set;
        }

        /// <summary>
        /// Converts the terminal to a string.
        /// </summary>
        /// <param name="terminal">The terminal to convert.</param>
        /// <returns>The text value of the terminal.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="terminal"/> is null.
        /// </exception>
        [SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = Justifications.NotClsCompliant)]
        public static explicit operator string(TerminalString terminal)
        {
            if (terminal == null)
                throw new ArgumentNullException(nameof(terminal));

            return terminal.Value.Text;
        }
    }
}