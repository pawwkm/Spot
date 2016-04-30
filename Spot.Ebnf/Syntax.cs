using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents the syntax of a language in Ebnf.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = Justifications.MissingCollectionSuffix)]
    public sealed class Syntax : IEnumerable<Rule>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Syntax"/> class.
        /// </summary>
        /// <param name="rules">The rules of the syntax.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="rules"/> is null.
        /// </exception>
        public Syntax(IEnumerable<Rule> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            Rules = new List<Rule>(rules);
        }

        /// <summary>
        /// All rules of the syntax.
        /// </summary>
        public IEnumerable<Rule> Rules
        {
            get;
            private set;
        }

        /// <summary>
        /// The starting rule of the syntax.
        /// </summary>
        public Rule Start
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a rule by its name.
        /// </summary>
        /// <param name="name">The name of the rule to get.</param>
        /// <returns>
        /// The rule with the given <paramref name="name"/> if it 
        /// exists in the syntax; otherwise null.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// </exception>
        public Rule GetRuleBy(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            foreach (Rule rule in this)
            {
                if (rule.Name == name)
                    return rule;
            }

            return null;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the syntax's rules.
        /// </summary>
        /// <returns>An enumerator that iterates through the syntax's rules.</returns>
        public IEnumerator<Rule> GetEnumerator()
        {
            return Rules.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the syntax's rules.
        /// </summary>
        /// <returns>An enumerator that iterates through the syntax's rules.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Rules.GetEnumerator();
        }
    }
}