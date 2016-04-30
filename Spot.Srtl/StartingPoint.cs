using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot.SrtL
{
    /// <summary>
    /// Defines the starting rule of a SrtL test.
    /// </summary>
    public sealed class StartingPoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartingPoint"/> class.
        /// </summary>
        /// <param name="position">The location where the starting point was defined.</param>
        /// <param name="rule">The name of the rule to start testing from.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> or <paramref name="rule"/> is null.
        /// </exception>
        public StartingPoint(InputPosition position, String rule)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));
            if (rule == null)
                throw new ArgumentNullException(nameof(rule));

            DefinedAt = position;
            Rule = rule;
        }

        /// <summary>
        /// The position where the first keyword where declared.
        /// </summary>
        public InputPosition DefinedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the rule to start from.
        /// </summary>
        public String Rule
        {
            get;
            private set;
        }
    }
}