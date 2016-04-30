using Pote.Text;
using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a syntactic sequence.
    /// </summary>
    public abstract class Sequence : Definition
    {
        private DefinitionList branches = new DefinitionList();

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="position">The position in the source where this sequence were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public Sequence(InputPosition position) : base(position)
        {
        }

        /// <summary>
        /// The possible branches in this sequence.
        /// </summary>
        public DefinitionList Branches
        {
            get
            {
                return branches;
            }
        }
    }
}