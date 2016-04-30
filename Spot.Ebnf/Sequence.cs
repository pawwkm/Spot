using System.Collections.Generic;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a syntactic sequence.
    /// </summary>
    public abstract class Sequence : Definition
    {
        private DefinitionList branches = new DefinitionList();

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