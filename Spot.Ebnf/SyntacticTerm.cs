using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a syntactic term.
    /// </summary>
    public sealed class SyntacticTerm : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntacticTerm"/> class.
        /// </summary>
        /// <param name="factor">
        /// The factor contained by this term.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factor"/> is null.
        /// </exception>
        public SyntacticTerm(SyntacticFactor factor)
        {
            if (factor == null)
                throw new ArgumentNullException(nameof(factor));

            Factor = factor;
        }

        /// <summary>
        /// The factor contained by this term.
        /// </summary>
        public SyntacticFactor Factor
        {
            get;
            private set;
        }

        /// <summary>
        /// The factor that is to be excluded from <see cref="Factor"/>.
        /// If null, there is no exception for this term.
        /// </summary>
        public SyntacticFactor Exception
        {
            get;
            set;
        }
    }
}