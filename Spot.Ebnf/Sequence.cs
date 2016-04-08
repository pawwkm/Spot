using System.Collections.Generic;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a syntactic sequence.
    /// </summary>
    public abstract class Sequence : Definition
    {
        private List<DefinitionList> branches = new List<DefinitionList>();

        /// <summary>
        /// The possible branches in this sequence.
        /// </summary>
        public IList<DefinitionList> Branches
        {
            get
            {
                return branches;
            }
        }
    }
}