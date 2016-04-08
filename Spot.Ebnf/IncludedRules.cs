using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Spot.Ebnf
{
    /// <summary>
    /// Defines a list of rules that should be called by the 
    /// <see cref="SyntaxValidator"/> when validating a syntax.
    /// Any rule that is not defined in this list is not called.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = Justifications.MissingCollectionSuffix)]
    public sealed class IncludedRules : IEnumerable<string>
    {
        private List<string> includedRules;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncludedRules"/> class.
        /// </summary>
        /// <param name="rules">The rules that is to be included.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="rules"/> is null.
        /// </exception>
        public IncludedRules(IEnumerable<string> rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            includedRules = new List<string>(rules);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncludedRules"/> class.
        /// </summary>
        /// <param name="rules">The rules that is to be included.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="rules"/> is null.
        /// </exception>
        public IncludedRules(params string[] rules)
        {
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));
            
            includedRules = new List<string>(rules);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the included rules.
        /// </summary>
        /// <returns>An enumerator that iterates through the included rules.</returns>
        public IEnumerator<string> GetEnumerator()
        {
            return includedRules.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the included rules.
        /// </summary>
        /// <returns>An enumerator that iterates through the included rules.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return includedRules.GetEnumerator();
        }
    }
}