using Pote.Text;
using System;

namespace Spot.SrtL
{
    /// <summary>
    /// A SrtL string.
    /// </summary>
    public sealed class String
    {
        private InputPosition definedAt = new InputPosition();

        private string content = "";

        /// <summary>
        /// The location where the string was defined.
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
        /// The content of the string.
        /// </summary>
        public string Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                content = value;
            }
        }
    }
}