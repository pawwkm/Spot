using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// The input of a SrtL test.
    /// </summary>
    public sealed class Input
    {
        private InputPosition definedAt = new InputPosition();

        private ConcatenatedString contents = new ConcatenatedString();

        /// <summary>
        /// The location where the 'input' keyword was defined.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public InputPosition DefinedAt
        {
            get
            {
                return definedAt;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                definedAt = value;
            }
        }

        /// <summary>
        /// The actual input.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public ConcatenatedString Contents
        {
            get
            {
                return contents;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                contents = value;
            }
        }
    }
}