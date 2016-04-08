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