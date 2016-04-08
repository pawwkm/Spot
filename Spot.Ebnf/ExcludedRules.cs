using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// Defines a list of rules that should not be called by the 
    /// <see cref="SyntaxValidator"/> when validating a syntax.
    /// Any rule that is not defined in this list is called.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = Justifications.MissingCollectionSuffix)]
    public sealed class ExcludedRules : IEnumerable<string>
    {
        private List<string> excludedRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludedRules"/> class.
        /// </summary>
        /// <param name="rules">The rules that is to be excluded.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="rules"/> is null.
        /// </exception>
        public ExcludedRules(IEnumerable<string> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            excludedRules = new List<string>(rules);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludedRules"/> class.
        /// </summary>
        /// <param name="rules">The rules that is to be excluded.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="rules"/> is null.
        /// </exception>
        public ExcludedRules(params string[] rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            
            excludedRules = new List<string>(rules);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the excluded rules.
        /// </summary>
        /// <returns>An enumerator that iterates through the excluded rules.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return excludedRules.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the excluded rules.
        /// </summary>
        /// <returns>An enumerator that iterates through the excluded rules.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return excludedRules.GetEnumerator();
        }
    }
}