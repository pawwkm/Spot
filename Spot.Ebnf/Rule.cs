using Pote.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Spot.Ebnf
{
    /// <summary>
    /// A syntax rule.
    /// </summary>
    [DebuggerDisplay("{Name, nq}")]
    public sealed class Rule
    {
        private DefinitionList branches = new DefinitionList();

        private List<Rule> referencedBy = new List<Rule>();

        private List<Rule> rulesReferenced = new List<Rule>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Rule"/> class.
        /// </summary>
        /// <param name="name">The name of the rule.</param>
        /// <param name="position">The position in the source where this rule were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> or <paramref name="position"/> is null.
        /// </exception>
        public Rule(string name, InputPosition position)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Name = name;
            DefinedAt = position;
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

        /// <summary>
        /// The position in the source where this rule were defined.
        /// </summary>
        public InputPosition DefinedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// The name of the rule.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }
    }
}