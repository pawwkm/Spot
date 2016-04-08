using System;

namespace Spot.Ebnf
{
    /// <summary>
    /// Represents a syntactic factor.
    /// </summary>
    public sealed class SyntacticFactor : Definition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SyntacticFactor"/> class
        /// that defines an <paramref name="primary"/> that is to occur once.
        /// </summary>
        /// <param name="primary">The syntactic primary of the factor.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="primary"/> is null.
        /// </exception>
        public SyntacticFactor(Definition primary)
        {
            if (primary == null)
                throw new ArgumentNullException(nameof(primary));

            SyntacticPrimary = primary;
            NumberOfRepetitions = 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntacticFactor"/> class
        /// that defines an exact number of <paramref name="repetitions "/>.
        /// </summary>
        /// <param name="primary">The syntactic primary of the factor.</param>
        /// <param name="repetitions">
        /// The exact number of times the <paramref name="primary"/> must be repeated.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="primary"/> is null.
        /// </exception>
        public SyntacticFactor(Definition primary, uint repetitions)
        {
            if (primary == null)
                throw new ArgumentNullException(nameof(primary));
 
            SyntacticPrimary = primary;
            NumberOfRepetitions = repetitions;
        }

        /// <summary>
        /// The exact number of times the 
        /// <see cref="SyntacticPrimary"/> must be repeated.
        /// </summary>
        public uint NumberOfRepetitions
        {
            get;
            private set;
        }

        /// <summary>
        /// The syntactic primary of the factor.
        /// </summary>
        public Definition SyntacticPrimary
        {
            get;
            private set;
        }
    }
}