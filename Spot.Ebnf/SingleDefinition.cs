using Pote.Text;
using System;
using System.Collections.Generic;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a single definition.
    /// </summary>
    public sealed class SingleDefinition : Definition
    {
        private List<SyntacticTerm> syntacticTerms = new List<SyntacticTerm>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleDefinition"/> class.
        /// </summary>
        /// <param name="position">The position in the source where this definition were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="position"/> is null.
        /// </exception>
        public SingleDefinition(InputPosition position) : base(position)
        {
        }

        /// <summary>
        /// The syntactic terms that are contained by this definition.
        /// </summary>
        public IList<SyntacticTerm> SyntacticTerms
        {
            get
            {
                return syntacticTerms;
            }
        }
    }
}