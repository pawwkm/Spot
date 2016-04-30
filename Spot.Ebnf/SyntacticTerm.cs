using Pote.Text;
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
        /// <param name="factor">The factor contained by this term.</param>
        /// <param name="position">The position in the source where this syntactic term were defined.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factor"/> or <paramref name="position"/> is null.
        /// </exception>
        public SyntacticTerm(SyntacticFactor factor, InputPosition position) : base(position)
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