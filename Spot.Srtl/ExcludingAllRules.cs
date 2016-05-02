using Pote.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spot.SrtL
{
    /// <summary>
    /// Dictates that all rules in a test should be ignored.
    /// </summary>
    public class ExcludingAllRules
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludingAllRules"/> class.
        /// </summary>
        /// <param name="exclude">The position the 'exclude' keyword was defined.</param>
        /// <param name="all">The position the 'all' keyword was defined.</param>
        /// <param name="rules">The position the 'rules' keyword was defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="exclude"/>, <paramref name="all"/> or <paramref name="rules"/> is null.
        /// </exception>
        public ExcludingAllRules(InputPosition exclude, InputPosition all, InputPosition rules)
        {
            if (exclude == null)
                throw new ArgumentNullException(nameof(exclude));
            if (all == null)
                throw new ArgumentNullException(nameof(all));
            if (rules == null)
                throw new ArgumentNullException(nameof(rules));

            ExcludeDefinedAt = exclude;
            AllDefinedAt = all;
            RulesDefinedAt = rules;
        }

        /// <summary>
        /// The position the 'exclude' keyword was defined.
        /// </summary>
        public InputPosition ExcludeDefinedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// The position the 'all' keyword was defined.
        /// </summary>
        public InputPosition AllDefinedAt
        {
            get;
            private set;
        }

        /// <summary>
        /// The position the 'rules' keyword was defined.
        /// </summary>
        public InputPosition RulesDefinedAt
        {
            get;
            private set;
        }
    }
}