using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// Defines the rules that can't be called in a test.
    /// </summary>
    public sealed class ExcludedRules
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludedRules"/> class.
        /// </summary>
        /// <param name="position">The location where the 'exclude' keyword was defined.</param>
        /// <param name="rules">The list of rules that can't be called in a test.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="rules"/> is null.
        /// </exception>
        public ExcludedRules(InputPosition position, StringList rules)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            DefinedAt = position;
            Rules = rules;
        }

        /// <summary>
        /// The location where the 'exclude' keyword was defined.
        /// </summary>
        public InputPosition DefinedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// The list of rules that can't be called in a test.
        /// </summary>
        public StringList Rules
        {
            get;
            private set;
        }
    }
}