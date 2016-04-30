using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a definition in a syntax described in Ebnf.
    /// </summary>
    public abstract class Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Definition"/> class.
        /// </summary>
        /// <param name="position">The position in the source where this definition were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        protected Definition(InputPosition position)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            DefinedAt = position;
        }

        /// <summary>
        /// The position in the source where this definition were defined.
        /// </summary>
        public InputPosition DefinedAt
        {
            get;
            private set;
        }
    }
}