using Pote.Text;
using System;
using System.Diagnostics;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a meta identifier.
    /// </summary>
    [DebuggerDisplay("{Value, nq}")]
    public sealed class MetaIdentifier : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetaIdentifier"/> class.
        /// </summary>
        /// <param name="name">The meta identifier.</param>
        /// <param name="position">The position in the source where this meta identifier were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> or <paramref name="position"/> is null.
        /// </exception>
        public MetaIdentifier(string name, InputPosition position) : base(position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
        }

        /// <summary>
        /// The meta identifier.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }
}