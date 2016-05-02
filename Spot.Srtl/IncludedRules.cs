using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// Defines the rules that can be called in a test.
    /// </summary>
    public sealed class IncludedRules
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludedRules"/> class.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rules"></param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="rules"/> is null.
        /// </exception>
        public IncludedRules(InputPosition position, StringList rules)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            DefinedAt = position;
            Rules = rules;
        }

        /// <summary>
        /// The location where the 'include' keyword was defined.
        /// </summary>
        public InputPosition DefinedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of rules that can be called in a test.
        /// </summary>
        public StringList Rules
        {
            get;
            private set;
        }
    }
}