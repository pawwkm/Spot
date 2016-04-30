using Pote.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spot.Ebnf
{
    /// <summary>
    /// A syntax rule.
    /// </summary>
    [DebuggerDisplay("{MetaIdentifier.Text, nq}")]
    public sealed class Rule
    {
        private DefinitionList branches = new DefinitionList();

        private List<Rule> referencedBy = new List<Rule>();

        private List<Rule> rulesReferenced = new List<Rule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="metaIdentifier">The name of the rule.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="metaIdentifier"/> is null.
        /// </exception>
        public Rule(Token<TokenType> metaIdentifier)
        {
            if (metaIdentifier == null)
                throw new ArgumentNullException(nameof(metaIdentifier));

            MetaIdentifier = metaIdentifier;
        }

        /// <summary>
        /// The name of the rule.
        /// </summary>
        public Token<TokenType> MetaIdentifier
        {
            get;
            private set;
        }

        /// <summary>
        /// The branches of the rule.
        /// </summary>
        public DefinitionList Branches
        {
            get
            {
                return branches;
            }
        }

        /// <summary>
        /// The rules that references this rule.
        /// </summary>
        public IList<Rule> ReferencedBy
        {
            get
            {
                return referencedBy;
            }
        }

        /// <summary>
        /// The rules that are referenced by this rule.
        /// </summary>
        public IList<Rule> RulesReferenced
        {
            get
            {
                return rulesReferenced;
            }
        }
    }
}