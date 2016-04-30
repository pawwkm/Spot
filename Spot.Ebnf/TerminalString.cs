using Pote.Text;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a terminal string.
    /// </summary>
    [DebuggerDisplay("{Value}")]
    public sealed class TerminalString : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalString"/> class.
        /// </summary>
        /// <param name="value">The value of the terminal string without quotes.</param>
        /// <param name="position">The position in the source where this terminal string were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> or <paramref name="position"/> is null.
        /// </exception>
        public TerminalString(string value, InputPosition position) : base(position)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            Value = value;
        }

        /// <summary>
        /// The value of the terminal string without quotes.
        /// </summary>
        public string Value
        {
            get;
            private set;
        }
    }
}